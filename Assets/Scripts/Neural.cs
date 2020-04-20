using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine.UI;
using System;
using System.IO;

public class Neural
{
    public Vector<float>[] nodes;
    public Matrix<float>[] weights;
    public Vector<float>[] biases;

    private int[] nodeSetup = { 312, 30, 2 };

    //Backprop;
    public Vector<float>[] gammaValues; // gradient of node to error
    public Matrix<float>[] deltaWeights; // gradient of each weight with respect to error
    public Vector<float>[] deltaBiases; // gradient of each bias with respect to error
    public float learningRate = 0.01f;

    public Neural()
    {
        nodes = new Vector<float>[nodeSetup.Length];
        weights = new Matrix<float>[nodeSetup.Length - 1];
        biases = new Vector<float>[nodeSetup.Length - 1];

        gammaValues = new Vector<float>[nodeSetup.Length - 1];
        deltaWeights = new Matrix<float>[nodeSetup.Length - 1];
        deltaBiases = new Vector<float>[nodeSetup.Length - 1];

        for (int i = 0; i < nodeSetup.Length; i++)
        {
            nodes[i] = CreateVector.Dense<float>(nodeSetup[i]);
        }

        for (int i = 0; i < nodeSetup.Length - 1; i++)
        {
            weights[i] = CreateMatrix.Dense<float>(nodeSetup[i + 1], nodeSetup[i]);
            deltaWeights[i] = CreateMatrix.Dense<float>(nodeSetup[i + 1], nodeSetup[i]);

            biases[i] = CreateVector.Dense<float>(nodeSetup[i + 1]);
            deltaBiases[i] = CreateVector.Dense<float>(nodeSetup[i + 1]);
            gammaValues[i] = CreateVector.Dense<float>(nodeSetup[i + 1]);
        }

        InitialiseWeights();
        InitialiseBiases();
    }

    public Neural(List<float> flatValues)
    {
        nodes = new Vector<float>[nodeSetup.Length];
        weights = new Matrix<float>[nodeSetup.Length - 1];
        biases = new Vector<float>[nodeSetup.Length - 1];

        gammaValues = new Vector<float>[nodeSetup.Length - 1];
        deltaWeights = new Matrix<float>[nodeSetup.Length - 1];
        deltaBiases = new Vector<float>[nodeSetup.Length - 1];

        for (int i = 0; i < nodeSetup.Length; i++)
        {
            nodes[i] = CreateVector.Dense<float>(nodeSetup[i]);
        }

        for (int i = 0; i < nodeSetup.Length - 1; i++)
        {
            weights[i] = CreateMatrix.Dense<float>(nodeSetup[i + 1], nodeSetup[i]);
            deltaWeights[i] = CreateMatrix.Dense<float>(nodeSetup[i + 1], nodeSetup[i]);

            biases[i] = CreateVector.Dense<float>(nodeSetup[i + 1]);
            deltaBiases[i] = CreateVector.Dense<float>(nodeSetup[i + 1]);
            gammaValues[i] = CreateVector.Dense<float>(nodeSetup[i + 1]);
        }

        LoadFromAllValues(flatValues);
    }

    private void InitialiseWeights()
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int x = 0; x < weights[i].RowCount; x++)
            {
                for (int y = 0; y < weights[i].ColumnCount; y++)
                {
                    weights[i][x, y] = UnityEngine.Random.Range(-1f, 1f);
                    //weights[i][x, y] = 0;
                }
            }
        }
    }

    private void InitialiseBiases()
    {
        for (int i = 0; i < biases.Length; i++)
        {
            for (int x = 0; x < biases[i].Count; x++)
            {
                biases[i][x] = UnityEngine.Random.Range(-1f, 1f);
            }
        }
    }

    public float[] Propogate(float[] inputs)
    {
        for (int i = 0; i < nodes[0].Count; i++)
        {
            nodes[0][i] = inputs[i];
        }

        // Original sigmoid for all layersa
        for (int i = 0; i < weights.Length; i++)
        {
            // ADD SIGMOID DO NOT FORGET
            nodes[i + 1] = Sigmoid(weights[i] * nodes[i] + biases[i]);
            //Debug.Log(nodes[i+1].Count + " :: " + biases[i].Count);
        }

        return nodes[nodes.Length - 1].AsArray();
    }

    public static Vector<float> Sigmoid(Vector<float> input)
    {
        for (int i = 0; i < input.Count; i++)
        {
            input[i] = Sigmoid(input[i]);
        }

        return input;
    }

    public static float Sigmoid(float value)
    {
        return 1.0f / (1.0f + (float)System.Math.Exp(-value));
    }


    // Additions
    public List<float> AllValues()
    {
        List<float> values = new List<float>();

        for (int i = 0; i < weights.Length; i++)
        {
            for (int x = 0; x < weights[i].RowCount; x++)
            {
                for (int y = 0; y < weights[i].ColumnCount; y++)
                {
                    values.Add(weights[i][x, y]);
                }
            }
        }

        for (int i = 0; i < biases.Length; i++)
        {
            for (int x = 0; x < biases[i].Count; x++)
            {
                values.Add(biases[i][x]);
            }
        }

        return values;
    }

    public void LoadFromAllValues(List<float> flatValues)
    {
        int readPosition = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            for (int x = 0; x < weights[i].RowCount; x++)
            {
                for (int y = 0; y < weights[i].ColumnCount; y++)
                {
                    weights[i][x, y] = flatValues[readPosition];
                    readPosition++;
                }
            }
        }

        for (int i = 0; i < biases.Length; i++)
        {
            for (int x = 0; x < biases[i].Count; x++)
            {
                biases[i][x] = flatValues[readPosition];
                readPosition++;
            }
        }
    }

    public string NetworkString()
    {
        string s = "";
        foreach (float m in AllValues())
        {
            s += (m.ToString() + ", ");
        }

        return s;
    }

    public void Save()
    {
        string path = Application.dataPath + "/NeuralTestData.txt";

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "\n");
        }

        string content = NetworkString() + "\n";
        File.AppendAllText(path, content);
    }




    // Backpropogation
    public float Loss(float[] targetOutput)
    {
        float loss = 0;
        for (int i = 0; i < nodeSetup[nodeSetup.Length -1]; i++)
        {
            loss += (targetOutput[i] - nodes[nodes.Length - 1][i]) * (targetOutput[i] - nodes[nodes.Length - 1][i]);
        }

        return loss;
    }

    public void Backpropogate(float[] targetOutput)
    {
        CalculateGamma(targetOutput);
        CalculateDeltaBiases();
        CalculateDeltaWeights();

        for (int i = 0; i < nodeSetup.Length - 1; i++)
        {
            weights[i] = weights[i] - (learningRate * deltaWeights[i]);
            biases[i] = biases[i] - (learningRate * deltaBiases[i]);
        }
    }

    void CalculateGamma(float[] targetOutput)
    {
        // Go through last layer
        for (int i = 0; i < nodeSetup[nodeSetup.Length - 1]; i++)
        {
            gammaValues[gammaValues.Length - 1][i] = 2f * (nodes[nodes.Length - 1][i] - targetOutput[i]) * nodes[nodes.Length - 1][i] * (1f - nodes[nodes.Length - 1][i]);
            //Debug.Log("Gamma Output: " + gammaValues[gammaValues.Length - 1][i]);
        }

        // Go through hidden layers starting at last hidden
        for (int i = nodeSetup.Length - 2; i > 0; i--)
        {
            for (int x = 0; x < nodeSetup[i]; x++)
            {
                float deltaSum = 0f;
                for (int y = 0; y < nodeSetup[i+1]; y++)
                {
                    deltaSum += gammaValues[i][y] * weights[i][y, x];
                }
                gammaValues[i - 1][x] = deltaSum * nodes[i][x] * (1f - nodes[i][x]);

                //Debug.Log("Hidden Gamma: " + gammaValues[i - 1][x]);
            }
        }
    }

    void CalculateDeltaBiases()
    {
        for (int i = 0; i < nodeSetup.Length - 1; i++)
        {
            for (int x = 0; x < nodeSetup[i+1]; x++)
            {
                deltaBiases[i][x] = gammaValues[i][x];
                //Debug.Log("Layer: " + i + " Index: " + x + " Value: " + deltaBiases[i][x]);
            }
        }
    }

    void CalculateDeltaWeights()
    {
        // Go through all weights
        for (int i = 0; i < nodeSetup.Length - 1; i++)
        {
            for (int x = 0; x < nodeSetup[i+1]; x++)
            {
                for (int y = 0; y < nodeSetup[i]; y++)
                {
                    deltaWeights[i][x, y] = gammaValues[i][x] * nodes[i][y];
                   // Debug.Log("Layer: " + i + " Row: " + x + " Column: " + y + " Value = " + deltaWeights[i][x, y]);
                }
            }
        }
    }

}
