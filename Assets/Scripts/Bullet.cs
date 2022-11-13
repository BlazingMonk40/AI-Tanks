using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject explosionSFX;
    public GameObject explosionVisual;
    private Player player;
    private Game game;
    private float gravity = -9.8f;

    public Player Player { get => player; set => player = value; }
    public Game Game { get => game; set => game = value; }

    private void Start()
    {

        GetComponent<SphereCollider>().enabled = false;
        StartCoroutine(TurnColliderOn());
    }
    private void Update()
    {
        ApplyGravity();
    }


    private void OnTriggerEnter(Collider other)
    {
        DisableColliderTriggers();
        CheckFriendlyFire(other);
        MakeImpactFX();
        Player.Distance = CalculateDistance(gameObject.transform.position, Game.notCurrentPlayer.transform.position,  other);
        Game.EndTurn();

        //Destroy this bullet 3 seconds after impact
        Destroy(gameObject, .5f);
    }

    private void MakeImpactFX()
    {
        //Play Sound FX
        GameObject soundFX = Instantiate(explosionSFX, transform.position, Quaternion.identity);
        soundFX.GetComponent<AudioSource>().Play();
        Destroy(soundFX, 3f);
        //Play the explosion particle effect at impact position and clean up after 2 seconds
        GameObject explosionParticles = Instantiate(explosionVisual, transform.position, Quaternion.identity);
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
/*
 * 
 * 
 * male(clay).
 * male(ken).
 * male(arnold).
 *
 * female(anna).
 * female(michelle).
 * female(sandra).
 *
 * sibling(clay, anna).
 * 
 * parent(michelle, clay).
 * parent(ken, clay).
 * 
 * parent(sandra, ken).
 * parent(arnold, ken).
 * 
 * relationship(X,Y) :- parent(X,Y), female(X), write(X), write(' is the mother of '), write(Y), nl.
 * 
 * relationship(X,Y) :- parent(X,Z), parent(Z,Y), female(X), write(X is the grandmother of Y).
 */