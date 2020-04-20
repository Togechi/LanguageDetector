using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRenderer : MonoBehaviour
{
    public Transform nodesObject;
    private List<Text> nodes;
    public Text guessText;
    public Text correctText;
    public Text answerText;
    public Text percentCorrectT;
    public Text avgLossT;
    public Text outputString;
    public Text epochText;
    public Text propText;
    public Text learningText;

    // Start is called before the first frame update
    void Start()
    {
        nodes = new List<Text>();
        foreach (Transform child in nodesObject)
        {
            nodes.Add(child.GetComponentInChildren<Text>());
        }

        //Debug.Log(nodes.Count);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Render(float[] nodes, string inputString, int guess, int answer, float percentageCorrect, float avgLoss, int propogations, int epoch, float learning)
    {
        int c = 0;
        Debug.Log("ABG: " + nodes.Length);
        for (int i = 0; i < this.nodes.Count; i++)
        {
            this.nodes[c].text = Math.Round(nodes[i], 2).ToString();
            c++;
        }

        outputString.text = inputString;
        if (guess == 0)
        {
            guessText.text = "English";
        }
        else if (guess == 1)
        {
            guessText.text = "Japanese";
        }
        else if (guess == -1)
        {

        }
        else
            Debug.LogError("Rendering Error");

        if (answer == 0)
        {
            answerText.text = "English";
        }
        else if (answer == 1)
        {
            answerText.text = "Japanese";
        }
        else if (answer == -1)
        {
            answerText.text = "";
        }
        else
            Debug.LogError("Rendering error");

        if (answer == -1)
        {
            correctText.text = "";
        }
        else if (guess == answer)
        {
            correctText.text = "Correct";
        }
        else
            correctText.text = "False";

        percentCorrectT.text = "Correct (Last 100): " + percentageCorrect + "%";
        avgLossT.text = "Avg Loss (Last 100): " + avgLoss;
        epochText.text = "Epoch: " + epoch;
        propText.text = "Propogations: " + propogations;
        learningText.text = "Learning Rate: " + learning;
    }

    public void Save(float percentCorrect, float avgLoss)
    {
        string path = Application.dataPath + "/NetworkData.txt";

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "\n");
        }

        string content = percentCorrect + "," + avgLoss + "\n";
        File.AppendAllText(path, content);
    }
}
