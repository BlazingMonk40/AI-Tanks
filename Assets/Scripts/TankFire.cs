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
    public string teamTag;
    private Player player;
    private Game game;

    public Player Player { get => player; set => player = value; }
    public Game Game { get => game; set => game = value; }

    void Start()
    {
        if (gameObject.transform.parent.parent.parent.tag == "Player1")
        {
            teamTag = "Player1";
            Player = Game.playerList[0];
        }
        else if (gameObject.transform.parent.parent.parent.tag == "Player2")
        {
            teamTag = "Player2";
            Player = Game.playerList[1];
        }
    }

    /// <summary>
    /// Creates a new bullet object and applies a velocity to it
    /// </summary>
    public void Fire()
    {
        StartCoroutine(HangFire());

    }

    private IEnumerator HangFire()
    {
        yield return new WaitForEndOfFrame();
        //Debug.Log(gameObject.tag + " " + Player.Power + " Fire!");
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, bulletContainer.transform);
        bullet.tag = gameObject.tag;
        bullet.GetComponent<Bullet>().Player = this.Player;
        bullet.GetComponent<Bullet>().Game = this.Game;
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * Player.Power * Player.powerMultiplier;

        GameObject fireSoundObj = Instantiate(tankFireSFX, transform);
        fireSoundObj.GetComponent<AudioSource>().Play();
        Destroy(fireSoundObj, 3f);
    }

    public float IncreasePower(float power)
    {
        return power += 10 * Time.deltaTime;
    }

    public float DecreasePower(float power)
    {
        return power += 10 * Time.deltaTime;
    }

}

