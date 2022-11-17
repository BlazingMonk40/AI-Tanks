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
    public NeuralNetFF(int numOfinputs, int numOfOutputs, int numberOfNeurons, int numberOfHiddenLayers, ActivationFunctions.Functions function)
    {
        this.function = new ActivationFunctions(function);
        this.numOfInputs = numOfinputs;
        this.numOfOutputs = numOfOutputs;
        this.numberOfHiddenLayers = numberOfHiddenLayers;
        this.fitness = 0;
        this.numberOfNeurons = numberOfNeurons;
        init();
    }

    public int getFitness()
    {
        return fitness;
    }
    public void setFitness(int fitness)
    {
        this.fitness = fitness;
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
            temp[i] = lastLayer.NL[i].getLastRecievedValue();
        }
        return temp;
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
    // public void Load(string path)//this loads the biases and weights from within a file into the neural network.
    // {
    //     TextReader tr = new StreamReader(path);
    //     int NumberOfLines = (int)new FileInfo(path).Length;
    //     string[] ListLines = new string[NumberOfLines];
    //     int index = 1;
    //     for (int i = 1; i < NumberOfLines; i++)
    //     {
    //         ListLines[i] = tr.ReadLine();
    //     }
    //     tr.Close();
    //     if (new FileInfo(path).Length > 0)
    //     {
    //         for (int i = 0; i < network.Length; i++)
    //         {
    //                 network[i].setBias(float.Parse(ListLines[index]));
    //                 index++;
    //         }

    //         for (int i = 0; i < weights.Length; i++)
    //         {
    //             for (int j = 0; j < weights[i].Length; j++)
    //             {
    //                 for (int k = 0; k < weights[i][j].Length; k++)
    //                 {
    //                     weights[i][j][k] = float.Parse(ListLines[index]); ;
    //                     index++;
    //                 }
    //             }
    //         }
    //     }
    // }
    public void Save(string path)//this is used for saving the biases and weights within the network to a file.
    {
        File.Create(path).Close();
        StreamWriter writer = new StreamWriter(path, true);
        foreach (Layer x in network)
        {
            writer.WriteLine(x.getBias());
        }
        foreach (Layer x in network)
        {
            foreach (Neuron y in x.NL)
            {
                writer.WriteLine(y.toString());
                foreach (float z in y.getWeights())
                {

                    writer.WriteLine(z);
                }
            }
        }
        writer.Close();
    }
}