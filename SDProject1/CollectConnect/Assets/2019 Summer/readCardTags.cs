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
    List<string> tags1, tags2;
    public static string choosen;

    List<string> syn1 = new List<string>(), syn2 = new List<string>();
    [SerializeField] GameEvent wordChosen;
    // Start is called before the first frame update
    void Start()
    {
        //store all of the tags for the cards 
        storeCardTags();
        Invoke("stepsToFindWord", .5f);
        
    }
    public void stepsToFindWord()
    {
        choosen = null;
        //find the index of the two cards that were dealt
        cardsDealt();
        addTaggedWords();
        findSynonyms();
    }
    //add the tags to an easily accessible list, excluding the first entry which is the name of the card
    void addTaggedWords()
    {
        if(tags1==null)
        {   tags1 = new List<string>(); tags2 = new List<string>(); }
        else { tags1.Clear(); tags2.Clear(); }

        for(int i =1;i <card_tags[loc_card1_tags].Count;i++)
        {
            tags1.Add(card_tags[loc_card1_tags][i]);
        }
        for (int i = 1; i < card_tags[loc_card2_tags].Count; i++)
        {
            tags2.Add(card_tags[loc_card2_tags][i]); 
        }


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
        string basic = "https://api.datamuse.com/words?rel_syn=";
        string spc= "https://api.datamuse.com/words?rel_spc=";
        string jjb= "https://api.datamuse.com/words?rel_jjb=";
        string trg= "https://api.datamuse.com/words?rel_trg=";

        string gen = "https://api.datamuse.com/words?rel_gen=";
        string com = "https://api.datamuse.com/words?rel_com=";
        string par = "https://api.datamuse.com/words?rel_par=";
        string ml = "https://api.datamuse.com/words?ml=";
        string end = "&max=5";
        syn1.Clear();
        syn2.Clear();
        for(int i=0; i< tags1.Count; i++)
        {
            //StartCoroutine(GetRequest(basic + tags1[i] + end, syn1, tags1[i], "syn"));
            //StartCoroutine(GetRequest(spc + tags1[i] + end, syn1, tags1[i], "spc"));
            //StartCoroutine(GetRequest(jjb + tags1[i] + end, syn1, tags1[i], "jjb"));
            //StartCoroutine(GetRequest(trg + tags1[i] + end, syn1, tags1[i], "trg"));


            StartCoroutine(GetRequest(gen + tags1[i] + end, syn1, tags1[i], "gen"));
            StartCoroutine(GetRequest(com + tags1[i] + end, syn1, tags1[i], "com"));
            StartCoroutine(GetRequest(par + tags1[i] + end, syn1, tags1[i], "par"));
            StartCoroutine(GetRequest(ml + tags1[i] + end, syn1, tags1[i], "ml"));
        }
        for (int i = 0; i < tags2.Count;i++)
        {
            //StartCoroutine(GetRequest(basic + tags2[i] + end, syn2, tags2[i], "syn"));
            //StartCoroutine(GetRequest(spc + tags2[i] + end, syn2, tags2[i], "spc"));
            //StartCoroutine(GetRequest(jjb + tags2[i] + end, syn2, tags2[i], "jjb"));
            //StartCoroutine(GetRequest(trg + tags2[i] + end, syn2, tags2[i], "trg"));


            StartCoroutine(GetRequest(gen + tags2[i] + end, syn2, tags2[i], "gen"));
            StartCoroutine(GetRequest(com + tags2[i] + end, syn2, tags2[i], "com"));
            StartCoroutine(GetRequest(par + tags2[i] + end, syn2, tags2[i], "par"));
            StartCoroutine(GetRequest(ml + tags2[i] + end, syn2, tags2[i], "ml"));
        }
        Invoke("word", 2);

        //https://api.datamuse.com/words?
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
    }

    void word()
    {
        for(int i=0; i<syn1.Count;i++)
        {
            for(int j=0;j<syn2.Count;j++)
            {

            }
        }
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
    IEnumerator isSyn(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
        }
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
                }
            }
        }
    }
}
