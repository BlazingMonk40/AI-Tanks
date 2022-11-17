using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;


public class Neuron
{
    public String id;
    public float actThreshold;
    private float[] weights;

    private Boolean active;
    public float lastRecievedValue;

    public Neuron(int x, int y, int nextLayerNumberOfNeurons, ActivationFunctions af)
    {
        weights = new float[nextLayerNumberOfNeurons];
        id = "[col = " + (x + 1) + ", row = " + (y + 1) + "]";
        active = x == 0 ? true : false;
        for (int i = 0; i < nextLayerNumberOfNeurons; i++)
        {
            weights[i] = UnityEngine.Random.Range(-1f, 1f);
        }
        actThreshold = af.calculate(UnityEngine.Random.Range(-1f, 1f));
        lastRecievedValue = 0.0f;

    }

    public Boolean activate()
    {
        this.active = lastRecievedValue >= actThreshold ? true : false;
        return active;
    }

    public void setActive(Boolean activated)
    {
        this.active = activated;
    }
    public void setLastReceivedValue(float LastRecivedValue)
    {
        this.lastRecievedValue = LastRecivedValue;
    }
    public float getLastRecievedValue()
    {
        return lastRecievedValue;
    }

    public bool getActive()
    {
        return active;
    }

    protected void setId(String id)
    {
        this.id = id;
    }

    public float getActThres()
    {
        return actThreshold;
    }

    public void setActThres(float actThreshold)
    {
        this.actThreshold = actThreshold;
    }

    public float[] getWeights()
    {
        return weights;
    }
    public void setWeights(float[] weights)
    {
        this.weights = weights;
    }

    public String toString()
    {
        return id;
    }

}