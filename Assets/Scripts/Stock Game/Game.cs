using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

[System.Serializable]
public class Game : MonoBehaviour
{

    [Header("Player Information")]
    public List<Player> playerList;
    public Player currentPlayer;
    public Player notCurrentPlayer;
    public NeuralNetworkFeedForward player1Net;
    public NeuralNetworkFeedForward player2Net;

    //Bullet tracer colors
    public Gradient player1Gradient;
    public Gradient player2Gradient;

    [Header("UI")]
    public Text currentPlayer1Text;
    public Text currentPlayer2Text;
    public Text distanceText;
    public Text winningPlayerText;
    private float distanceBetweenPlayers;
    public int totalShots = 0;

    [Header("Wind Stuffs")]
    public VisualEffect windVFX;
    public Text windSpeedText;
    public int windSpeed;
    private int windTurnCounter = 0;

    public bool inTrainingMode = true;
    public bool gameOver = false;
    public bool testDestoryPlayers;
    public bool testCreatePlayers;

    [Header("Game Object References")]
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public GameObject bulletContainer;
    public GameObject smokeVFXContainer;
    public GameObject gameOverText;
    public GameObject player1UIContainer;
    public GameObject player2UIContainer;

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

    public int TotalShots
    {
        get => totalShots;
        set
        {
            totalShots = value;
            if(GameManager.instance.playStyle[3])
                if (totalShots >= 20)
                    GameOver();
        }
    }
    #endregion


    private void Awake()
    {
        CreatePlayers();
        if (GameManager.instance.trainingMode)
        {
            GetComponentInChildren<VisualEffect>().gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// Performs multiple actions required for creating a game instance.
    /// 
    /// ResetGame() Resets certain variables back to default.
    /// DestroyGamePiecess() Destroys any existing game piece/components to clear the board for the new ones.
    /// 
    /// Creates two new players and initializes them and other related variables
    /// Adds the player networks to the respective game manager lists. 
    /// 
    /// StartTurns() Begins the game loop 
    /// 
    /// 11/19/22 Clay Brown
    /// </summary>
    public void CreatePlayers()
    {
        ResetGame();
        DestroyGamePieces();
        StopAllCoroutines();
        playerList[0] = Instantiate(player1Prefab, transform, false).GetComponent<Player>();
        playerList[1] = Instantiate(player2Prefab, transform, false).GetComponent<Player>();

        playerList[0].gameObject.name = player1Prefab.gameObject.name;
        playerList[1].gameObject.name = player2Prefab.gameObject.name;

        currentPlayer = playerList[0];
        notCurrentPlayer = playerList[1];

        playerList[0].InitPlayer(this, player1UIContainer);
        playerList[1].InitPlayer(this, player2UIContainer);
        GameManager.instance.player1Nets.Add(player1Net);
        GameManager.instance.player2Nets.Add(player2Net);

        StartTurns();
    }

    /// <summary>
    /// Loops through all game component lists and deletes the objects. Leaving only the UI and the Scene.
    /// 
    /// 11/19/22 Clay Brown
    /// </summary>
    private void DestroyGamePieces()
    {
        foreach (Player p in playerList)
        {
            if(p)
                Destroy(p.gameObject);
        }
        foreach (Bullet bullet in bulletContainer.transform.GetComponentsInChildren<Bullet>())
        {
            Destroy(bullet.gameObject);        
        }
        foreach (VisualEffect smoke in smokeVFXContainer.transform.GetComponentsInChildren<VisualEffect>())
        {
            Destroy(smoke.gameObject);
        }

    }

    void Start()
    {
        distanceText.text = DistanceBetweenPlayers.ToString();

        #region Set PlayStyle
        /*
         * Ensures only one playstyle is active, write a warning log if multiple are active.
         */
        int count = 0;  //Counter
        foreach (bool b in GameManager.instance.playStyle) //Loop through all play styles and count which are positive
            if (b)
                count++;
        if (count > 1)              //If more than one play style is positive set manual to true and everything else false.
        {
            Debug.LogWarning("Multiple Playstyles active. Current Playstyle set to Manual.");
            GameManager.instance.playStyle[0] = true;
            for (int i = 1; i < GameManager.instance.playStyle.Count; i++)
            {
                GameManager.instance.playStyle[i] = false;
            }
        }
        #endregion
    }

    private void StartTurns()
    {
        if (gameOver) return;

        if (GameManager.instance.playStyle[1])
        {
            playerList[0].InitAI(AIManager.AIType.REINFORCED);
            playerList[1].InitAI(AIManager.AIType.REINFORCED);
            StartCoroutine(HandleCurrentTurn());
        }
        else if (GameManager.instance.playStyle[2])
        {

            playerList[0].InitAI(AIManager.AIType.KNN);
            playerList[1].InitAI(AIManager.AIType.KNN);
            StartCoroutine(HandleCurrentTurn());
        }
        else if (GameManager.instance.playStyle[3])
        {
            StartCoroutine(HandleCurrentTurn());
        }
        if(!GameManager.instance.trainingMode)
            GenerateWindSpeed();
    }

    void Update()
    {
        //Update the players distance text if they have moved
        if(playerList[0] != null && playerList[1] != null)
            if (distanceText.text != ((int)DistanceBetweenPlayers).ToString() + " meters")
                distanceText.text = ((int)DistanceBetweenPlayers).ToString() + " meters";
        
        //Fail Safe to stop bug where thousands of bullets spawn
        if (bulletContainer.transform.childCount > 10)
            GameOver();


        //Testing purposes
        if (testDestoryPlayers)
        {
            testDestoryPlayers = false;
            DestroyGamePieces();
        } 
        if (testCreatePlayers)
        {
            testCreatePlayers = false;
            CreatePlayers();
        }
    }

    /// <summary>
    /// Stop the game and display the Victor or if none, a Draw
    /// 
    /// 11/19/22 Clay Brown
    /// </summary>
    public void GameOver()
    {
        StopAllCoroutines();
        gameOver = true;
        gameOverText.SetActive(true);

        //Tell the game manager this game is finished
        GameManager.instance.FinishedGames++;

        if (GameManager.instance.trainingMode)
            winningPlayerText.text = "Training Time Over.";
        else
        {
            if (playerList[0].Health <= 0)
                winningPlayerText.text = playerList[1].name + " won the game!";
            else if (playerList[1].Health <= 0)
                winningPlayerText.text = playerList[0].name + " won the game!";
            else
                winningPlayerText.text = "Game Time Exceeded.\nDraw!";
        }
    }

    /// <summary>
    /// Reset any information that needs to be reset at the start of a new game
    /// 
    /// 11/19/22 Clay Brown
    /// </summary>
    public void ResetGame()
    {
        gameOver = false;
        gameOverText.SetActive(false);
        totalShots = 0;
    }

    public IEnumerator HandleCurrentTurn()
    {
        if (gameOver) StopAllCoroutines();

        if (GameManager.instance.playStyle[1])
        {
            yield return new WaitForSeconds(GameManager.instance.timeBetweenShots);
            float[] actions;
            actions = currentPlayer.PlayerAI.getMove(currentPlayer.Distance);
            currentPlayer.Power = actions[0];
            currentPlayer.Angle = actions[1];
            currentPlayer.AiAim();
            currentPlayer.AiFire();
        }
        else if (GameManager.instance.playStyle[2])
        {
            yield return new WaitForSeconds(GameManager.instance.timeBetweenShots);
            float[] actions;
            actions = currentPlayer.PlayerAI.getMove(currentPlayer.Distance);
            currentPlayer.Power = actions[0];
            currentPlayer.Angle = actions[1];
            currentPlayer.AiAim();
            currentPlayer.AiFire();
        }
        else if (GameManager.instance.playStyle[3])
        {
            yield return new WaitForSeconds(GameManager.instance.timeBetweenShots);
            float[] inputs = new float[GameManager.instance.layers[0]];
            inputs[0] = DistanceBetweenPlayers;
            inputs[1] = currentPlayer.transform.position.y - notCurrentPlayer.transform.position.y;
            if (currentPlayer == playerList[0])
                inputs[2] = WindSpeed;
            else
                inputs[2] = -WindSpeed;
            if (inputs.Length >= 4)
                inputs[3] = currentPlayer.transform.position.x;
            currentPlayer.CallFeedForward(inputs);
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
    
    private void GenerateWindSpeed()
    {
        WindSpeed = UnityEngine.Random.Range(-10, 11);

        windVFX.SetFloat("New float", -WindSpeed / 2);

        if (WindSpeed == 0)
            windVFX.SendEvent("No Wind");
        else
            windVFX.SendEvent("Wind");

        ApplyWindSpeedToSmoke();
    }

    public void ApplyWindSpeedToSmoke()
    {
        if (smokeVFXContainer.transform.childCount > 0)
            for (int i = 0; i < smokeVFXContainer.transform.childCount; i++)
            {
                smokeVFXContainer.transform.GetChild(i).GetComponent<VisualEffect>().SetVector3("New Vector3", new Vector3(WindSpeed / 2, 1f, 0f));
            }
    }

    private void UpdateWindSpeedText(int value)
    {
        string temp = "";
        string windActual = " |" + value + "| ";
        string windIndicators = "";
        for (int i = 0; i < Mathf.Abs(value); i++)
        {
            if (value > 0)
                windIndicators += ">";
            else if (value < 0)
                windIndicators += "<";
        }
        if (value > 0)
        {
            windActual += windIndicators;
            windActual = windActual.PadLeft(windActual.Length + Mathf.Abs(value * 2), ' ');
        }
        else if (value < 0)
        {
            temp = windIndicators + windActual;
            windActual = temp;
            windActual = windActual.PadRight(windActual.Length + Mathf.Abs(value * 2), ' ');
        }
        windSpeedText.text = windActual;
    }
}
