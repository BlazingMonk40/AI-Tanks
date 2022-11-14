using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Game : MonoBehaviour
{

    [Header("Play Style")]
    [Tooltip("[0] Manual, [1]Reinforced, [2]NEAT")]
    [SerializeField] public List<bool> playStyle = new List<bool>(3);

    [Header("Player Information")]
    public List<Player> playerList;
    public Player currentPlayer;
    public Player notCurrentPlayer;

    [Header("UI")]
    public Text currentPlayer1Text;
    public Text currentPlayer2Text;
    public Text distanceText;
    private float distanceBetweenPlayers;

    [TextArea]
    public string windSpeedInspector;
    public Text windSpeedText;
    public int windSpeed;
    private int windTurnCounter = 0;

    public bool gameOver = false;
    private GameObject bulletContainer;
    

    public float DistanceBetweenPlayers
    {
        get
        {
            return playerList[1].transform.position.x - playerList[0].transform.position.x;
        }

        set => distanceBetweenPlayers = value;
    }

    public int WindSpeed 
    { 
        get => windSpeed;
        set
        {
            windSpeed = value;
            UpdateWindSpeedText(value);
        }
    }

    private void GenerateWindSpeed()
    {
        WindSpeed = UnityEngine.Random.Range(-10, 11);
    }
    private void UpdateWindSpeedText(int value)
    {
        string temp = "";
        string windActual = " |" + value + "| ";
        string windIndicators = "";
        for(int i = 0; i < Mathf.Abs(value); i++)
        {
            if (value > 0)
                windIndicators += ">";
            else if (value < 0)
                windIndicators += "<";
        }
        if (value > 0)
        {
            windActual += windIndicators;
            windActual = windActual.PadLeft(windActual.Length + Mathf.Abs(value*2), ' ');
        }
        else if (value < 0)
        {
            temp = windIndicators + windActual;
            windActual = temp;
            windActual = windActual.PadRight(windActual.Length + Mathf.Abs(value*2), ' ');
        }
        windSpeedInspector = windActual;
        windSpeedText.text = windActual;
    }
    private void Awake()
    {
        //Set current player
        currentPlayer = playerList[0];
        notCurrentPlayer = playerList[1];

        foreach (Player player in playerList)
        {
            player.InitPlayer(this);
        }

        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).name == "Bullet Container")
            {
                bulletContainer = transform.GetChild(i).gameObject;
            }
        }
    }

    void Start()
    {
        GenerateWindSpeed();

        distanceText.text = DistanceBetweenPlayers.ToString();

        #region Set PlayStyle
        /*
         * Ensures only one playstyle is active, write a warning log if multiple are active.
         */
        int count = 0;  //Counter
        foreach (bool b in playStyle) //Loop through all play styles and count which are positive
            if (b)
                count++;
        if (count > 1)              //If more than one play style is positive set manual to true and everything else false.
        {
            Debug.LogWarning("Multiple Playstyles active. Current Playstyle set to Manual.");
            playStyle[0] = true;
            for (int i = 1; i < playStyle.Count; i++)
            {
                playStyle[i] = false;
            }
        }
        #endregion

        

        if (playStyle[1])
        {
            playerList[0].InitAI(AIManager.AIType.REINFORCED);
            playerList[1].InitAI(AIManager.AIType.REINFORCED);
            StartCoroutine(HandleCurrentTurn());
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (distanceText.text != ((int)DistanceBetweenPlayers).ToString() + " meters")
            distanceText.text = ((int)DistanceBetweenPlayers).ToString() + " meters";
        if (bulletContainer.transform.childCount > 10)
            gameOver = true;
    }
    public IEnumerator HandleCurrentTurn()
    {
        if (gameOver)
            Debug.LogAssertion(name + ": GAME OVER!");
        else
        {
            if (playStyle[1])
            {
                yield return new WaitForSeconds(1f);
                float[] actions;
                actions = currentPlayer.PlayerAI.getMove(currentPlayer.Distance);
                currentPlayer.Power = actions[0];
                currentPlayer.AiAim(actions[1]);
                currentPlayer.AiFire();
            }
        }
    }
    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }
    public void EndTurn()
    {
        windTurnCounter++;
        if (windTurnCounter == 2)
        {
            windTurnCounter = 0;
            GenerateWindSpeed();
        }
        ToggleCurrentPlayerText();
        ChangeCurrentPlayer();
        StartCoroutine(HandleCurrentTurn());
    }
    private void ChangeCurrentPlayer()
    {
        if (currentPlayer == playerList[0])
        {
            currentPlayer = playerList[1];
            notCurrentPlayer = playerList[0];
        }
        else
        {
            currentPlayer = playerList[0];
            notCurrentPlayer = playerList[1];
        }
    }

    public void ToggleCurrentPlayerText()
    {
        if (currentPlayer1Text.enabled)
        {
            currentPlayer1Text.enabled = false;
            currentPlayer2Text.enabled = true;
        }
        else
        {
            currentPlayer1Text.enabled = true;
            currentPlayer2Text.enabled = false;
        }
    }
}
