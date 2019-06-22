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

    public void addPoints(ConnectGM.names id, int points)
    {
        if (id == ConnectGM.names.Hana)
            hana.points += points;
        else loki.points += points;
    }

    public void castVotes()
    {
        castVote[] describers = FindObjectsOfType<castVote>();
        //have hana and loki vote at random, with random time delay to look like thinking 
        StartCoroutine(thinkBeforVote(describers, ConnectGM.names.Hana, hana._voteIcon));
        StartCoroutine(thinkBeforVote(describers, ConnectGM.names.Loki, loki._voteIcon));

    }
    IEnumerator thinkBeforVote(castVote[] describers, ConnectGM.names id, GameObject voteIcon)
    {
        yield return new WaitForSeconds(rnd.Next(1, 10));
        int i = rnd.Next(0, describers.Length);
        while (!describers[i].castTheVote((int)id, voteIcon))
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
