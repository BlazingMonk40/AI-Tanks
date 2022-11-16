using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    public List<AudioSource> cannon_Fire;
    public List<AudioSource> engine;
    public List<AudioSource> flyby;
    public List<AudioSource> hit;
    public List<AudioSource> reload;
    public List<AudioSource> turret;
    public List<AudioSource> warning;

    public AudioSource Cannon_Fire 
    {
        get
        {
            return cannon_Fire[Random.Range(0, cannon_Fire.Count)];
        }
    }
    public AudioSource Engine 
    {
        get
        {
            return engine[Random.Range(0, engine.Count)];
        }
    }
    public AudioSource Flyby 
    {
        get
        {
            return flyby[Random.Range(0, flyby.Count)];
        }
    }
    public AudioSource Hit 
    {
        get
        {
            return hit[Random.Range(0, hit.Count)];
        }
    }
    public AudioSource Reload 
    {
        get
        {
            return reload[Random.Range(0, reload.Count)];
        }
    }
    public AudioSource Turret 
    {
        get
        {
            return turret[Random.Range(0, turret.Count)];
        }
    }
    public AudioSource Warning 
    {
        get 
        { 
           return warning[Random.Range(0, warning.Count)]; 
        }
    }
}
