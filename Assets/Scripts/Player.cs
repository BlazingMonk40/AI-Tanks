using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private float rotationOffset = 90f;
    public float powerMultiplier = .75f;
    private float power;
    private float angle;
    private float distance;
    private bool myTurn;
    AI playerAI;

    public float Power 
    { 
        get => power;
        set
        {
            power = value;
            SetPowerText(value.ToString());
        }
    }
    public float Angle 
    { 
        get => angle;
        set
        {
            angle = value;
            SetAngleText(value.ToString());
        }
    }
    public float Distance 
    { 
        get => distance;
        set 
        { 
            distance = value;
            SetDistanceText(value.ToString());
        }
    }
    public AI PlayerAI 
    { 
        get => playerAI; 
        set => playerAI = value; 
    }

    /*public IEnumerator SendInfo(string tag)
    {
        yield return new WaitForEndOfFrame();
        if (tag == "Player1")
        {
            //GameManager.instance.
        }
        else if (tag == "Player2")
        {

        }
    }*/

    private TankFire fire;
    private TurretController turret;
    private GameObject turretObj;
    GameObject bulletSpawner; 
    public GameObject bulletPrefab;
    public GameObject bulletContainer;

    public GameObject playerContainer;
    private Text powerText;
    private Text angleText;
    private Text distanceText;

    private Quaternion stockRotation;

    public void InitAI(AI.AIType type)
    {
        PlayerAI = new AI(type);
    }

    void Start()
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

        fire = GetComponentInChildren<TankFire>();
        turret = GetComponentInChildren<TurretController>();
        turretObj = transform.GetChild(0).gameObject;
        bulletSpawner = GameObject.FindGameObjectWithTag(gameObject.tag + "BS");
        Power = 50f;
        Angle = 0f;
        Distance = 0f;

        stockRotation = turretObj.transform.rotation;

    }

    private void Update()
    {
        FixAxis();
        //HandleFiring();
        //HandleAiming();
    }

    private void FixAxis()
    {
        if (transform.eulerAngles.x != 0f)
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
        if (transform.position.z != -320f)
            transform.position = new Vector3(transform.position.x, transform.position.y, -320f);
    }

    public void AiAim(float dir)
    {
        if (gameObject.tag == "Player2")
        {
            Debug.Log("Player2 Angle: " + dir);
            turretObj.transform.eulerAngles = new Vector3(0f, 180f, 180f - dir);
            //Debug.Log("\nPlayer 2 AI Aim: param = " + dir + " turret euler = " + turretObj.transform.eulerAngles);
            Angle = turretObj.transform.eulerAngles.z - 90f;
        }
        else
        {
            Debug.Log("Player1 Angle: " + dir);
            turretObj.transform.eulerAngles = new Vector3(0f, 0f, dir + 90);
            //Debug.Log("\nPlayer 1 AI Aim: param = " + dir + " turret euler = " + turretObj.transform.eulerAngles);
            Angle = turretObj.transform.eulerAngles.z - 90f;
        }


        if (turretObj.transform.eulerAngles.z > 180f)
        {
            Debug.LogWarning(this.tag + " angle greater than 180f");
            turretObj.transform.eulerAngles = new Vector3(0f, 0f, 0f - rotationOffset);
        }
        if (turretObj.transform.eulerAngles.z < 0)
        {
            Debug.LogWarning(this.tag + " angle less than 0");
            turretObj.transform.eulerAngles = new Vector3(0f, 0f, 180f - rotationOffset);
        }
    }
    public void AiFire()
    {
        Fire();
    }
    public void HandleFiring()
    {
        if (GameManager.instance.GetCurrentPlayer() == this)
        {
            
            Debug.DrawRay(bulletSpawner.transform.position, bulletSpawner.transform.forward * (Power * powerMultiplier) / 2, Color.red);
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                Fire();
            }

            if (Input.GetAxis("Vertical") < 0)
            {
                Power = fire.DecreasePower(Power);
                Power = (int)power;
            }

            if (Input.GetAxis("Vertical") > 0)
            {
                Power = fire.IncreasePower(Power);
                Power = (int)power;
            }
        }
    }

    public void HandleAiming()
    {
        if (GameManager.instance.GetCurrentPlayer() == this)
        {
            if (Input.GetAxis("Horizontal") < 0)
            {
                //Bounds
                if (turretObj.transform.eulerAngles.z - rotationOffset > 180f)
                    turretObj.transform.eulerAngles = new Vector3(0f, 0f, 0f - rotationOffset);
                if (turretObj.transform.eulerAngles.z - rotationOffset < 0)
                    turretObj.transform.eulerAngles = new Vector3(0f, 0f, 180f - rotationOffset);

                turret.Raise();
                if(gameObject.tag == "Player2")//red tank
                    Angle = Mathf.Abs((int)turretObj.transform.eulerAngles.z - rotationOffset - 180);
                else//blue
                    Angle = Mathf.Abs((int)turretObj.transform.eulerAngles.z - rotationOffset);

            }

            if (Input.GetAxis("Horizontal") > 0)
            {
                if (turretObj.transform.eulerAngles.z - rotationOffset > 180f)
                    turretObj.transform.eulerAngles = new Vector3(0f, 0f, 0f - rotationOffset);
                if (turretObj.transform.eulerAngles.z - rotationOffset < 0)
                    turretObj.transform.eulerAngles = new Vector3(0f, 0f, 180f - rotationOffset);

                turret.Lower();
                if (gameObject.tag == "Player2")
                    Angle = Mathf.Abs((int)turretObj.transform.eulerAngles.z - rotationOffset - 180);
                else
                    Angle = Mathf.Abs((int)turretObj.transform.eulerAngles.z - rotationOffset);

            }
        }
        
    }
    public void Fire()
    {
        
        Debug.Log(gameObject.tag+ " " + Power + " Fire!");
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawner.transform.position, Quaternion.identity, bulletContainer.transform);
        bullet.tag = gameObject.tag;
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawner.transform.forward * Power * powerMultiplier;

        GameManager.instance.EndTurn();
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
