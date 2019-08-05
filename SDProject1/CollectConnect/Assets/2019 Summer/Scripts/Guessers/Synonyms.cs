

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
public class Synonyms: MonoBehaviour
{

    public System.Random rnd=new System.Random();
    public DateTime startTime;
    public string choosen;
    double maxTime = 20;
    int maxDepth = 2;
    public bool firstCard, secondCard;
    public bool done;
    // syn  spc  gen par
    string[] types = { "https://api.datamuse.com/words?ml=", "https://api.datamuse.com/words?rel_jjb=", "https://api.datamuse.com/words?rel_gen=" };//"https://api.datamuse.com/words?rel_par=" };
    string end = "&max=2";

    public GameEvent finished;

    public void getSynFromTag(List<string> original, List<string> syn, bool first)
    {
        done = false;
        firstCard = secondCard = false;
        StartCoroutine(GetRequest(syn,original, first));
    }

 
    public virtual void compareSyn(List<string> syn1, List<string> syn2)
    {
        //will use the new python script to compare word similarity 
        // so for now will go back to picking at random until we have the new script, no need to do extra computation 
        firstCard = false;
        for (int i = 0; i < syn1.Count; i++)
        {
            if (syn2.Contains(syn1[i]))
            {
                choosen = syn1[i];
                //
                done = true;
                return;

            }
        }

        //chose from list 1
      //  Debug.Log("Last Resort");
        if (rnd.Next() % 2 == 0)
        {
            choosen = syn1[rnd.Next(0, syn1.Count)];

        }
        else
            choosen = syn2[rnd.Next(0, syn2.Count)];
        done = true;
       //Call GM script to set 
    }

    public bool timedOut()
    {
        TimeSpan delta = DateTime.Now - startTime;
        Debug.Log(delta.TotalSeconds.ToString());
        if (delta.TotalSeconds > maxTime)
        {
            return true;
        }
        return false;
    }


    IEnumerator GetRequest(List<string> synonyms, List<string> original, bool first)
    {
        int listSizeBefore =0 ;
        synonyms.Clear();
        for (int depth = 0; depth < maxDepth; depth++)
        {
           
            synonyms.Clear();

            for (int i =listSizeBefore ; i <original.Count; i++)
            {
                for (int j = 0; j < types.Length; j++)
                {
                    string uri = types[j] + original[i] + end;
                    UnityWebRequest webRequest = UnityWebRequest.Get(uri);

            // Request and wait for the desired page.
                    yield return webRequest.SendWebRequest();

                    string[] separatingStrings = { @"{""word"":""", "......." };

                    string[] pages = webRequest.downloadHandler.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                    int page = pages.Length - 1;
                    if (webRequest.isNetworkError)
                    {
                        Debug.Log(": Error: " + webRequest.error);
                    }
                    else
                    {
                        for (int k = 1; k < page; k++)
                        {
                            int loc = pages[k].IndexOf('"');
                            pages[k] = pages[k].Remove(loc);
                            if(!original.Contains(pages[k])&& !synonyms.Contains(pages[k]))
                                synonyms.Add(pages[k]);
                        }

                    }
                }
            }
            listSizeBefore = original.Count;
            original = original.Concat(synonyms).ToList();
        }
        //end loop 
        synonyms = original;
  //      Debug.Log(original[0]);

        if (first)
        {
            firstCard = true;
    //        Debug.Log("First " + original[0]);
        }
        else
        {
            secondCard = true;
 //           Debug.Log("Second " + original[0]);
        }

    }
}