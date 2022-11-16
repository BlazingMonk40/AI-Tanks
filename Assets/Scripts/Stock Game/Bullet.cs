using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject explosionSFX;
    public GameObject explosionVisual;
    public GameObject smokeVFX;
    public GameObject fxContainer;
    private GameObject smokeVFXContainer;
    private Player player;
    private Game game;
    private float gravity = -9.8f;
    private bool impacted = false;

    private string shotHistoryPath = "Assets/Scripts/AI/ShotHistory.txt";

    public Player Player { get => player; set => player = value; }
    public Game Game { get => game; set => game = value; }

    private void Start()
    {
        smokeVFXContainer = GameObject.Find("Impact Smoke Container");
        GetComponent<SphereCollider>().enabled = false;
        StartCoroutine(TurnColliderOn());
    }
    private void Update()
    {
        ApplyGravity();
        ApplyWind();
    }

    private void ApplyWind()
    {
        if (Game.WindSpeed < 0)
            GetComponent<Rigidbody>().velocity += Vector3.right * Game.WindSpeed/2 * Time.deltaTime;
        else
            GetComponent<Rigidbody>().velocity -= Vector3.left * Game.WindSpeed/2 * Time.deltaTime;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!impacted)
        {
            impacted = true;
            
            DisableColliderTriggers();
            CheckFriendlyFire(other);
            MakeImpactFX();
            Player.Distance = CalculateDistance(gameObject.transform.position, Game.notCurrentPlayer.transform.position, other);
            if (Player.Distance == 0)
            {
                MakeImpactSmoke();
                WriteShotHistory(shotHistoryPath);
            }

            Game.EndTurn();

            //Destroy this bullet 3 seconds after impact
            Destroy(gameObject, .1f);
        }
    }

    public void WriteShotHistory(string path)
    {
        if (File.Exists(shotHistoryPath))
        {
            StreamWriter writer = new StreamWriter(path, true);
            //Total Distance, ^X, ^Y, Wind, Angle, Power

            writer.WriteLine(Game.DistanceBetweenPlayers.ToString().PadRight(10) + " | " + (Game.notCurrentPlayer.transform.position.y - Game.currentPlayer.transform.position.y).ToString().PadLeft(1).PadRight(9) +
                                " | " + Game.WindSpeed.ToString().PadLeft(3).PadRight(3) + " | " + Player.Angle.ToString().PadRight(10) + " | " + Player.Power.ToString().PadRight(10));

            writer.Close();
        }
        else
            File.Create(path).Close();
    }

    private void MakeImpactSmoke()
    {
        GameObject smokeVFXObj = Instantiate(smokeVFX, transform.position, Quaternion.identity, smokeVFXContainer.transform);
        Game.ApplyWindSpeedToSmoke();
    }

    private void MakeImpactFX()
    {
        //Play Sound FX
        GameObject soundFX = Instantiate(explosionSFX, transform.position, Quaternion.identity, fxContainer.transform);
        soundFX.GetComponent<AudioSource>().Play();
        Destroy(soundFX, 3f);
        //Play the explosion particle effect at impact position and clean up after 2 seconds
        GameObject explosionParticles = Instantiate(explosionVisual, transform.position, Quaternion.identity, fxContainer.transform);
        explosionParticles.GetComponent<ParticleSystem>().Play();
        Destroy(explosionParticles, 3f);
    }
    private void ApplyGravity()
    {
        //Applys Gravity to this rigid body
        GetComponent<Rigidbody>().velocity += Vector3.up * gravity * (Time.deltaTime * 2f);
    }


    /// <summary>
    /// Check if what the bullet just collided with is it's own player
    ///     If yes:
    ///         Then we just shot ourselves so let's make the bullet do no damage
    /// </summary>
    /// <param name="other"></param>
    private void CheckFriendlyFire(Collider other)
    {
        if (other.GetComponentInParent<Player>() == Player)
            GetComponent<Rigidbody>().mass = 0;
    }
    

    /// <summary>
    /// Wait until the bullet has cleared the barrel before activated the collider
    /// </summary>
    /// <returns></returns>
    private IEnumerator TurnColliderOn()
    {
        yield return new WaitForSeconds(.1f);
        GetComponent<SphereCollider>().enabled = true;
    }


    /// <summary>
    /// Returns the horizontal (x) distance between the Vector3 parameters.
    ///
    /// </summary>
    /// <param name="bullet"></param>
    /// <param name="otherPlayer"></param>
    /// <returns></returns>
    private float CalculateDistance(Vector3 bullet, Vector3 otherPlayer, Collider other)
    {
        
        float distance;

        if (other.GetComponentInParent<Player>() != Player && other.transform.parent.tag != "Scenery")
        {
            //If the other object is not our player and not the scenery then it must be the other player so set distance to 0
            other.GetComponentInParent<Player>().Hit();
            distance = 0;
        }
        else
        {
            //Gets the bullets distance from the other player
            //toggle for each player
            if (Player == Game.playerList[0])
                distance = bullet.x - otherPlayer.x;
            else
                distance = otherPlayer.x - bullet.x;
        }
        return distance;
    }


    /// <summary>
    /// Disables the triggers on any SphereCollider attached to this object.
    /// The purpose is to avoid the shot registering twice.
    /// </summary>
    private void DisableColliderTriggers()
    {
        //Disable triggers
        SphereCollider[] colliders = GetComponents<SphereCollider>();
        foreach (SphereCollider c in colliders)
            c.isTrigger = false;
    }


}