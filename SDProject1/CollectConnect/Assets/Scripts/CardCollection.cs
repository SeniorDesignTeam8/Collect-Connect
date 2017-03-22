using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class CardCollection : IEnumerable
{
    private string _name;
    private List<Card> _cardList;

    public int Size
    {
        get { return _cardList.Count; }
    }

    public CardCollection(string name)
    {
        _name = name;
        _cardList = new List<Card>();
    }

    public void RemoveAt(int index)
    {
        _cardList.RemoveAt(index);
    }

    public void AddCards(params Card[] cards)
    {
        _cardList.AddRange(cards);
        _cardList = _cardList.Distinct().ToList();
    }

    public void Shuffle()
    {
        for (int i = 0; i < _cardList.Count; i++)
        {
            int index = Random.Range(0, _cardList.Count);
            Card temp = _cardList[index];
            _cardList[index] = _cardList[i];
            _cardList[i] = temp;
        }
    }

    public Card Draw()
    {
        int index = Random.Range(0, _cardList.Count);
        Card c = _cardList[index];
        _cardList.RemoveAt(index);
        return c;
    }

    public IEnumerator GetEnumerator()
    {
        // This allows us to use a foreach loop across a CardCollection.
        return ((IEnumerable) _cardList).GetEnumerator();
    }

    public int IndexOf(Card searchCard)
    {
        return _cardList.IndexOf(searchCard);
    }

    public Card At(int index)
    {
        return _cardList[index];
    }
}
