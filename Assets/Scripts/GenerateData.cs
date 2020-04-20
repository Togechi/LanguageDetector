using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GenerateData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string path = Application.dataPath + "/circleData.txt";

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "\n");
        }

        for (int i = 0; i < 50; i++)
        {
            float x = UnityEngine.Random.Range(-0.8f, 0.8f);
            float y1 = Mathf.Sqrt(0.64f - (x * x));
            float y2 = y1 * -1;

            File.AppendAllText(path, "0" + "," + x + "," + y1 + "\n");
            File.AppendAllText(path, "0" + "," + x + "," + y2 + "\n");

            float n = UnityEngine.Random.Range(-0.45f, 0.45f);
            float m1 = Mathf.Sqrt(0.2025f - (n * n));
            float m2 = m1 * -1;

            File.AppendAllText(path, "1" + "," + n + "," + m1 + "\n");
            File.AppendAllText(path, "1" + "," + n + "," + m2 + "\n");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
