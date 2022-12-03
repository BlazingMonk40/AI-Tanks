using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Play Style")]
    [Tooltip("[0] Manual, [1]Reinforced, [2]ANN, [3]FeedForward")]
    [SerializeField] public List<bool> playStyle = new List<bool>(4);
    public bool trainingMode;

    [Header("Generation Card")]
    [TextArea(5,20)]
    public string genCardTextArea;


    [Range(.5f, 8f)]
    public float timeScale;
    public float timeBetweenShots;
    private float gameOffset = 150f;
    public int numberGames;
    public int finishedGames;
    public GameObject gamePrefab;
    public List<GameObject> gameList;
    public static GameManager instance;

    [Header("Neural Net")]
    public int populationSize;
    public List<NeuralNetworkFeedForward> player1Nets;
    public List<NeuralNetworkFeedForward> player2Nets;
    public int mutatePlayer1Number;
    public int mutatePlayer2Number;
    public int numberOfParents = 0;
    private int generationNumber = 1;
    public bool isTraining = false;
    public int[] layers;
    internal string layersString;
    private DateTime gameStartTime;
    
    public Vector3 player1SpawnPosition;
    public Vector3 player2SpawnPosition;

    //Generation Card stuff
    private bool firstEntryForGenCard = true; //just for formatting the gen. card
    private bool showTrainingGameDistance = true; //Display the current game's distance
    private int gameCounter;

    //ShotHistory
    public string shotHistoryPath;

    public int FinishedGames
    {
        get => finishedGames;
        set
        {
            finishedGames = value;
            if (finishedGames == numberGames)
            {
                finishedGames = 0;
                RespawnAllGames();

            }
        }
    }

    private void Awake()
    {
        SetShotHistoryFile();
        numberOfParents = (int)(numberGames * .2f);
        layersString = GetLayers();
        player1Nets = new List<NeuralNetworkFeedForward>();
        player2Nets = new List<NeuralNetworkFeedForward>();
        #region Class Instance
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        #endregion
    }

    public void SetShotHistoryFile()
    {
        string path = "Assets/Scripts/AI/ShotHistory/Main/";
        string[] files = Directory.GetFiles(path);
        int count = 0;
        string file = "";
        //Check size pick one to append to
        for (int i = 0; i < files.Length; i++)
        {
            string[] fileParts = files[i].Split('/');
            file = fileParts[fileParts.Length - 1];
            if (files[i].EndsWith(".meta") || new FileInfo(path + file).Length >= 100000000)
            {
                count++;
                continue;
            }
            else
                path = path + file;
        }
        if (count == files.Length)
        {
            path = path + "ShotHistory_" + ((count / 2) + 1) + ".txt";
            File.Create(path);
        }
        shotHistoryPath = path;
    }

    void Start()
    {
        populationSize = numberGames * 2;
        gameStartTime = DateTime.Now;
        InitGames();
        Time.timeScale = timeScale;
    }

    private void FeedForward()
    {
        SaveBestNetworks(player1Nets, $"Assets/Scripts/AI/Clay FeedForward/BestNets/{layersString}/Player1", "Player1", false);
        SaveBestNetworks(player2Nets, $"Assets/Scripts/AI/Clay FeedForward/BestNets/{layersString}/Player2", "Player2", true);

        generationNumber++;
        player1Nets = new List<NeuralNetworkFeedForward>();
        player2Nets = new List<NeuralNetworkFeedForward>();
    }

    private void SaveBestNetworks(List<NeuralNetworkFeedForward> nets, string path, string playername, bool makeNewLine)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        //Sorts in worst to best order 0=worst length-1=best
        nets.Sort();

        float topFit = 0;
        float topHit = 0;

        //Saves off the top preformers and gathers data about how they performed
        for (int i = 0; i < numberOfParents; i++)
        {
            if(trainingMode)
                nets[nets.Count - i - 1].Save(path + "/parent_" + i + ".txt");
            topFit += nets[nets.Count - i - 1].GetFitness();
            topHit += nets[nets.Count - i - 1].Hits;

        }
        float bottomFit = 0;
        float bottomHit = 0;
        for (int i = 0; i < numberGames - numberOfParents; i++)
        {
            bottomFit += nets[i].GetFitness();
            bottomHit += nets[i].Hits;

        }
        GenerationCard(path.Substring(0, path.LastIndexOf('/')) + "/GenerationCards", playername, topFit/numberOfParents, topHit, 
                        bottomFit/(numberGames - numberOfParents), bottomHit, makeNewLine);
    }

    public void GenerationCard(string path, string playername, float topFit, float topHit, float bottomFit, float bottomHit, bool makeNewLine)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        string filePath = path + "/GenerationCard_" + gameStartTime.ToString().Replace('/', '_').Replace(':', ',') + ".txt";
        if (firstEntryForGenCard)
        {
            File.Create(filePath).Close();
        }

        StreamWriter writer = new StreamWriter(filePath, true);

        if (firstEntryForGenCard)
        {
            writer.WriteLine("------------------------------------------\n");
            writer.WriteLine($"Game Information:\nNumber of Games: {numberGames}\nTime Game Started: {gameStartTime}\n");
            writer.WriteLine("------------------------------------------\n");
            firstEntryForGenCard = false;
        }
        if (showTrainingGameDistance && trainingMode)
        {
            writer.WriteLine($"Game Distance: {player2SpawnPosition.x - player1SpawnPosition.x}\n");
            showTrainingGameDistance = false;
        }
        else
            showTrainingGameDistance = true;
        
        writer.WriteLine($"{playername} Generation: {generationNumber}\n\t" +
                            $"Avg Fitness of top 20% = {topFit}\n\t" +
                            $"Number of Hits in the top 20% = {topHit}/{numberOfParents * 10}\n\t" +
                            $"Avg Fitness of the bottom 80% = {bottomFit}\n\t" +
                            $"Number of Hits in the bottom 80% = {bottomHit}/{(numberGames - numberOfParents) * 10}\n");
        if (makeNewLine)
            writer.WriteLine("------------------------------------------\n");
        writer.Close();

        //WriteGenCardToInspector(path);
    }

    private void WriteGenCardToInspector(string path)
    {
        if (!File.Exists(path)) return;

        StreamReader reader = new StreamReader(path);
        genCardTextArea = reader.ReadToEnd();
        reader.Close();
    }

    private void InitGames()
    {
        player1SpawnPosition = new Vector3(UnityEngine.Random.Range(-120f, -50), -15f, -25f);
        player2SpawnPosition = new Vector3(UnityEngine.Random.Range(50f, 120f), -30f, -25f);
        gameCounter = 0;
        SpawnGames(1);        
    }
    private void RespawnAllGames()
    {
        if (playStyle[3])
            FeedForward();
        gameCounter++;
        if (gameCounter == 5)
        {
            gameCounter = 1;
            player1SpawnPosition = new Vector3(UnityEngine.Random.Range(-120f, -50), -15f, -25f);
            player2SpawnPosition = new Vector3(UnityEngine.Random.Range(50f, 120f), -30f, -25f);
        }
        mutatePlayer1Number = 0;
        mutatePlayer2Number = 0;

        foreach (GameObject game in gameList)
        {
            game.GetComponent<Game>().CreatePlayers();
        }
    }

    private void SpawnGames(int gridSize)
    {
        if (gridSize < 2)
            SpawnNormalGames();
        else
            SpawnGamesGridStyle(gridSize);
    }
    private void SpawnNormalGames()
    {
        for (int i = 0; i < numberGames; i++)
        {
            GameObject game = Instantiate(gamePrefab, new Vector3(0f, 0f, gameOffset * i), Quaternion.identity);
            game.name = "Game: " + i;
            gameList.Add(game);
        }
    }

    private void SpawnGamesGridStyle(int gridSize)
    {
        for (int i = 0; i < numberGames / gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                GameObject game = Instantiate(gamePrefab, new Vector3(500 * j, 0f, gameOffset * i), Quaternion.identity);
                game.name = "Game: " + (j + (i * gridSize));
                gameList.Add(game);
            }
        }
    }

    /// <summary>
    /// Simply converts the layers array elements to a string.
    /// The returned string is used to specify a "neuron configuration" folder in which we'll save the networks actually using that config.
    /// See below for examples.
    /// <para> If we have the network layers configured as {3, 4, 2}, we'll save the best networks in ./Player[1/2]/{3, 4, 2}/...</para>
    /// <para> If we have the network layers configured as {3, 9, 4, 2}, we'll save the best networks in ./Player[1/2]/{3, 9, 4, 2}/...</para>
    /// 
    /// </summary>
    /// <returns></returns>
    private string GetLayers()
    {
        string rtn = "{";
        for (int i = 0; i < layers.Length; i++)
        {
            if (i < layers.Length - 1)
                rtn += layers[i] + ", ";
            else
                rtn += layers[i] + "}";
        }
        return rtn;
    }
}
