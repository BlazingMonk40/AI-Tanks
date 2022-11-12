using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Transform turretTransform;

    public List<Player> playerList;
    public Player currentPlayer;

    public List<PlayerInfo> player1InfoList;

    public List<PlayerInfo> player2InfoList;

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
        distanceText.text = DistanceBetweenPlayers.ToString();

        currentPlayer = playerList[0];
        playerList[0].InitAI(AI.AIType.REINFORCED);
        playerList[1].InitAI(AI.AIType.REINFORCED);
        StartCoroutine(HandleCurrentTurn());
    }
    

    // Update is called once per frame
    void Update()
    {
        if(distanceText.text != ((int)DistanceBetweenPlayers).ToString() + " meters")
            distanceText.text = ((int)DistanceBetweenPlayers).ToString() + " meters";
    }
    public IEnumerator HandleCurrentTurn()
    {
        yield return new WaitForSeconds(2.5f);
        float[] actions;
        actions = currentPlayer.PlayerAI.getMove(currentPlayer.Distance);
        currentPlayer.Power = actions[0];
        currentPlayer.AiAim(actions[1]);
        currentPlayer.AiFire();
    }
    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }
    public void EndTurn()
    {
        StartCoroutine(ChangeCurrentPlayer());
        StartCoroutine(HandleCurrentTurn());
    }
    internal IEnumerator ChangeCurrentPlayer()
    {
        yield return new WaitForEndOfFrame();
        if (currentPlayer == playerList[0])
            currentPlayer = playerList[1];
        else
            currentPlayer = playerList[0];
    }
}
