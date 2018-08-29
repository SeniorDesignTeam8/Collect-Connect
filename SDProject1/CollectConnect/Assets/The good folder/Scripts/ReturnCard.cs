using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnCard : MonoBehaviour {
    Board board;
    CardManager manager;

    void returnCardsToHand()
    {
        if(board.cardonBoard)
        {
            for(int i=0;i<board.available.Count;i++)
            {
                if(board.available[i].transform.childCount>0)
                {
                    if(board.available[i].transform.GetChild(0).gameObject.tag=="card")
                    {
                        GameObject card = board.available[i].transform.GetChild(0).gameObject;
                        card.transform.SetParent(card.GetComponent<Dragable>().hand);
                    }
                    else
                    {
                        GameObject tile = board.available[i].transform.GetChild(0).gameObject;
                        tile.transform.SetParent(tile.GetComponent<wordDrag>().bank);
                    }
                }
            }
        }
    }

    public void returnCardToDeck()
    {
        if(transform.childCount>0)
        {
            returnCardsToHand();
            manager.returned.Add(transform.GetChild(0).gameObject);
            Destroy(transform.GetChild(0).gameObject);
            manager.dealCards(manager.players[manager.turn]);
            manager.turnSystem();
        }
    }

	void Start ()
    {
        board = GameObject.Find("mainCanvas").GetComponent<Board>();
        manager= GameObject.Find("mainCanvas").GetComponent<CardManager>();
    }

    //85DCFFFF
}
