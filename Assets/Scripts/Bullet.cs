using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject explosion;
    private Player player;
    private float gravity = -9.8f;
    private void OnTriggerEnter(Collider other)
    {
        //Play Particle effect at impact position
        GameObject boom = Instantiate(explosion, transform.position, Quaternion.identity);
        boom.GetComponent<ParticleSystem>().Play();
        
        //Get's distance from each player
        if (gameObject.tag == "Player1")
        {
            float distance;
            player = GameManager.instance.playerList[0];
            
            if (other.tag == "Player2")
                distance = 0;
            else
                distance = CalculateDistance(GameManager.instance.playerList[1].transform.position, gameObject.transform.position);
            
            player.Distance = distance;

            //StartCoroutine(player.SendInfo(gameObject.tag));
        }
        else if (gameObject.tag == "Player2")
        {
            float distance;
            player = GameManager.instance.playerList[1];
            
            if (other.tag == "Player1")
                distance = 0;
            else
                distance = CalculateDistance(gameObject.transform.position, GameManager.instance.playerList[0].transform.position);
            
            player.Distance = distance;
            //StartCoroutine(player.SendInfo(gameObject.tag));
        }
        //Disable triggers
        SphereCollider[] colliders = GetComponents<SphereCollider>();
        foreach (SphereCollider c in colliders)
            c.isTrigger = false;

        //Destroy particle system and bullet object
        Destroy(boom, 5f);
        Destroy(gameObject, 3f);
    }
    private void Start()
    {
        
        GetComponent<SphereCollider>().enabled = false;
        StartCoroutine(TurnColliderOn());
    }
    private IEnumerator TurnColliderOn()
    {
        yield return new WaitForSeconds(.1f);
        GetComponent<SphereCollider>().enabled = true;
    }

    private float CalculateDistance(Vector3 start, Vector3 last)
    {
        return (last.x - start.x);
    }
    private void Update()
    {
        GetComponent<Rigidbody>().velocity += Vector3.up * gravity * 2f * Time.deltaTime;
    }
}
