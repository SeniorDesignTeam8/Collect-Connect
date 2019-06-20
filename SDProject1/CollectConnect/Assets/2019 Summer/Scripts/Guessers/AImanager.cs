using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

public class AImanager : MonoBehaviour
{
    //names of the bots 
    [SerializeField] BasicGuesser hana;
    [SerializeField] TricksterGuesser loki;

    [SerializeField] GameObject keywords;
    GameObject[] words;
    System.Random rnd = new System.Random();
    // Start is called before the first frame update
    void Start()
    {
        words = new GameObject[2];
    }

    public void startAIGuess()
    {
        hana.guessBasic();
        loki.guessBasic();
    }
    public void showChoices()
    {
        GameObject GuessHolder = GameObject.FindGameObjectWithTag("GuessHold");
        makeKeyword(GuessHolder.transform, 0,true);
        makeKeyword(GuessHolder.transform, 1, false);
        
    }
    void makeKeyword(Transform parent, int index, bool trickster)
    {
        GameObject Word = Instantiate(keywords, parent);
        words[index] = Word;
        if (trickster)
        {
            Word.GetComponentInChildren<TextMeshProUGUI>().text = loki.choosen;
            Word.GetComponent<castVote>().ownedBy = (int)ConnectGM.names.Loki;
        }
        else
        {
            Word.GetComponentInChildren<TextMeshProUGUI>().text = hana.choosen;
            Word.GetComponent<castVote>().ownedBy = (int)ConnectGM.names.Hana;
        }
    }


    public void castVotes()
    {
        castVote[] describers = FindObjectsOfType<castVote>();
        //have loki vote at random
        int i = rnd.Next(0, describers.Length);
        while(!describers[i].castTheVote((int)ConnectGM.names.Loki, loki._voteIcon))
        {
            i = rnd.Next(0, describers.Length);
        }

        //same with Hana
        i = rnd.Next(0, describers.Length);
        while (!describers[i].castTheVote((int)ConnectGM.names.Hana, hana._voteIcon))
        {
            i = rnd.Next(0, describers.Length);
        }
    }
    public void deleteWords()
    {
        foreach (var x in words)
        {
            Destroy(x);
        }
    }
}
