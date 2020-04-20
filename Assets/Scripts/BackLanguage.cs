using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine.UI;

public class BackLanguage : MonoBehaviour
{
    public float learningRate = 0.01f;
    public Neural network;
    public NetworkRenderer networkRenderer;
    public int batchSize;
    public InputField inputText;

    List<float[]> inputs = new List<float[]>();
    List<float[]> outputs = new List<float[]>();
    List<string> inputStrings = new List<string>();

    int reader = 0;
    int propogations = 0;
    int epoch = 0;
    float percentageCorrect;
    bool isPaused = true;
    bool isInput = false;
    float totalLoss;
    float testLoss;

    // Start is called before the first frame update
    void Start()
    {
        network = new Neural();
        network.learningRate = learningRate;

        ReadData();
    }

    private void ReadData()
    {
        isPaused = true;
        string pathEng = Application.dataPath + "/Data/languageData.txt";
        List<string> linesEng = File.ReadAllLines(pathEng).ToList();
        inputStrings.AddRange(linesEng);

        foreach (string line in linesEng)
        {
            string[] entries = line.Split(',');
            //Debug.Log(entries[0] + "::" + entries[1]);

            inputs.Add(StringToInput(entries[0]));

            float[] emptyZeros = new float[2];

            if (entries[1] == "1")
            {
                emptyZeros[0] = 1f;
            }
            else if (entries[1] == "2")
            {
                emptyZeros[1] = 1f;
            }
            else
            {
                Debug.LogError("Something has gone wrong  " + entries.Length.ToString() + " :: " + entries[1]);
            }

            outputs.Add(emptyZeros);
        }

        Debug.Log("Read Data " + inputs.Count + "   " + outputs.Count);

        isPaused = false;
    }

    private void RunNetwork(float[] output, float[] input, bool doBackprop, bool lastInBatch)
    {
        float[] result = network.Propogate(input);
        //Debug.Log(result.Length + "  " + result[0]);

        float[] nodes = network.nodes[network.nodes.Length - 1].ToArray();

        //Debug.Log(network.Loss(output));
        if (doBackprop)
        { 
            totalLoss += network.Loss(output);
            network.Backpropogate(output);
        }
        else
        {
            //totalLoss += network.Loss(output);
        }

        propogations++; 

        int guess = OutputsToGuess(nodes);
        int answer = OutputsToGuess(outputs[reader]);
        if (guess == answer)
        {
            percentageCorrect++;
        }

        if (lastInBatch)
        {
            networkRenderer.Render(nodes, inputStrings[reader], guess, answer, percentageCorrect/batchSize*100f, totalLoss / batchSize, propogations, epoch, learningRate);
        }
    }

    private void RunNetwork(float[] input)
    {
        float[] result = network.Propogate(input);
        //Debug.Log(result.Length + "  " + result[0]);

        float[] nodes = network.nodes[network.nodes.Length - 1].ToArray();

        int guess = OutputsToGuess(nodes);

        networkRenderer.Render(nodes, inputText.text, guess, -1, -1f, -1f, 0, epoch, learningRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            learningRate += 0.0025f;
            network.learningRate = learningRate;
        }
        else if (Input.GetKeyDown(KeyCode.Minus))
        {
            learningRate -= 0.0025f;
            network.learningRate = learningRate;
        }


        if (isInput)
        {
            RunNetwork(StringToInput(inputText.text));
        }

        if (isPaused)
        {
            return;
        }

        // Do training
        for (int x = 0; x < 1; x++)
        {
            for (int i = 0; i < batchSize; i++)
            {
                if (reader >= inputs.Count - 1)
                {
                    epoch++;
                    reader = 0;
                    Debug.Log("EPOCH " + epoch);
                }
                
                if (i == batchSize - 1)
                {
                    RunNetwork(outputs[reader], inputs[reader], true, true);
                }
                else
                {

                    RunNetwork(outputs[reader], inputs[reader], true, false);
                }

                reader++;

            }
            Debug.Log("Average Loss = " + totalLoss/batchSize);
        }

        totalLoss = 0;
        percentageCorrect = 0;
    }

    int OutputsToGuess(float[] outputs)
    {
        float maxValue = outputs.Max();
        int maxIndex = outputs.ToList().IndexOf(maxValue);

        return maxIndex;
    }

    public float[] StringToInput(string s)
    {
        List<float> output = new List<float>();

        foreach (char c in s)
        {
            int[] emptyZeros = new int[26];
            int index = char.ToUpper(c) - 64;

            if (index >= 1 && index <= 26)
            {
                emptyZeros[index - 1] = 1;
            }
            
            foreach (int m in emptyZeros)
            {
                output.Add(m);
            }

        }

        float[] arrayOutput = output.ToArray();
        Array.Resize(ref arrayOutput, 312);
        return arrayOutput;
    }

    public void InputValueChanged()
    {
        if (inputText.text == "")
        {
            isPaused = false;
            isInput = false;
        }
        else
        {
            isPaused = true;
            isInput = true;
        }
    }

}
