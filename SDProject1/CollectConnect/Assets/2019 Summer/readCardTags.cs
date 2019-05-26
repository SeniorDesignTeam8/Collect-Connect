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
    int loc_card1_tags, loc_card2_tags;

    // Start is called before the first frame update
    void Start()
    {
        storeCardTags();
      //  findSynonyms();
      //  
    }

    void storeCardTags()
    {
        bool first = true;
        card_tags = new List<List<string>>();
        reader = new StreamReader(File.OpenRead(path));
        while(!reader.EndOfStream)
        {
            List<string> temp = new List<string>();
            string line = reader.ReadLine();
            if(line!=null && !first)
            {
                string[] values = line.Split(',');
                
                int loc = values[0].LastIndexOf('.');
                values[0] = values[0].Remove(loc);
                temp.Add(values[0]);
              //  Debug.Log(values[0]);
                string [] values2 = values[1].Split(' ');
                for(int i =0;i<values2.Length;i++)
                {
               //     Debug.Log(values2[i]);
                    temp.Add(values2[i]);
                }
            }
            
            if (temp.Count > 0)
                card_tags.Add(temp);
            first = false;
        }
    }
    //get the location of the list where the tags for those cards are being stored
    //
    public void cardsDealt()
    {
        
        GameObject []cards = GameObject.FindGameObjectsWithTag("card");
        string temp1 = cards[0].GetComponent<cardID>().frontImg;
        string temp2 = cards[1].GetComponent<cardID>().frontImg;
        for (int i=0; i<card_tags.Count; i++)
        {
            if (card_tags[i][0] == temp1)
                loc_card1_tags = i;
            if (card_tags[i][0] == temp2)
                loc_card2_tags = i;
        }
        
    }
    public void findSynonyms()
    {
        //https://api.datamuse.com/words?ml=duck&sp=b*&max=10
        /*
         jjb 	Popular adjectives used to modify the given noun, per Google Books Ngrams 	beach → sandy
        syn 	Synonyms (words contained within the same WordNet synset) 	ocean → sea
        trg 	"Triggers" (words that are statistically associated with the query word in the same piece of text.) 	cow → milking
        ant 	Antonyms (per WordNet) 	late → early
        spc 	"Kind of" (direct hypernyms, per WordNet) 	gondola → boat
        gen 	"More general than" (direct hyponyms, per WordNet) 	boat → gondola
        com 	"Comprises" (direct holonyms, per WordNet) 	car → accelerator
        par 	"Part of" (direct meronyms, per WordNet) 	trunk → tree
         */
        StartCoroutine(GetRequest("https://api.datamuse.com/words?ml=dragon&max=5"));
    }
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] separatingStrings = { @"{""word"":""", "......." };

            string[] pages = webRequest.downloadHandler.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                for (int i = 1; i < page; i++)
                {
                    int loc = pages[i].IndexOf('"');
                    pages[i] = pages[i].Remove(loc);
                    Debug.Log(pages[i] + '\n');
                }
            }
        }
    }
}
