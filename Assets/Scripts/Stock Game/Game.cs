using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

[System.Serializable]
public class Game : MonoBehaviour
{

    [Header("Play Style")]
    [Tooltip("[0] Manual, [1]Reinforced, [2]ANN, [3]FeedForward")]
    [SerializeField] public List<bool> playStyle = new List<bool>(4);

    [Header("Player Information")]
    public List<Player> playerList;
    public Player currentPlayer;
    public Player notCurrentPlayer;

    public Gradient player1Gradient;
    public Gradient player2Gradient;

    [Header("UI")]
    public Text currentPlayer1Text;
    public Text currentPlayer2Text;
    public Text distanceText;
    public GameObject gameOverText;
    public Text winningPlayerText;
    private float distanceBetweenPlayers;


    [TextArea]
    public string windSpeedInspector;
    public Text windSpeedText;
    public int windSpeed;
    private int windTurnCounter = 0;

    public bool gameOver = false;
    private GameObject bulletContainer;
    private GameObject smokeVFXContainer;

    #region Properties
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
    #endregion

    private void GenerateWindSpeed()
    {
        WindSpeed = UnityEngine.Random.Range(-10, 11);
        ApplyWindSpeedToSmoke();
    }

    public void ApplyWindSpeedToSmoke()
    {
        if(smokeVFXContainer.transform.childCount > 0)
            for(int i = 0; i < smokeVFXContainer.transform.childCount; i++)
            {
                smokeVFXContainer.transform.GetChild(i).GetComponent<VisualEffect>().SetVector3("New Vector3", new Vector3(WindSpeed/2, 1f, 0f));
            }
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

        bulletContainer = GameObject.Find("Bullet Container");
    }

    void Start()
    {
        smokeVFXContainer = gameObject.transform.GetChild(transform.childCount - 1).transform.GetChild(1).gameObject; //Get the last child "Containers" and get it's second child "Impact Smoke Container" <- Shitty hard coding I can be bothered to do the right way.

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

        GenerateWindSpeed();
        if (playStyle[1])
        {
            playerList[0].InitAI(AIManager.AIType.REINFORCED);
            playerList[1].InitAI(AIManager.AIType.REINFORCED);
            StartCoroutine(HandleCurrentTurn());
        }
        else if (playStyle[2])
        {

            playerList[0].InitAI(AIManager.AIType.ANN);
            playerList[1].InitAI(AIManager.AIType.ANN);
            StartCoroutine(HandleCurrentTurn());

        }
        else if(playStyle[3])
        {
            StartCoroutine(HandleCurrentTurn());

        }
    }


    // Update is called once per frame
    void Update()
    {
        if (distanceText.text != ((int)DistanceBetweenPlayers).ToString() + " meters")
            distanceText.text = ((int)DistanceBetweenPlayers).ToString() + " meters";
        
        //Fail Safe to stop bug where thousands of bullets spawn
        if (bulletContainer.transform.childCount > 10)
            GameOver();
    }

    public void GameOver()
    {
        gameOver = true;
        gameOverText.SetActive(true);
        winningPlayerText.text = currentPlayer.name + " won the game!";
    }


    public IEnumerator HandleCurrentTurn()
    {
        if (gameOver)
        {
            //Debug.LogAssertion(name + ": GAME OVER!");
        }
        else
        {
            if (playStyle[1])
            {
                yield return new WaitForSeconds(1f);
                float[] actions;
                actions = currentPlayer.PlayerAI.getMove(currentPlayer.Distance);
                currentPlayer.Power = actions[0];
                currentPlayer.Angle = actions[1];
                currentPlayer.AiAim();
                currentPlayer.AiFire();
            }
            else if (playStyle[2])
            {
                yield return new WaitForSeconds(1f);
                float[] actions;
                actions = currentPlayer.PlayerAI.getMove(currentPlayer.Distance);
                currentPlayer.Power = actions[0];
                currentPlayer.Angle = actions[1];
                currentPlayer.AiAim();
                currentPlayer.AiFire();
            }
            else if (playStyle[3])
            {
                yield return new WaitForSeconds(1f);
                float[] inputs = new float[3];
                inputs[0] = DistanceBetweenPlayers;
                inputs[1] = currentPlayer.transform.position.y - notCurrentPlayer.transform.position.y;
                inputs[2] = Mathf.Abs(WindSpeed);
                currentPlayer.CallFeedForward(inputs);
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
