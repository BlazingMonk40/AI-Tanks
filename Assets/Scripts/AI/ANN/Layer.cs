using System.Collections.Generic;
using System;
using System.IO;

public class Layer
{
    string name;
    int id, nextNumOfNeurons, numOfNeurons;
    float bias;


    public List<Neuron> NL;
    public ActivationFunctions function;

    public Layer(int id, int numOfNeurons, ActivationFunctions function)
    {
        this.id = id;
        this.name = "Layer " + (id + 1);
        this.bias = UnityEngine.Random.Range(-2.0f, 2f);
        this.numOfNeurons = numOfNeurons;
        this.function = function;
        this.nextNumOfNeurons = 0;
    }
    public float getBias()
    {
        return bias;
    }

    public void setNext(int nextLayerNeuronNumber)
    {
        this.nextNumOfNeurons = nextLayerNeuronNumber;
    }
    public int getNumberOfNeurons()
    {
        return numOfNeurons;
    }

    public void setBias(float bias)
    {
        this.bias = bias;
    }
    /*
    * return summed weight of all neurons of weight associated with next row neurons
    */
    public float getSummedWeight(int row)
    {
        float weightSum = 0.0f;
        foreach (Neuron n in NL)
        {
            weightSum += n.getWeights()[row];
        }
        return weightSum += bias;
    }

    public void init()
    {
        NL = new List<Neuron>();
        for (int i = 0; i < numOfNeurons; i++)
        {
            NL.Add(new Neuron(id, i, nextNumOfNeurons, function));
        }
    }

    public string toString()
    {
        return name;
    }

    public void mutate()
    {
        bool lockfirst = true;
        foreach (Neuron n in NL)
        {
            float[] temp = n.getWeights();
            if (lockfirst == false)
            {
                n.setActThres(function.calculate(n.getActThres() * UnityEngine.Random.Range(0.0f, 1.25f)));
                n.setActive(false);
            }
            for (int i = 0; i < temp.Length - 1; i++)
            {
                float tempF = UnityEngine.Random.Range(-1f, 1f);
                if (tempF == 1f)
                {
                    tempF = UnityEngine.Random.Range(0.0f, 1f);
                }
                else if (tempF == -1f)
                {
                    tempF = UnityEngine.Random.Range(-1f, 0.0f);
                }
                temp[i] = tempF;
            }
            n.setWeights(temp);
            lockfirst = false;
        }
    }
}