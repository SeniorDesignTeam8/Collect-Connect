using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BasicGuesser : Synonyms
{
    public int points=0;
    List<string> syn1 = new List<string>();
    List<string> syn2 = new List<string>();
    [SerializeField] GameObject voteIcon;
    public GameObject _voteIcon;
    public void guessBasic()
    {
        readyToCastVote();
        syn1.Clear();
        syn2.Clear();
        startTime = DateTime.Now;
        getSynFromTag(readCardTags._basic[readCardTags.loc_card1_tags], syn1, true);
        getSynFromTag(readCardTags._basic[readCardTags.loc_card2_tags], syn2, false);
    }
    private void Update()
    {
        if (firstCard && secondCard)
            compareSyn(syn1, syn2);
    }
    public void readyToCastVote()
    {
        _voteIcon = Instantiate(voteIcon);
    }
}
