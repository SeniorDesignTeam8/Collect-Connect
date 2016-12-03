using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public int Score { get; private set; }

    private string _playerName; // The player's name (internally).
    private CardCollection _playerHand; // Represents the player's cards.
    private static int _numInvalidHands; // Counts how many nonexistent players tried to draw hands.



    private void Start()
    {
        _playerName = gameObject.name.Replace(" ", "").ToLower(); // Remove spaces and change to all lowercase to standardize.
        if (_playerName == "player1" || _playerName == "player2" || _playerName == "player3" || _playerName == "player4")
            DrawHand(); // Draw 5 cards (or whatever's left in the deck).
        else // Invalid player name. Use empty hand.
        {
            _numInvalidHands++;
            _playerHand = new CardCollection("Invalid Deck #" + _numInvalidHands);
        }
    }

    public void DrawHand()
    {
        List<Card> handList = new List<Card>();
        int deckSize = BoardManager.Deck.Size;
        bool shouldCardsFlip = _playerName == "player1" || _playerName == "player4";
        for (int i = 0; i < Math.Min(5, deckSize); i++) // If there are < 5 cards left, just draw them all.
            handList.Add(BoardManager.Deck.Draw());
        foreach (var c in handList) // Move each card to the board, flipping if needed.
        {
            c.MoveToBoard(shouldCardsFlip);
        }
        _playerHand = new CardCollection(_playerName + "'s Hand", handList.ToArray());
    }



    public void IncreaseScore(int reward)
    {
        Score += reward;
    }

    public void ReduceScore(int penalty)
    {
        Score -= penalty;
    }
}
