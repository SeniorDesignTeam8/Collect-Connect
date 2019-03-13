﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimplBoard : MonoBehaviour
{

    [SerializeField]
    int numOptions;
    [SerializeField]
    Transform startPos;
    [SerializeField]
    Transform parentCardPos;
    [SerializeField]
    GameObject holderPF;
    GameObject [] spots;
    [SerializeField]
    GameObject keyBank;


    [SerializeField]
    endGameStats stats;


    GameObject[] dealtCards;
    List<GameObject> keywords;
    GameObject parentcard;
    GM gm;

	void Start ()
    {
        keywords = new List<GameObject>();
        gm = GetComponent<GM>();
        spots = new GameObject[numOptions];
        dealtCards = new GameObject[numOptions];

        createBoard();
        dealCards();
        dealKeywords();
        keyWordAcess(true);
        
	}


    public void createBoard()
    {
        float offsetX = startPos.position.x;
        float offsetY = startPos.position.y;
        float widthX = holderPF.GetComponent<RectTransform>().sizeDelta.x;
        widthX +=10;
        for (int i = 0; i < numOptions;i++)
        {
            spots[i] = Instantiate(holderPF, new Vector3(i * widthX + offsetX,  offsetY, 0), Quaternion.identity);
            spots[i].transform.SetParent(transform);
         //   spots[i].GetComponent<glow>().enabled = false;
        }

        // make board and place holders equal distance aprt 

    }
    public void dealKeywords()
    {
        int row=0;
        bool col = true;
        for(int i =0; i<6;i++)
        {
            if (i < 2) row = 0;
            else if (i == 2) { row = 1; }
            else if (i == 3) { row = 1; col = false; }
            else if (i > 3) row = 2;

            keywords.Add(gm.getKeywords(row, col));
            keywords[i].transform.SetParent(keyBank.transform);
        }
    }
    public void dealCards()
    {
        parentcard= gm.dealCard(true);
        parentcard.transform.SetParent(parentCardPos);
        RectTransform rt = parentcard.GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        
        stats.setParent(parentcard);

        int count = 0;
        foreach (var x in spots)
        {     
            GameObject card = gm.dealCard(false);
            card.transform.SetParent(x.transform);
            card.GetComponent<DragItems>().canBeMoved = false;

          //  x.GetComponent<Image>().color = Color.white;
           dealtCards[count] = card;
            count++;
        }
       // stopCardGlow();
    }

    public void stopCardGlow()
    {
        foreach (var x in dealtCards)
        {
            if (x.GetComponent<glow>() != null)
            {
                x.GetComponent<glow>().enabled = false;
                x.GetComponent<Image>().color = Color.white;
            }
        }
    }
    public void deactivate()
    {
        keyWordAcess(false);
        foreach (var x in dealtCards)
        {
            x.GetComponent<DragItems>().canBeMoved = false;
        }

    }


	public void cardAcess()
    {
        foreach (var x in dealtCards)
        {
            x.GetComponent<DragItems>().canBeMoved = true;
           // x.GetComponent<glow>().enabled = true;
        }
    }

    void keyWordAcess(bool access)
    {
        foreach (var x in keywords)
        {
            x.GetComponent<DragItems>().canBeMoved = access;
            if(x.GetComponentInChildren<glow>()!=null)
            { 
            if (x.GetComponentInChildren<glow>().enabled)
                x.GetComponentInChildren<glow>().background.color = new Color(0, 0, 0, 0);

            x.GetComponentInChildren<glow>().enabled = access;
           }
        }

    }

    public void roundOver()
    {
        foreach(var x in dealtCards)
        {
            if(x!=endGameStats.lastCard)
                Destroy(x);
        }
        foreach(var x in keywords)
        {
            if(x!=endGameStats.lastKeyword)
                Destroy(x);
        }
        keywords.Clear();

    }

    public void newRound()
    {
        dealCards();
        dealKeywords();
        keyWordAcess(true);
    }
}
