using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class NeuralNetFF : IComparable<NeuralNetFF>
{
    ActivationFunctions function;
    private int numOfInputs, numOfOutputs, fitness;
    public int numberOfHiddenLayers;
    public List<Layer> network;
    int numberOfNeurons = 0;
    public float error =0;
    public NeuralNetFF(int numOfinputs, int numOfOutputs, int numberOfNeurons, int numberOfHiddenLayers, ActivationFunctions.Functions function)
    {
        this.function = new ActivationFunctions(function);
        this.numOfInputs = numOfinputs;
        this.numOfOutputs = numOfOutputs;
        this.numberOfHiddenLayers = numberOfHiddenLayers;
        this.fitness = 100;
        this.numberOfNeurons = numberOfNeurons;
        init();
    }

    public int getFitness()
    {
        return fitness;
    }
    public void setFitness(int fitness)
    {
        this.fitness = fitness < 0 ? 0: fitness>100 ? 100:fitness ;
    }
    public void feedForward()
    {
        for (int i = 0; i < network.Count - 1; i++)
        {
            for (int j = 0; j < network[i + 1].getNumberOfNeurons(); j++)
            {
                network[i + 1].NL[j].setLastReceivedValue(network[i + 1].function.calculate(network[i].getSummedWeight(j)));
                network[i + 1].NL[j].activate();
            }
        }
    }
    public void backPropergate()
    {
        for (int i = 0; i < network.Count - 1; i--)
        {
            for (int j = 0; j < network[i].getNumberOfNeurons(); j++)
            {
                network[i + 1].NL[j].setLastReceivedValue(network[i + 1].function.calculate(network[i].getSummedWeight(j)));
                network[i + 1].NL[j].activate();
            }
        }
    }
    public bool[] getOutput()
    {
        bool[] temp = new bool[numOfOutputs];
        Layer lastLayer = network[numberOfHiddenLayers + 1];
        for (int i = 0; i < lastLayer.NL.Count; i++)
        {
            temp[i] = lastLayer.NL[i].getActive();
        }
        return temp;
    }

    public float[] getRealOutput()
    {
        float[] temp = new float[numOfOutputs];
        Layer lastLayer = network[numberOfHiddenLayers + 1];
        for (int i = 0; i < lastLayer.NL.Count; i++)
        {
            temp[i] = lastLayer.function.calculate(lastLayer.NL[i].getLastRecievedValue());
        }
        return temp;
    }
    public void setRealInput(float[] input)
    {
        if (input.Length != numOfInputs)
            return;

        for (int i = 0; i < network[0].NL.Count; i++)//get neuron
        {
            float[] changedWeights = network[0].NL[i].getWeights();
            for (int j = 0; j < changedWeights.Length; j++)//change weights
            {

                changedWeights[j] = network[0].function.calculate(input[i]);
            }
            network[0].NL[i].setWeights(changedWeights);
        }
    }
    public void mutate()
    {
        foreach (Layer x in network)
        {
            x.mutate();
        }
    }

    public int CompareTo(NeuralNetFF other)
    {
        if (other == null) return 1;
        if (fitness > other.getFitness())
            return 1;
        else if (fitness < other.getFitness())
            return -1;
        else
            return 0;
    }
    private void init()
    {
        network = new List<Layer>();
        for (int i = 0; i < numberOfHiddenLayers + 2; i++)
        {
            if (i == 0)
            {
                Layer inputlayer = new Layer(i, numOfInputs, function);
                inputlayer.setNext(numberOfNeurons);
                inputlayer.init();
                network.Add(inputlayer);
            }
            else if (i == numberOfHiddenLayers)
            {
                Layer temp = new Layer(i, numberOfNeurons, function);
                temp.setNext(numOfOutputs);
                temp.init();
                network.Add(temp);
            }
            else if (i == numberOfHiddenLayers + 1)
            {
                Layer outputlayer = new Layer(i, numOfOutputs, function);
                outputlayer.setNext(1);
                outputlayer.init();
                network.Add(outputlayer);
            }
            else
            {
                Layer hiddenlayer = new Layer(i, numberOfNeurons, function);
                hiddenlayer.setNext(numberOfNeurons);
                hiddenlayer.init();
                network.Add(hiddenlayer);
            }
        }
    }

}