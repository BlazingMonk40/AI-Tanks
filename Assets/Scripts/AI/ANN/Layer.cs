using System.Collections.Generic;
using System;
using System.IO;

public class Layer
{
    string name;
    int id, nextNumOfNeurons, numOfNeurons;
    float bias;


    public List<Neuron> NL;
    ActivationFunctions function;

    public Layer(int id, int numOfNeurons, ActivationFunctions function)
    {
        this.id = id;
        this.name = "Layer " + (id + 1);
        this.bias = function.calculate(UnityEngine.Random.Range(0.0f, 1f));
        this.numOfNeurons = numOfNeurons;
        this.function = function;
    }
    public float getBias()
    {
        return bias;
    }

    public void setNext(int nextLayerNeuronNumber)
    {
        this.nextNumOfNeurons = nextLayerNeuronNumber;
    }
    public int getNumberOfNeurons(){
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
        float temp = 0.0f;
        foreach (Neuron n in NL)
        {
            temp += n.getWeights()[row];
        }
        return temp += bias;
    }

    public void init()
    {
        for (int i = 0; i < numOfNeurons; i++)
        {
            NL = new List<Neuron>();
            NL.Add(new Neuron(id, i, nextNumOfNeurons, function));
        }
    }
    public string toString(){
        return name;
    }

    public void mutate()
    {
        foreach (Neuron n in NL)
        {
            float[] temp = n.getWeights();
            n.setActThres(function.calculate(n.getActThres() * UnityEngine.Random.Range(0.0f, 1.25f)));
            n.setActive(false);
            for (int i = 0; i < temp.Length - 1; i++)
            {
                float tempF = UnityEngine.Random.Range(-1f, 1f);
                if (tempF == 1f)
                { 
                    tempF = UnityEngine.Random.Range(0.0f, 1f);
                }
                else if (tempF == 0.0f)
                { 
                    tempF = UnityEngine.Random.Range(-1f, 0.0f);
                }
                temp[i] = tempF;
            }
            n.setWeights(temp);
        }
    }
}