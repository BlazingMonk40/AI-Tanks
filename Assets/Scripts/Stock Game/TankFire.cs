using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankFire : MonoBehaviour
{
    public GameObject tankFireSFX;
    public GameObject bulletPrefab;
    public float power;
    public GameObject bulletContainer;
    private GameObject fxContainer;
    
    
    public string teamTag;
    private Player player;
    private Game game;

    public Player Player { get => player; set => player = value; }
    public Game Game { get => game; set => game = value; }

    private void Start()
    {
        fxContainer = bulletContainer.transform.GetChild(0).gameObject;
    }

    /// <summary>
    /// Creates a new bullet object and applies a velocity to it
    /// </summary>
    public void Fire()
    {

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, bulletContainer.transform);
        bullet.tag = gameObject.tag;
        bullet.GetComponent<Bullet>().fxContainer = fxContainer;
        bullet.GetComponent<Bullet>().Player = this.Player;
        bullet.GetComponent<Bullet>().Game = this.Game;
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * Player.Power * Player.powerMultiplier;

        if (Player == Game.playerList[0])
            bullet.GetComponent<TrailRenderer>().colorGradient = Game.player1Gradient;
        else
            bullet.GetComponent<TrailRenderer>().colorGradient = Game.player2Gradient;
        if (!GameManager.instance.trainingMode)
        {
            GameObject fireSoundObj = Instantiate(tankFireSFX, transform.position, Quaternion.identity, fxContainer.transform);
            fireSoundObj.GetComponent<AudioSource>().Play();
            Destroy(fireSoundObj, 3f);
        }
    }

    private IEnumerator HangFire()
    {
        yield return new WaitForSeconds(.1f);

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

