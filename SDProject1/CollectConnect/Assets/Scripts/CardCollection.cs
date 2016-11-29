using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class CardCollection : ScriptableObject
{
    private string _name;
    private readonly List<Card> _cardList;

    public int Size
    {
        get { return _cardList.Count; }
    }

    public CardCollection(string name, params Card[] cards)
    {
        _name = name;
        _cardList = new List<Card>();
        foreach (Card c in cards)
        {
            _cardList.Add(c);
        }
    }

    public CardCollection(string name, params CardCollection[] collections)
    {
        _name = name;
        _cardList = new List<Card>();
        foreach (CardCollection cC in collections)
        {
            _cardList.AddRange(cC._cardList);
        }
    }

    public CardCollection(string name)
    {
        _name = name;
        _cardList = new List<Card>();
    }

    public void Shuffle()
    {
        for (int i = 0; i < _cardList.Count; i++)
        {
            int index = Random.Range(0, _cardList.Count - 1);
            Card temp = _cardList[index];
            _cardList[index] = _cardList[i];
            _cardList[i] = temp;
        }
    }

    public Card Draw()
    {
        int index = Random.Range(0, _cardList.Count - 1);
        Card c = _cardList[index];
        _cardList.RemoveAt(index);
        return c;
    }
}
