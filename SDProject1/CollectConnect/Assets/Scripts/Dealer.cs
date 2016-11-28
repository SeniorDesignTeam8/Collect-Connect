using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Networking.NetworkSystem;
using Random = UnityEngine.Random;


public class Dealer : MonoBehaviour
{
    [Serializable]
    public class CardCollection
    {
        private string _name;
        private readonly List<Card> _cardList;

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
            Card drawnCard;
            try
            {
                drawnCard = _cardList[0];
                _cardList.RemoveAt(0);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
            return drawnCard;
        }
    }

    public static Dealer Instance;
    private CardCollection _deck;

    // Use this for initialization
    private void Awake()
    {
        if (Instance == null)
        {
            BuildDeck();
            if (_deck == null)
                _deck = new CardCollection("Deck");
            _deck.Shuffle();
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void BuildDeck()
    {
        List<CardCollection> deckList = new List<CardCollection>();
        // Load the collections.
        List<string> collectionList = new List<string>();
        try
        {
            using (
                StreamReader reader =
                    new StreamReader(new FileStream(Application.dataPath + @"\Assets\TextFiles\Collections.txt",
                        FileMode.OpenOrCreate)))
            {
                while (!reader.EndOfStream)
                {
                    collectionList.Add(reader.ReadLine());
                }
            }
            collectionList = collectionList.Distinct().ToList(); // Remove any duplicates.
        }
        catch (Exception)
        {
            throw;
        }
        // Load the artifacts from each collection to make cards from them. Then add them to their respective lists.
        try
        {
            foreach (string col in collectionList)
            {
                List<Card> cards = new List<Card>();
                
                using (
                    StreamReader reader =
                        new StreamReader(
                            new FileStream(Application.dataPath + @"\Assets\TextFiles\Collections\" + col + @".txt",
                                FileMode.OpenOrCreate)))
                {
                    while (!reader.EndOfStream)
                    {
                        Card c = new Card();
                        c.SetName(reader.ReadLine());
                        c.AddProperty("Collection", col, "1");
                        string s = reader.ReadLine();
                        while (s != @"\" && s != null)
                        {
                            string[] separated = s.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                            c.AddProperty(separated[0], separated[2], separated[3]);
                            s = reader.ReadLine();
                        }
                        cards.Add(c);
                    }
                }
                deckList.Add(new CardCollection(col, cards.ToArray()));
            }
        }
        catch (Exception)
        {
            throw;
        }
        _deck = new CardCollection("Deck", deckList.ToArray());
    }
}
