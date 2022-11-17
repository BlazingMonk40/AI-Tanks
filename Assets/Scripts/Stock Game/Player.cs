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
    private float power = -1;
    private float angle = -1;
    private float distance = -1;
    private bool myTurn;
    AIManager playerAI;

    private TankFire fire;
    private TurretController turret;
    private GameObject turretObj;
    private GameObject bulletSpawner; 
    public GameObject bulletPrefab;
    public GameObject bulletContainer;

    public GameObject playerUIContainer;
    private Text powerText;
    private Text angleText;
    private Text distanceText;
    private Game game;

    private NeuralNetworkFeedForward net;
    private float[] input;
    private float[] output;

    private int health = 3;
    private Text healthText;

    #region Properties
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
            angle = 360 -value;
            SetAngleText((360 - (int)value).ToString());
        }
    }
    public float Distance
    {
        get => distance;
        set
        {
            distance = value;
            if(value == 0)
                SetDistanceText("Hit!");
            else
                SetDistanceText(((int)value).ToString());
        }
    }
    public AIManager PlayerAI
    {
        get => playerAI;
        set => playerAI = value;
    }
    public Game Game 
    { 
        get => game; 
        set => game = value; 
    }
    public int Health 
    { 
        get => health;
        set
        {
            health = value;
            SetHealthText();
        }
    }
     public NeuralNetworkFeedForward Net 
    { 
        get => net; 
        set => net = value; 
    }


    #endregion

    public void Init(NeuralNetworkFeedForward net)
    {
        Net = net;
    }

    public void InitAI(AIManager.AIType type)
    {
        PlayerAI = new AIManager(type);
        PlayerAI.distanceBetweenPlayers = Game.DistanceBetweenPlayers;
    }

    public void CallFeedForward(float[] input)
    {
        output = Net.FeedForward(input);
        Power = output[0];
        Angle = output[1];
        AiAim();
        AiFire();
    }

    

    internal void InitPlayer(Game game)
    {
        #region Text Info
        powerText = playerUIContainer.transform.GetChild(1).gameObject.GetComponent<Text>();
        angleText = playerUIContainer.transform.GetChild(2).gameObject.GetComponent<Text>();
        distanceText = playerUIContainer.transform.GetChild(3).gameObject.GetComponent<Text>();
        healthText = playerUIContainer.transform.GetChild(4).gameObject.GetComponent<Text>();
        #endregion
        
        if (gameObject.tag == "Player1")
        {
            transform.localPosition = new Vector3(UnityEngine.Random.Range(-120f, -50), -20f, -25f);
        }
        else if (gameObject.tag == "Player2")
        {
            transform.localPosition = new Vector3(UnityEngine.Random.Range(50f, 120f), -35f, -25f);
        }
        Game = game;
        fire = GetComponentInChildren<TankFire>();
        fire.Player = this;
        fire.Game = this.Game;
        turret = GetComponentInChildren<TurretController>();
        turret.Player = this;
        turret.Game = this.Game;
        turretObj = turret.gameObject;
        bulletSpawner = fire.gameObject;
        Power = -1f;
        Angle = -1f;
        Distance = -1f;
        Health = 3;
    }

    private void Update()
    {
        //FixAxis();
        
        if (Game.playStyle[0]) //True for Manual playstyle
        {
            HandleFiring();
            HandleAiming();
        }
        if(Game.playStyle[1]) //True for Reinforced Learning
        {
            if(this == Game.playerList[0])
                Debug.DrawRay(bulletSpawner.transform.position, bulletSpawner.transform.forward * (Power * powerMultiplier) / 2, Color.blue);
            else
                Debug.DrawRay(bulletSpawner.transform.position, bulletSpawner.transform.forward * (Power * powerMultiplier) / 2, Color.red);
        }
    }

    public void Hit()
    {
        Health -= 1;
        if (Health <= 0)
            Game.GameOver();
    }

    private void FixAxis()
    {
        if (transform.localEulerAngles.x != 0f)
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
        if (transform.position.z != -320f)
            transform.position = new Vector3(transform.position.x, transform.position.y, -15f);
    }

    public void AiAim()
    {
        turretObj.transform.localEulerAngles = new Vector3(Angle, 0f, 0f);
        Angle = turretObj.transform.localEulerAngles.x;
        ConstrainRotations();
    }
    public void AiFire()
    {
        fire.Fire();
    }
    private IEnumerator ConstrainRotations()
    {
        yield return new WaitForEndOfFrame();
        //Constrain turret rotations
        if (turretObj.transform.localEulerAngles.x >= 5f)
        {
            Debug.LogError(turretObj.transform.localEulerAngles.x);
            //Debug.LogWarning(this.tag + " angle greater than 5f");
            turretObj.transform.localEulerAngles = new Vector3(5f, 0f, 0f);
        }
        else if (turretObj.transform.localEulerAngles.x <= 280f)
        {
            Debug.LogError(turretObj.transform.localEulerAngles.x);
            //Debug.LogWarning(this.tag + " angle less than -80f");
            turretObj.transform.localEulerAngles = new Vector3(280f, 0f, 0f);
        }
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
                StartCoroutine(ConstrainRotations());

                turret.Raise();
                if(gameObject.tag == "Player2")//red tank
                    Angle = Mathf.Abs((int)turretObj.transform.eulerAngles.z - rotationOffset - 180);
                else//blue
                    Angle = Mathf.Abs((int)turretObj.transform.eulerAngles.z - rotationOffset);

            }

            if (Input.GetAxis("Horizontal") > 0)
            {
                StartCoroutine(ConstrainRotations());

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
    private void SetHealthText()
    {
        healthText.text = "Health: " + Health;
    }

}
