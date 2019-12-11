using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Read : MonoBehaviour
{
    private string fileName= "./Assets/Resources/MyText.grd";
    private string str;
    private List<string> numList = new List<string>();
    private List<int> numList2 = new List<int>();
    string arrayStr3;
    string[] arrayStr4;
    int[] arrayInt5;
    private string[,] m_Sum = new string[159,159];

    private void Start()
    {
        ReadFile();
    }

    public void ReadFile()
    {
        StreamReader sr = File.OpenText(fileName);
        while (true)
        {
            str = sr.ReadLine();
            int lineLength = 0;
            if (str != null)
            {
                lineLength = str.Length;
                if (lineLength >= 1000)
                {
                    arrayStr3 = str.Replace("  ", " ");
                    arrayStr4 = arrayStr3.Split(' ');
                    arrayInt5 = new int[arrayStr4.Length];
                    for (int i = 0; i < arrayStr4.Length; i++)
                    {
                        arrayInt5[i] = int.Parse(arrayStr4[i].Substring(8, 2));
                        numList.Add(arrayStr4[i]);
                    }
                    for (int i = 0; i < arrayInt5.Length; i++)
                    {
                        numList2.Add(arrayInt5[i]);
                    }
                }
            }
            if (str == null)
            {
                break;
            }
        }
        for (int x = 0; x < 159; x++)
        {
            for (int y = 0; y < 159; y++)
            {
                int i = y + x * 159;
                m_Sum[x, y] = numList[i];
            }
        }
        for (int x = 0; x < 159; x++)
        {
            for (int y = 0; y < 159; y++)
            {
                //Debug.Log(m_Sum[x, y]);
            }
        }
        for (int i = 0; i < numList2.Count; i++)
        {
            Debug.Log(numList2[i]);
        }
        sr.Close();
    }
}
