using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
public class Player : MonoBehaviour
{
    private float rotationOffset = 90f;
    public float powerMultiplier = 1f;
    private float power;
    private float angle;
    private float distance;
    private bool myTurn;
    AI playerAI;


    


    private TankFire fire;
    private TurretController turret;
    private GameObject turretObj;
    private GameObject bulletSpawner; 
    public GameObject bulletPrefab;
    public GameObject bulletContainer;

    public GameObject playerContainer;
    private Text powerText;
    private Text angleText;
    private Text distanceText;
    private Game game;

    private Quaternion stockRotation;
    public float Power
    {
        get => power;
        set
        {
            power = value;
            SetPowerText(((int)value).ToString());
        }
    }
    public float Angle
    {
        get => angle;
        set
        {
            angle = value;
            SetAngleText(((int)value).ToString());
        }
    }
    public float Distance
    {
        get => distance;
        set
        {
            distance = value;
            SetDistanceText(((int)value).ToString());
        }
    }
    public AI PlayerAI
    {
        get => playerAI;
        set => playerAI = value;
    }
    public Game Game 
    { 
        get => game; 
        set => game = value; 
    }

    public void InitAI(AI.AIType type)
    {
        PlayerAI = new AI(type);
    }

    void Awake()
    {
        #region Text Info
        if (gameObject.tag == "Player1")
        {
            playerContainer = GameObject.Find("Player 1");
        }
        else if (gameObject.tag == "Player2")
        {
            playerContainer = GameObject.Find("Player 2");
        }
        powerText = playerContainer.transform.GetChild(1).gameObject.GetComponent<Text>();
        angleText = playerContainer.transform.GetChild(2).gameObject.GetComponent<Text>();
        distanceText = playerContainer.transform.GetChild(3).gameObject.GetComponent<Text>();
        #endregion
    }

    internal void InitPlayer(Game game)
    {
        Debug.Log(transform.localPosition);
        if (gameObject.tag == "Player1")
        {
            transform.localPosition = new Vector3(-75f, -20f, -25f);
        }
        else if (gameObject.tag == "Player2")
        {
            transform.localPosition = new Vector3(75f, -35f, -25f);
        }
        Debug.Log(transform.localPosition);
        Game = game;
        fire = GetComponentInChildren<TankFire>();
        fire.Player = this;
        fire.Game = this.Game;
        turret = GetComponentInChildren<TurretController>();
        turret.Player = this;
        turret.Game = this.Game;
        turretObj = transform.GetChild(0).gameObject;
        bulletSpawner = fire.gameObject;
        Power = 50f;
        Angle = 0f;
        Distance = 0f;
    }

    private void Update()
    {
        //FixAxis();
        
        if (Game.GetPlayStyle[0]) //True for Manual playstyle
        {
            HandleFiring();
            HandleAiming();
        }
        if(Game.GetPlayStyle[1])
        {
            Debug.DrawRay(bulletSpawner.transform.position, bulletSpawner.transform.forward * (Power * powerMultiplier) / 2, Color.red);
        }
    }

    private void FixAxis()
    {
        if (transform.localEulerAngles.x != 0f)
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
        if (transform.position.z != -320f)
            transform.position = new Vector3(transform.position.x, transform.position.y, -15f);
    }

    public void AiAim(float dir)
    {
        if (gameObject.tag == "Player2")
        {
            //Debug.Log("Player2 Angle: " + dir);
            turretObj.transform.localEulerAngles = new Vector3(0f, 0f, dir);
            Angle = turretObj.transform.localEulerAngles.z;
            ConstrainRotations();
        }
        else
        {
            //Debug.Log("Player1 Angle: " + dir);
            turretObj.transform.localEulerAngles = new Vector3(0f, 0f, dir);
            Angle = turretObj.transform.localEulerAngles.z;
            ConstrainRotations();
        }

    }

    private void ConstrainRotations()
    {
        //Constrain turret rotations
        if (turretObj.transform.localEulerAngles.z > 180f)
        {
            Debug.LogWarning(this.tag + " angle greater than 180f");
            turretObj.transform.localEulerAngles = new Vector3(0f, 0f, 0f - rotationOffset);
        }
        if (turretObj.transform.localEulerAngles.z < 0)
        {
            Debug.LogWarning(this.tag + " angle less than 0");
            turretObj.transform.localEulerAngles = new Vector3(0f, 0f, 180f - rotationOffset);
        }
    }

    public void AiFire()
    {
        fire.Fire();
    }
    public void HandleFiring()
    {
        if (Game.GetCurrentPlayer() == this)
        {
            
            Debug.DrawRay(bulletSpawner.transform.position, bulletSpawner.transform.forward * (Power * powerMultiplier) / 2, Color.red);
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                fire.Fire();
            }

            if (Input.GetAxis("Vertical") < 0)
            {
                Power = fire.DecreasePower(Power);
            }

            if (Input.GetAxis("Vertical") > 0)
            {
                Power = fire.IncreasePower(Power);
            }
        }
    }

    public void HandleAiming()
    {
        if (Game.GetCurrentPlayer() == this)
        {
            if (Input.GetAxis("Horizontal") < 0)
            {
                //Bounds
                ConstrainRotations();

                turret.Raise();
                if(gameObject.tag == "Player2")//red tank
                    Angle = Mathf.Abs((int)turretObj.transform.eulerAngles.z - rotationOffset - 180);
                else//blue
                    Angle = Mathf.Abs((int)turretObj.transform.eulerAngles.z - rotationOffset);

            }

            if (Input.GetAxis("Horizontal") > 0)
            {
                ConstrainRotations();

                turret.Lower();
                if (gameObject.tag == "Player2")
                    Angle = Mathf.Abs((int)turretObj.transform.eulerAngles.z - rotationOffset - 180);
                else
                    Angle = Mathf.Abs((int)turretObj.transform.eulerAngles.z - rotationOffset);

            }
        }
        
    }
    

    public void SetPowerText(string text)
    {
        powerText.text = "Power: " + text;
    }
    public void SetAngleText(string text)
    {
        angleText.text = "Angle: " + text;
    }
    public void SetDistanceText(string text)
    {
        distanceText.text = "Distance: " + text;
    }

}
