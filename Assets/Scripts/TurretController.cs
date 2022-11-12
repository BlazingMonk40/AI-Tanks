using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    // Update is called once per frame
    private float rotationOffset = 90f;
    public float rotationSpeed = 25;
    public string teamTag;
    private Player player;

    private void Start()
    {
        if (gameObject.transform.parent.tag == "Player1")
        {
            teamTag = "Player1";
            player = GameManager.instance.playerList[0];
        }
        else
        {
            teamTag = "Player2";
            player = GameManager.instance.playerList[1];
        }
        
        if (rotationSpeed <= 25)
            rotationSpeed = 25;
    }
    void Update()
    {
        
    }
    public void Raise()
    {
        gameObject.transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + 1f * Time.deltaTime * rotationSpeed);
    }

    public void Lower()
    {
        gameObject.transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z - 1f * Time.deltaTime * rotationSpeed);
    }

}
