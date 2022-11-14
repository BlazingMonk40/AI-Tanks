using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    // Update is called once per frame
    //private float rotationOffset = 90f;
    public float rotationSpeed = 25;
    public string teamTag;
    private Player player;
    private Game game;

    public Player Player { get => player; set => player = value; }
    public Game Game { get => game; set => game = value; }

    private void Start()
    {
        
        if (rotationSpeed <= 25)
            rotationSpeed = 25;
    }

    public void Raise()
    {
        gameObject.transform.localEulerAngles += new Vector3(transform.localEulerAngles.z - 1f * Time.deltaTime * rotationSpeed, 0f, 0f);
    }

    public void Lower()
    {
        gameObject.transform.localEulerAngles -= new Vector3(transform.localEulerAngles.z - 1f * Time.deltaTime * rotationSpeed, 0f, 0f);
    }

}
