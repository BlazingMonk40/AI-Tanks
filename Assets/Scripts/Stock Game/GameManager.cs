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
    public int[] layers = new int[] { 3, 9, 2 };
    private float avgFitness;

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
            generationNumber++;
            MakeNewGeneration(player1Nets, "Assets/Scripts/AI/Clay FeedForward/BestNets/Player1/", "Player1", false);
            MakeNewGeneration(player2Nets, "Assets/Scripts/AI/Clay FeedForward/BestNets/Player2/", "Player2", true);


            isTraining = true;
        }
    }

    private void MakeNewGeneration(List<NeuralNetworkFeedForward> nets, string path, string playername, bool makeNewLine)
    {
        //Sorts in worst to best order 0=worst length-1=best
        nets.Sort();

        float tempFit = 0;
        float tempHit = 0;
        for (int i = 0; i < numberOfParents; i++)
        {
            nets[nets.Count - i - 1].Save(path + "parent_" + i + ".txt");
            tempFit += nets[nets.Count - i - 1].GetFitness();
            tempHit += nets[nets.Count - i - 1].Hits;

        }
        GenerationCard("Assets/Scripts/AI/Clay FeedForward/GenerationCard.txt", playername, tempFit/numberOfParents, tempHit, makeNewLine);
    }


    private void RespawnAllGames()
    {
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

    public void GenerationCard(string path, string playername, float fit, float hit, bool makeNewLine)
    {
        if (File.Exists(path) && generationNumber > 0)
        {
            StreamWriter writer = new StreamWriter(path, true);
            //Variabless to be saved

            
            
            writer.WriteLine($"{playername} Generation: {generationNumber - 1} \n Avg Fitness of top 20% = {fit} \n Number of Hits in the top 20% = {hit}\n");
            if (makeNewLine)
                writer.WriteLine("------------------------------------------\n");
            writer.Close();
        }
        else
            File.Create(path).Close();
    }

    private void InitGames()
    {
        //SpawnNormalGames();
        SpawnGamesGridStyle(5);
        
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
}
