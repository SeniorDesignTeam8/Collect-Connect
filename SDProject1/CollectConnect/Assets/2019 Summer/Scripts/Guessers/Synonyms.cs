using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Random = System.Random;

public class Synonyms : MonoBehaviour
{
    public string choosen;

    private readonly string end = "&max=2";

    public GameEvent finished;
    private readonly string gen = "https://api.datamuse.com/words?rel_gen=";
    private readonly double maxTime = 20;
    private readonly string par = "https://api.datamuse.com/words?rel_par=";

    public Random rnd = new Random();
    private readonly string spc = "https://api.datamuse.com/words?rel_spc=";
    public DateTime startTime;

    private string syn = "https://api.datamuse.com/words?rel_syn=";

    public void getSynFromTag(List<string> original, List<string> syn)
    {
        for (var i = 0; i < original.Count; i++)
        {
            StartCoroutine(GetRequest(syn + original[i] + end, syn, original[i], "syn"));
            StartCoroutine(GetRequest(spc + original[i] + end, syn, original[i], "spc"));
            StartCoroutine(GetRequest(gen + original[i] + end, syn, original[i], "gen"));
            StartCoroutine(GetRequest(par + original[i] + end, syn, original[i], "par"));
        }
    }

    public void generatetxtfile(List<string> syn1, List<string> syn2)
    {
        var s = Environment.CurrentDirectory + "\\file.txt";
        Debug.Log(s);
        var list1 = "\"" + string.Join(" ", syn1) + "\"";
        var list2 = "\"" + string.Join(" ", syn2) + "\"";
        string[] lines = {list1, list2};
        var fileoutput = list1 + list2;
        Debug.Log(list1);
        Debug.Log(list2);
        //System.IO.WriteLine(list1);
//        File.WriteAllText(s,list1+Environment.NewLine);
//        File.AppendAllLines(s, list2);
        File.WriteAllLines(s, lines);
        Debug.Log("File is READY");
    }


    public virtual void compareSyn(List<string> syn1, List<string> syn2)
    {
        //Is this the right place to put this?
        generatetxtfile(syn1, syn2);
        for (var i = 0; i < syn1.Count; i++)
            if (syn2.Contains(syn1[i]))
            {
                choosen = syn1[i];
                finished.Raise();
                return;
            }

        if (!timedOut())
        {
            var synDepth1 = new List<string>();
            var synDepth2 = new List<string>();
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
                if (syn1.Count > 0)
                    choosen = syn1[rnd.Next(0, syn1.Count)];
            }
            else
            {
                if (syn2.Count > 0)
                    choosen = syn2[rnd.Next(0, syn2.Count)];
            }

            finished.Raise();
        }
    }

    public bool timedOut()
    {
        var delta = DateTime.Now - startTime;
        Debug.Log(delta.TotalSeconds.ToString());
        if (delta.TotalSeconds > maxTime) return true;
        return false;
    }

    public IEnumerator compareDepth(float delayTime, List<string> syn1, List<string> syn2, List<string> depth1,
        List<string> depth2)
    {
        yield return new WaitForSeconds(delayTime);
        if (depth1 != null)
        {
            syn1 = syn1.Concat(depth1).ToList();
            syn2 = syn2.Concat(depth2).ToList();
        }

        compareSyn(syn1, syn2);
    }

    public void endTheSearch()
    {
        StopAllCoroutines();
    }

    //save a list of words parsed froom datamuse 
    private IEnumerator GetRequest(string uri, List<string> words, string original, string type)
    {
        using (var webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] separatingStrings = {@"{""word"":""", "......."};

            var pages = webRequest.downloadHandler.text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
            var page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                //    if (pages.Length > 0)
                //        Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                for (var i = 1; i < page; i++)
                {
                    var loc = pages[i].IndexOf('"');
                    pages[i] = pages[i].Remove(loc);
                    words.Add(pages[i]);
                    Debug.Log(original + ":   " + pages[i] + " : " + type + '\n');
                }
            }
        }
    }
}