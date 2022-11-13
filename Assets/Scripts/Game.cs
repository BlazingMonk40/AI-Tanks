using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Game : MonoBehaviour
{

    [Header("Play Style")]
    [Tooltip("[0] Manual, [1]Reinforced, [2]NEAT")]
    [SerializeField] private List<bool> playStyle = new List<bool>(3);

    [Header("Player Information")]
    public List<Player> playerList;
    public Player currentPlayer;
    public Player notCurrentPlayer;

    public List<PlayerInfo> player1InfoList;

    public List<PlayerInfo> player2InfoList;

    [Header("UI")]
    public Text currentPlayer1Text;
    public Text currentPlayer2Text;
    public Text distanceText;
    private float distanceBetweenPlayers;

    public float DistanceBetweenPlayers
    {
        get
        {
            return playerList[1].transform.position.x - playerList[0].transform.position.x;
        }

        set => distanceBetweenPlayers = value;
    }

    public List<bool> GetPlayStyle { get => playStyle; }

    void Start()
    {
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

        //Set current player
        currentPlayer = playerList[0];
        notCurrentPlayer = playerList[1];

        foreach (Player player in playerList)
        {
            player.InitPlayer(this);
        }

        if (playStyle[1])
        {
            playerList[0].InitAI(AI.AIType.REINFORCED);
            playerList[1].InitAI(AI.AIType.REINFORCED);
            StartCoroutine(HandleCurrentTurn());
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (distanceText.text != ((int)DistanceBetweenPlayers).ToString() + " meters")
            distanceText.text = ((int)DistanceBetweenPlayers).ToString() + " meters";
    }
    public IEnumerator HandleCurrentTurn()
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
    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }
    public void EndTurn()
    {
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
