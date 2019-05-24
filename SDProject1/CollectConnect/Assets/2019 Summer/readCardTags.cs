using Mono.Data.Sqlite;
using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
public class readCardTags : MonoBehaviour
{
    public static int NUM_RESULTS=3;
    List<List<string>> card_tags;
    static string path = "Assets/Resources/Records/CSVEXPORTFILE.csv";
    StreamReader reader;
    
  
    // Start is called before the first frame update
    void Start()
    {
        card_tags = new List<List<string>>();
        reader = new StreamReader(File.OpenRead(path));
        while(!reader.EndOfStream)
        {
            List<string> temp = new List<string>();
            string line = reader.ReadLine();
            if(line!=null)
            {
                string[] values = line.Split(',');
                temp.Add(values[0]);
                string [] values2 = values[1].Split(' ');
                for(int i =0;i<values2.Length;i++)
                {
                    Debug.Log(values2[i]);
                    temp.Add(values2[i]);
                }
            }
            if (temp.Count > 0)
                card_tags.Add(temp);

        }
    }

}
