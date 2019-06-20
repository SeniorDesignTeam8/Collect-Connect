using Mono.Data.Sqlite;
using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class readCardTags : MonoBehaviour
{
    public static List<List<string>> _abstract;
    public static List<List<string>> _basic;
    public static List<List<string>> _silly;
    public static List<string> cardName;

    static string path = "Assets/Resources/Records/CSVEXPORTFILE.csv";
    StreamReader reader;
    public static int loc_card1_tags = -1, loc_card2_tags = -1;



    void Start()
    {
         _abstract = new List<List<string>>();
         _basic = new List<List<string>>();
         _silly = new List<List<string>>();
         cardName = new List<string>();
        //store all of the tags for the cards 
        storeCardTags();
    }

    void storeCardTags()
    {
        bool first = true;
        reader = new StreamReader(File.OpenRead(path));
        while(!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            if(line!=null && !first)
            {
                //splits the columns of the csv file into 4 strings
                //1st is the cardName 2nd is basic words 3rd is abstract words 4th is sill words
                string[] values = line.Split(',');
                
                //removes the .png from the image name 
                int loc = values[0].LastIndexOf('.');
                values[0] = values[0].Remove(loc);

                //adds the 1st value from the split line which is the name 
                cardName.Add(values[0]);

                // split the description values to each be their own string list 
                _basic.Add(getKeywords(values[1]));
                _abstract.Add(getKeywords(values[2]));
                _silly.Add(getKeywords(values[3]));
                
            }
                
            first = false;
        }
    }
    List<string> getKeywords(string inital)
    {
        string[] basicArry = inital.Split(' ');
        List<string> temp = new List<string>();
        for (int i = 0; i < basicArry.Length; i++)
        {
            temp.Add(basicArry[i]);
        }
        return temp;
    }

    //get the location of the list where the tags for those cards are being stored
    public static void getDealtCardsName()
    {
        GameObject []cards = GameObject.FindGameObjectsWithTag("card");
        string temp1 = cards[0].GetComponent<cardID>().frontImg;
        string temp2 = cards[1].GetComponent<cardID>().frontImg;
        for (int i=0; i<cardName.Count; i++)
        {
            if (cardName[i] == temp1)
                loc_card1_tags = i;
            if (cardName[i]== temp2)
                loc_card2_tags = i;
        }
        
    }
}
