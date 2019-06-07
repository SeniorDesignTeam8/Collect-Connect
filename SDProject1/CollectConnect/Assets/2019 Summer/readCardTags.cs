using Mono.Data.Sqlite;
using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class readCardTags : MonoBehaviour
{
    float thinking = 3f;
    //is it oulling synonys from the web
    bool isPullSyn = false;
    //is it currently comparing sysonyms that have been gathered
    bool isCompSyn = false;
    //counts how many web request have been processed 
    public int runAllRequest;
    bool found_syn = false;
    public static int NUM_RESULTS=3;
    List<List<string>> card_tags;
    static string path = "Assets/Resources/Records/CSVEXPORTFILE.csv";
    StreamReader reader;
    int loc_card1_tags, loc_card2_tags;

    public static string choosen;

    List<string> syn1 = new List<string>(), syn2 = new List<string>();
    [SerializeField] GameEvent wordChosen;



    void Start()
    {
        //store all of the tags for the cards 
        storeCardTags();

        //after the cards have been dealt begin trying to generate relations
        Invoke("stepsToFindWord", .2f);
        
    }
    public void stepsToFindWord()
    {
        choosen = null;
        //find the index of the two cards that were dealt
        cardsDealt();
        runAllRequest = 0;
        isPullSyn = true;
        //run it through datamuse to find more synonyms
        findSynonyms();
        Invoke("comparePulledSyn", thinking);
    }
    //add the tags to an easily accessible list, excluding the first entry which is the name of the card
    //void addTaggedWords()
    //{
    //    StopAllCoroutines();
    //    if (tags1==null)
    //    {   tags1 = new List<string>(); tags2 = new List<string>(); }
    //    tags1.Clear(); tags2.Clear();

    //    for(int i =1;i <card_tags[loc_card1_tags].Count;i++)
    //    {
    //        tags1.Add(card_tags[loc_card1_tags][i]);
    //    }
    //    for (int i = 1; i < card_tags[loc_card2_tags].Count; i++)
    //    {
    //        tags2.Add(card_tags[loc_card2_tags][i]); 
    //    }


    //}

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
        string syn = "https://api.datamuse.com/words?rel_syn=";
        string spc= "https://api.datamuse.com/words?rel_spc=";
        string gen = "https://api.datamuse.com/words?rel_gen=";
        string par = "https://api.datamuse.com/words?rel_par=";

        string end = "&max=2";

        //if there is ANY web request being processed end the fuck outta em because that is messing up the list 
        StopAllCoroutines();

        //dis bitch emtpy... YEET
        syn1.Clear();
        syn2.Clear();

        for(int i=1; i< card_tags[loc_card1_tags].Count; i++)
        {
            StartCoroutine(GetRequest(syn + card_tags[loc_card1_tags][i] + end, syn1, card_tags[loc_card1_tags][i], "syn"));
            StartCoroutine(GetRequest(spc + card_tags[loc_card1_tags][i] + end, syn1, card_tags[loc_card1_tags][i], "spc"));
            StartCoroutine(GetRequest(gen + card_tags[loc_card1_tags][i] + end, syn1, card_tags[loc_card1_tags][i], "gen"));
            StartCoroutine(GetRequest(par + card_tags[loc_card1_tags][i] + end, syn1, card_tags[loc_card1_tags][i], "par"));
        }
        for (int i = 1; i < card_tags[loc_card2_tags].Count;i++)
        {
            StartCoroutine(GetRequest(syn + card_tags[loc_card2_tags][i] + end, syn2, card_tags[loc_card2_tags][i], "syn"));
            StartCoroutine(GetRequest(spc + card_tags[loc_card2_tags][i] + end, syn2, card_tags[loc_card2_tags][i], "spc"));
            StartCoroutine(GetRequest(gen + card_tags[loc_card2_tags][i] + end, syn2, card_tags[loc_card2_tags][i], "gen"));
            StartCoroutine(GetRequest(par + card_tags[loc_card2_tags][i] + end, syn2, card_tags[loc_card2_tags][i], "par"));
        }

    }


    void comparePulledSyn()
    {
        StopAllCoroutines();
        string syn = "https://api.datamuse.com/words?rel_syn=";
        string ml = "https://api.datamuse.com/words?ml=";
        string end = "&max=25";
        List<string> temp = new List<string>();
        for (int i = 0; i < syn1.Count; i++)
        {
            if(syn2.Contains(syn1[i]))
            {
                choosen = syn1[i];
                wordChosen.Raise();
                return;
            }
        }
        
        for (int i= 0; i<syn1.Count;i++)
        {
            StartCoroutine(GetRequest(syn + syn1[i] + end, temp, syn1[i], "syn"));
        }
        
        if(foundSecondSyn(temp, syn2))
            return;

        Debug.Log("Last Resort");
        System.Random rnd = new System.Random();
        if(rnd.Next()%2==0)
        {
            choosen = syn1[rnd.Next(0, syn1.Count)];
            //chose from list 1
        }
        else
            choosen = syn2[rnd.Next(0, syn2.Count)];

        wordChosen.Raise();

       
    }
    bool foundSecondSyn(List<string> secondSyn, List<string> compare )
    {
        for (int i = 0; i < secondSyn.Count; i++)
        {
            if (compare.Contains(secondSyn[i]))
            {
                choosen = secondSyn[i];
                wordChosen.Raise();
                return true;
            }
        }
        return false;
    }

    IEnumerator GetRequest(string uri, List<string> words, string original, string type)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] separatingStrings = { @"{""word"":""", "......." };

            string[] pages = webRequest.downloadHandler.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            int page = pages.Length - 1;

            if (page <= 0)
                runAllRequest++;
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
                    words.Add(pages[i]);
                    Debug.Log(original +":   "+ pages[i] + " : " +type+'\n');
                    if (i == page - 1)
                        runAllRequest++;
                }
                
            }
           
        } 
        
    }

}
