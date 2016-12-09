﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

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
        PlayerScore.GetComponent<Text>();
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

    public void PlaceCard(Card c, Vector3 rotation)
    {
        for (int i = 0; i < CardPlaceholders.Length; i++)
        {
            if (!_slotStatus[i])
            {
                c.transform.position = CardPlaceholders[i].transform.position + new Vector3(0,0, -5);
                c.transform.Rotate(rotation, Space.Self);
                _slotStatus[i] = true;
                break;
            }
        }
    }

    public List<string> GetKeywords()
    {
        // Get the property value string from the property list in each card.
        List<string> keywords = (from Card c in _playerHand from prop in c._propertyList select prop.PropertyValue).ToList();
        // Remove any duplicates and return.
        return keywords.Distinct().ToList();
    }

    public CardCollection GetHand()
    {
        return _playerHand;
    }

    public void CardExpansion(Card card, Player player)
    {
        //TODO: connect to correct player board
    }
}
