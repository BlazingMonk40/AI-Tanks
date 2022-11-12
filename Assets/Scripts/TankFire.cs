using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankFire : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float powerMultiplier = .75f;
    public float power;
    public GameObject bulletContainer;
    public string teamTag;
    private Player player;
    void Start()
    {
        if (gameObject.transform.parent.parent.parent.tag == "Player1")
        {
            teamTag = "Player1";
            player = GameManager.instance.playerList[0];
        }
        else if (gameObject.transform.parent.parent.parent.tag == "Player2")
        {
            teamTag = "Player2";
            player = GameManager.instance.playerList[1];
        }
    }

    public float IncreasePower(float power)
    {
        return power += 10 * Time.deltaTime;
    }

    public float DecreasePower(float power)
    {
        return power -= 10 * Time.deltaTime;
    }

}

