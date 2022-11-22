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
    private int generationNumber;
    public bool isTraining = false;
    public int[] layers = new int[] { 3, 4, 2 };
    
    public Vector3 player1SpawnPosition;
    public Vector3 player2SpawnPosition;

    //Generation Card stuff
    private bool firstEntryForGenCard; //just for formatting the gen. card
    private bool showTrainingGameDistance = true; //Display the current game's distance

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
        numberOfParents = (int)(numberGames * .2f);

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

    void Start()
    {
        populationSize = numberGames * 2;

        InitGames();
        Time.timeScale = timeScale;
    }
    private void Update()
    {
        
    }

    private void FeedForward()
    {
        if (isTraining == false)
        {
            MakeNewGeneration(player1Nets, "Assets/Scripts/AI/Clay FeedForward/BestNets/Player1/", "Player1", false);
            MakeNewGeneration(player2Nets, "Assets/Scripts/AI/Clay FeedForward/BestNets/Player2/", "Player2", true);

            generationNumber++;
            isTraining = true;
        }
    }

    private void MakeNewGeneration(List<NeuralNetworkFeedForward> nets, string path, string playername, bool makeNewLine)
    {
        //Sorts in worst to best order 0=worst length-1=best
        nets.Sort();

        float topFit = 0;
        float topHit = 0;
        for (int i = 0; i < numberOfParents; i++)
        {
            nets[nets.Count - i - 1].Save(path + "parent_" + i + ".txt");
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
        GenerationCard("Assets/Scripts/AI/Clay FeedForward/GenerationCard.txt", playername, topFit/numberOfParents, topHit, 
                        bottomFit/(numberGames - numberOfParents), bottomHit, makeNewLine);
    }


    private void RespawnAllGames()
    {
        player1SpawnPosition = new Vector3(UnityEngine.Random.Range(-120f, -50), -15f, -25f);
        player2SpawnPosition = new Vector3(UnityEngine.Random.Range(50f, 120f), -30f, -25f);

        isTraining = false;
        mutatePlayer1Number = 0;
        mutatePlayer2Number = 0;
        if (playStyle[3])
            FeedForward();
        foreach (GameObject game in gameList)
        {
            game.GetComponent<Game>().CreatePlayers();
        }
    }

    public void GenerationCard(string path, string playername, float topFit, float topHit, float bottomFit, float bottomHit, bool makeNewLine)
    {
        if(!File.Exists(path)) File.Create(path).Close();
        
        StreamWriter writer = new StreamWriter(path, true);

        if (!firstEntryForGenCard)
        {
            writer.WriteLine("------------------------------------------\n");
            writer.WriteLine($"Game Information:\nNumber of Games: {numberGames}\nTime Game Started: {DateTime.Now}\n");
            writer.WriteLine("------------------------------------------\n");
            firstEntryForGenCard = true;
        }
        if (showTrainingGameDistance && trainingMode)
        {
            writer.WriteLine($"Game Distance: {player2SpawnPosition.x - player1SpawnPosition.x}\n");
            showTrainingGameDistance = false;
        }
        else
            showTrainingGameDistance = true;
        
        writer.WriteLine($"{playername} Generation: {generationNumber}\n\t" +
                            $"Avg Fitness of top 80% = {topFit}\n\t" +
                            $"Number of Hits in the top 80% = {topHit}/{numberOfParents * 10}\n\t" +
                            $"Avg Fitness of the bottom 20% = {bottomFit}\n\t" +
                            $"Number of Hits in the bottom 20% = {bottomHit}/{(numberGames - numberOfParents) * 10}\n");
        if (makeNewLine)
            writer.WriteLine("------------------------------------------\n");
        writer.Close();

        WriteGenCardToInspector(path);
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
        SpawnGamesGridStyle(1);        
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
        if (gridSize < 2)
            SpawnNormalGames();
        else
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
    }
}
