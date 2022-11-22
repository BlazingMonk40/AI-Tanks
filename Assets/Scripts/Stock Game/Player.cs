using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    private GameObject playerUIContainer;
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

            if (Net != null)
            {
                Net.AddFitness(-Mathf.Abs(value));
                //Debug.Log(gameObject.name + " Fitness: " + Net.GetFitness());
            }
            if (value == 0)
            {
                if(Net != null)
                    Net.Hits++;
                SetDistanceText("Hit!");
            }
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

    public void InitAI(AIManager.AIType type)
    {
        PlayerAI = new AIManager(type);
        PlayerAI.Game = Game;
        PlayerAI.Player = this;
    }

    public void CallFeedForward(float[] input)
    {
        output = Net.FeedForward(input);
        Power = output[0] * 100;
        Angle = output[1] * 90;
        AiAim();
        AiFire();
    }

    

    internal void InitPlayer(Game game, GameObject container)
    {
        #region Text Info
        playerUIContainer = container;
        powerText = playerUIContainer.transform.GetChild(1).gameObject.GetComponent<Text>();
        angleText = playerUIContainer.transform.GetChild(2).gameObject.GetComponent<Text>();
        distanceText = playerUIContainer.transform.GetChild(3).gameObject.GetComponent<Text>();
        healthText = playerUIContainer.transform.GetChild(4).gameObject.GetComponent<Text>();
        #endregion
        if (GameManager.instance.trainingMode)
        {
            if (gameObject.tag == "Player1")
            {
                transform.localPosition = GameManager.instance.player1SpawnPosition;
            }
            else if (gameObject.tag == "Player2")
            {
                transform.localPosition = GameManager.instance.player2SpawnPosition;
            }
        }
        else
        {
            if (gameObject.tag == "Player1")
            {
                transform.localPosition = new Vector3(UnityEngine.Random.Range(-120f, -50), -15f, -25f);
                transform.localEulerAngles = new Vector3(0f, 90f, 0f);
            }
            else if (gameObject.tag == "Player2")
            {
                transform.localPosition = new Vector3(UnityEngine.Random.Range(50f, 120f), -30f, -25f);
                transform.localEulerAngles = new Vector3(0f, 270f, 0f);
            }
        }
        Game = game;
        fire = GetComponentInChildren<TankFire>();
        fire.Player = this;
        fire.Game = this.Game;
        fire.bulletContainer = Game.bulletContainer;
        turret = GetComponentInChildren<TurretController>();
        turret.Player = this;
        turret.Game = this.Game;
        turretObj = turret.gameObject;
        bulletSpawner = fire.gameObject;
        Power = -1f;
        Angle = -1f;
        Distance = -1f;
        Health = 3;

        if (GameManager.instance.playStyle[3])
        {
            InitNeuralNetwork();
        }
    }

    void InitNeuralNetwork()
    {
        NeuralNetworkFeedForward net = new NeuralNetworkFeedForward(GameManager.instance.layers);
        Net = net;
        
        if (this == Game.playerList[0]) //If this is player 1
        {
            Game.player1Net = LoadNetwork(GameManager.instance.mutatePlayer1Number, "Assets/Scripts/AI/Clay FeedForward/BestNets/Player1/");
            GameManager.instance.mutatePlayer1Number++;
        }
        else                            //If this is player 2
        {
            Game.player2Net = LoadNetwork(GameManager.instance.mutatePlayer2Number, "Assets/Scripts/AI/Clay FeedForward/BestNets/Player2/");
            GameManager.instance.mutatePlayer2Number++;
        }

        NeuralNetworkFeedForward LoadNetwork(int mutateNumber, string path)
        {
            int i = mutateNumber / (GameManager.instance.numberGames / GameManager.instance.numberOfParents);

            if (!File.Exists(path + "parent_" + i + ".txt"))
            {
                Net.InitStuff();
            }
            else
            {
                Debug.Log(Game.name + " " + this.name);
                Net.Load(path + "parent_" + i + ".txt");
            }
            
            if (mutateNumber % (GameManager.instance.numberGames / GameManager.instance.numberOfParents) != 0)
            {
                Net.Mutate();
            }
            return Net;
        }
    }

    private void Update()
    {
        FixAxis();
        
        if (GameManager.instance.playStyle[0]) //True for Manual playstyle
        {
            HandleFiring();
            HandleAiming();
        }
        if(GameManager.instance.playStyle[1]) //True for Reinforced Learning
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
        if(!GameManager.instance.trainingMode)
            if (Health <= 0)
                Game.GameOver();
    }

    private void FixAxis()
    {
        if (gameObject.tag == "Player1")
        {
            if(transform.localEulerAngles != new Vector3(0f, 90f, 0f))
                transform.localEulerAngles = new Vector3(0f, 90f, 0f);
        }
        else if (gameObject.tag == "Player2")
        {
            if (transform.localEulerAngles != new Vector3(0f, 270f, 0f))
                transform.localEulerAngles = new Vector3(0f, 270f, 0f);
        }
    }

    public void AiAim()
    {
        if (turretObj == null) return;
        turretObj.transform.localEulerAngles = new Vector3(Angle, 0f, 0f);
        Angle = turretObj.transform.localEulerAngles.x;
        //ConstrainRotations();
    }
    public void AiFire()
    {

        if (fire == null) return;
        fire.Fire();
    }
    private void ConstrainRotations()
    {
        //This is busted 10:23_11/16/22
        if (turretObj.transform.localEulerAngles.x < 5f)
        {
            Debug.LogError(turretObj.transform.localEulerAngles.x);
            turretObj.transform.localEulerAngles = new Vector3(5f, 0f, 0f);
        }
        else if (turretObj.transform.localEulerAngles.x > 280f)
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
                //ConstrainRotations();

                turret.Raise();
                if(gameObject.tag == "Player2")//red tank
                    Angle = Mathf.Abs((int)turretObj.transform.eulerAngles.z - rotationOffset - 180);
                else//blue
                    Angle = Mathf.Abs((int)turretObj.transform.eulerAngles.z - rotationOffset);

            }

            if (Input.GetAxis("Horizontal") > 0)
            {
                //ConstrainRotations();

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
