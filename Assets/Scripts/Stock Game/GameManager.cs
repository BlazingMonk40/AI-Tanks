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
    public GameObject gamePrefab;
    public List<GameObject> gameList;
    public static GameManager instance;

    [Header("Neural Net")]
    public int populationSize;
    public List<NeuralNetworkFeedForward> nets;

    private int generationNumber;
    public bool isTraining = false;
    public int[] layers = new int[] { 3, 9, 2 };



    private void Awake()
    {
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
        InitGames();
        //InitNeuralNetworks();

        Time.timeScale = timeScale;
    }
    private void Update()
    {
        /*if (isTraining == false)
        {
            if (generationNumber == 0)
            {
                InitNeuralNetworks();
            }
            else
            {
                //Sorts in worst to best order 0=worst length-1=best
                nets.Sort();
                
                //Saves Single best fitness network - Probably want to change to top 10% later
                nets[populationSize - 1].Save("Assets/Scripts/AI/ANN/Save.txt");
                
                //Repopulate half of the neural network and mutate
                for (int i = 0; i < populationSize / 2; i++)
                {
                    nets[i] = new NeuralNetworkFeedForward(nets[i + (populationSize / 2)]);
                    nets[i].Mutate();

                    nets[i + (populationSize / 2)] = new NeuralNetworkFeedForward(nets[i + (populationSize / 2)]); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
                }
            }

            //Let them train for some time then reset !!!Reset not implemented 10:41_11/15/22!!!
            isTraining = true;
            Invoke("Timer", 20f);
            generationNumber++;
        }*/
    }

    private void Timer()
    {
        isTraining = false;
    }

    void InitNeuralNetworks()
    {
        //population must be even, just setting it to 20 incase it's not
        if (populationSize % 2 != 0)
        {
            populationSize = 20;
        }

        nets = new List<NeuralNetworkFeedForward>();

        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetworkFeedForward net = new NeuralNetworkFeedForward(layers);
            net.Load("Assets/Scripts/AI/ANN/Save.txt");
            net.Mutate();
            nets.Add(net);
        }
    }

    private void InitGames()
    {
        for(int i =0; i < numberGames; i++)
        {
            GameObject game = Instantiate(gamePrefab, new Vector3(0f, 0f, gameOffset * i), Quaternion.identity);
            game.name = "Game: " + i;
            gameList.Add(game);
        }

        /*for(int i = 0; i < gameList.Count; i++)
        {

        }*/
    }
}
