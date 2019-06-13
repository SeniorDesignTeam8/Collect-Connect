

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

    string  syn = "https://api.datamuse.com/words?rel_syn=";
    string  spc = "https://api.datamuse.com/words?rel_spc=";
    string  gen = "https://api.datamuse.com/words?rel_gen=";
    string  par = "https://api.datamuse.com/words?rel_par=";

    string end = "&max=2";

    public GameEvent finished;

    public void getSynFromTag(List<string> original, List<string> syn)
    {
        for (int i = 0; i < original.Count; i++)
        {
            StartCoroutine(GetRequest(syn + original[i] + end, syn, original[i], "syn"));
            StartCoroutine(GetRequest(spc + original[i] + end, syn, original[i], "spc"));
            StartCoroutine(GetRequest(gen + original[i] + end, syn, original[i], "gen"));
            StartCoroutine(GetRequest(par + original[i] + end, syn, original[i], "par"));
        }
    }

 
    public virtual void compareSyn(List<string> syn1, List<string> syn2)
    {
        for (int i = 0; i < syn1.Count; i++)
        {
            if (syn2.Contains(syn1[i]))
            {
                choosen = syn1[i];
                finished.Raise();
                return;
            }
        }

        if (!timedOut())
        {
            List<string> synDepth1 = new List<string>();
            List<string> synDepth2 = new List<string>();
            getSynFromTag(syn1, synDepth1);
            getSynFromTag(syn2, synDepth2);
            StartCoroutine(compareDepth(5, syn1, syn2, synDepth1, synDepth2));
        }
        else
        {
            //chose from list 1
            Debug.Log("Last Resort");
            if (rnd.Next() % 2 == 0)
            {
                choosen = syn1[rnd.Next(0, syn1.Count)];

            }
            else
                choosen = syn2[rnd.Next(0, syn2.Count)];

            finished.Raise();
        }
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

    public IEnumerator compareDepth(float delayTime, List<string> syn1, List<string> syn2, List<string> depth1, List<string> depth2)
    {
        yield return new WaitForSeconds(delayTime);
        if (depth1 != null)
        {
            syn1 = syn1.Concat(depth1).ToList();
            syn2 = syn2.Concat(depth2).ToList();
        }
        compareSyn(syn1, syn2);
    }
    //save a list of words parsed froom datamuse 
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
                if (pages.Length > 0)
                    Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                for (int i = 1; i < page; i++)
                {
                    int loc = pages[i].IndexOf('"');
                    pages[i] = pages[i].Remove(loc);
                    words.Add(pages[i]);
                    Debug.Log(original + ":   " + pages[i] + " : " + type + '\n');
                }

            }

        }

    }
}