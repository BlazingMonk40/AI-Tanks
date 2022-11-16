using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    private List<AudioSource> cannon_Fire;
    private List<AudioSource> engine;
    private List<AudioSource> flyby;
    private List<AudioSource> hit;
    private List<AudioSource> reload;
    private List<AudioSource> turret;
    private List<AudioSource> warning;

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
