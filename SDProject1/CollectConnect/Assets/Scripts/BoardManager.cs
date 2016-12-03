using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    public Player[] Players;
    public int Columns = 8, Rows = 8;
    public static CardCollection Deck;
    public static bool IsCardExpanded;

    private int _currentPlayer = 1;
    private bool _isTurnOver;
    private readonly List<Vector3> _gridPositions = new List<Vector3>();

    private void Start()
    {
        if (Instance == null)
        {
            BuildDeck();
            if (Deck == null)
                Deck = new CardCollection("Deck");
            Deck.Shuffle();
            foreach (Player p in Players)
            {
                p.DrawHand();
            }
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void InitializeList()
    {
        _gridPositions.Clear();
        for (var x = 1; x < Columns - 1; x++)
            for (var y = 1; y < Rows - 1; y++)
                _gridPositions.Add(new Vector3(x, y, 0f));
    }

    private Vector3 RandomPosition()
    {
        var randomIndex = Random.Range(0, _gridPositions.Count);
        var randomPosition = _gridPositions[randomIndex];
        _gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    private void LayOutObjectAtRandom(IList<GameObject> tileArray, int min, int max)
    {
        var objectCount = Random.Range(min, max + 1);

        for (var i = 0; i < objectCount; i++)
        {
            var tileChoice = tileArray[Random.Range(0, tileArray.Count)];
            Instantiate(tileChoice, RandomPosition(), Quaternion.identity);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (!_isTurnOver)
            return;
        _currentPlayer++;
        _currentPlayer %= Players.Length;
        _isTurnOver = false;
    }
    private static void BuildDeck()
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
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }
        // Load the artifacts from each collection to make cards from them. Then add them to their respective lists.
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
                    c.SetExpInfo(reader.ReadLine());
                    c.AddProperty("Collection", col, "1");
                    string s = reader.ReadLine();
                    while (s != @"\" && s != null)
                    {
                        string[] separated = s.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        c.AddProperty(separated[0], separated[2], separated[3]);
                        s = reader.ReadLine();
                    }
                    cards.Add(c);
                }
            }
            deckList.Add(new CardCollection(col, cards.ToArray()));
        }
        Deck = new CardCollection("Deck", deckList.ToArray());
    }
}
