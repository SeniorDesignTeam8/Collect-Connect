﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BasicGuesser : Synonyms
{
    List<string> syn1 = new List<string>();
    List<string> syn2 = new List<string>();

    public void guessBasic()
    {

        syn1.Clear();
        syn2.Clear();

        startTime = DateTime.Now;
        getSynFromTag(readCardTags._abstract[readCardTags.loc_card1_tags], syn1);
        getSynFromTag(readCardTags._abstract[readCardTags.loc_card2_tags], syn2);
        StartCoroutine(compareDepth(5f, syn1, syn2, syn1, syn2));
    }
}
