using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public int Score { get; private set; }
    public bool IsDrawingCards { get; private set; }

    public GameObject[] CardPlaceholders;
    public GameObject PlayerScore;
    public GameObject ExpCardPlace; // The expanded card placeholder.
    public const int HandSize = 5;

    private bool[] _slotStatus = new bool[HandSize]; // True if taken, false if available.
    private string _playerName; // The player's name (internally).
    private CardCollection _playerHand; // Represents the player's cards.

    private void Start()
    {
        IsDrawingCards = true;
        _playerName = gameObject.name.Replace(" ", "").ToLower(); // Remove spaces and change to all lowercase to standardize.
        _playerHand = new CardCollection(gameObject.name + "'s Hand");
    }

    private void Update()
    {
        if (IsDrawingCards)
        {
            if (_playerHand.Size < HandSize)
            {
                if (BoardManager.IsDeckReady)
                {
                    Card c = BoardManager.Deck.Draw();
                    c.MoveToBoard(this);
                    _playerHand.AddCards(c);
                }
                else
                {
                    IsDrawingCards = false;
                }
            }
            else
                IsDrawingCards = false;
        }
    }

    public void IncreaseScore(int reward)
    {
        Score += reward;
    }

    public void ReduceScore(int penalty)
    {
        Score -= penalty;
    }

    public void PlaceCard(Card c)
    {
        for (int i = 0; i < CardPlaceholders.Length; i++)
        {
            if (!_slotStatus[i])
            {
                c.GetComponent<Transform>().position = CardPlaceholders[i].GetComponent<Transform>().position;
                
                _slotStatus[i] = true;
            }
        }
    }
}
