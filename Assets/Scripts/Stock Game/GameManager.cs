using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Play Style")]
    [Tooltip("[0] Manual, [1]Reinforced, [2]ANN, [3]FeedForward")]
    [SerializeField] public List<bool> playStyle = new List<bool>(4);


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

    private int generationNumber;
    public bool isTraining = false;
    public int[] layers = new int[] { 3, 9, 2 };

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
        if(playStyle[3])
            FeedForward();
    }

    private void FeedForward()
    {
        if (isTraining == false)
        {
            MakeNewGeneration(player1Nets, "Assets/Scripts/AI/Clay FeedForward/Player1Save.txt");
            MakeNewGeneration(player2Nets, "Assets/Scripts/AI/Clay FeedForward/Player2Save.txt");


            //Let them train for some time then reset !!!Reset not implemented 10:41_11/15/22!!!
            isTraining = true;
            //Invoke("Timer", 30f);
            //RespawnAllGames();
            generationNumber++;
        }
    }

    private void MakeNewGeneration(List<NeuralNetworkFeedForward> nets, string path)
    {
        //Sorts in worst to best order 0=worst length-1=best
        nets.Sort();

        //Saves Single best fitness network - Probably want to change to top 10% later
        nets[nets.Count - 1].Save(path);

        //Repopulate half of the neural network and mutate
        for (int i = 0; i < nets.Count / 2; i++)
        {
            nets[i] = new NeuralNetworkFeedForward(nets[i + (nets.Count / 2)]);
            nets[i].Mutate();

            nets[i + (nets.Count / 2)] = new NeuralNetworkFeedForward(nets[i + (nets.Count / 2)]); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
        }
    }

    internal void RespawnThisGame(Game thisGame)
    {
        isTraining = false;
        foreach (GameObject game in gameList)
        {
            if(game == thisGame)
                game.GetComponent<Game>().CreatePlayers();
        }
    }

    private void RespawnAllGames()
    {
        foreach (GameObject game in gameList)
        {
            game.GetComponent<Game>().CreatePlayers();
        }
    }

    private void Timer()
    {
        isTraining = false;
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
