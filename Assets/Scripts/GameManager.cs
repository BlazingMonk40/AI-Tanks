using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float gameOffset = 150f;
    public int numberGames;
    public GameObject gamePrefab;
    public List<Game> gameList;
    public static GameManager instance;

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
    }

    private void InitGames()
    {
        for(int i =0; i < numberGames; i++)
        {
            GameObject game = Instantiate(gamePrefab, new Vector3(0f, 0f, gameOffset * i), Quaternion.identity);
            gameList.Add(game.GetComponent<Game>());
        }
    }
}
