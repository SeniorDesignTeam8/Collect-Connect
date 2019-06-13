﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;

public class Dealer : Synonyms
{
    public static string[] keywords= new string[3];
    List<string> syn1= new List<string>();
    List<string> syn2 = new List<string>();
    [SerializeField]GameEvent keywordsChosen;


    public void stepsGetKeywords()
    {
        finished = keywordsChosen;
        syn1.Clear();
        syn2.Clear();

        pickSingleDescriptor();

        startTime = DateTime.Now;
        getSynFromTag(readCardTags._basic[readCardTags.loc_card1_tags],  syn1);
        getSynFromTag(readCardTags._basic[readCardTags.loc_card2_tags], syn2);

        StartCoroutine(compareDepth(5f, syn1, syn2, syn1, syn2));

    }
    public override void compareSyn(List<string> syn1, List<string> syn2)
    {
        for (int i = 0; i < syn1.Count; i++)
        {
            if (syn2.Contains(syn1[i]))
            {
                if (syn1[i] != keywords[0] && syn1[i] != keywords[1])
                    keywords[2] = syn1[i];
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
                keywords[2] = syn1[rnd.Next(0, syn1.Count)];

            }
            else
                keywords[2] = syn2[rnd.Next(0, syn2.Count)];

            finished.Raise();
        }
    }



    //pick 1 word from the list of tags for each card that are not the same word 
    void pickSingleDescriptor()
    {
        int i = rnd.Next(0, readCardTags._basic[readCardTags.loc_card1_tags].Count);
        keywords[0] = readCardTags._basic[readCardTags.loc_card1_tags][i];

        i = rnd.Next(0, readCardTags._basic[readCardTags.loc_card2_tags].Count);
        keywords[1] = readCardTags._basic[readCardTags.loc_card2_tags][i];

        //make sure each keyword is unique 
        while(keywords[0]==keywords[1])
        {
            i = rnd.Next(0, readCardTags._basic[readCardTags.loc_card2_tags].Count);
            keywords[1] = readCardTags._basic[readCardTags.loc_card2_tags][i];
        }

    }

}
