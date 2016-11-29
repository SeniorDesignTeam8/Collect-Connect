using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public readonly string PlayerName;
    private CardCollection _playerHand;
    public int PlayerScore;

    public Player(string name)
    {
        PlayerName = name;
    }

    public void DrawHand()
    {
        List<Card> handList = new List<Card>();
        int deckSize = BoardManager.Deck.Size;
        for (int i = 0; i < Math.Min(5, deckSize); i++)
            handList.Add(BoardManager.Deck.Draw());
        _playerHand = new CardCollection(PlayerName + "'s Hand", handList.ToArray());
    }



    public void IncreaseScore(int reward)
    {
        PlayerScore += reward;
    }

    public void ReduceScore(int penalty)
    {
        PlayerScore -= penalty;
    }
}
