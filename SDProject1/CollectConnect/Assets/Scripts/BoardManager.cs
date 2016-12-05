using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    public static bool IsDeckReady { get; private set; }
    public static BoardManager Instance;
    public static GameObject[] Players;
    public int Columns = 8, Rows = 8;
    public static CardCollection Deck;
    public static bool IsCardExpanded;
    public AudioSource SoundEffectSource;
    public AudioClip SelectSound;
    public AudioClip DeselectSound;
    public AudioClip ExpandSound;

    private int _currentPlayer = 1;
    private bool _isTurnOver;
    private readonly List<Vector3> _gridPositions = new List<Vector3>();

    private void Awake()
    {
        IsDeckReady = false;
    }

    private void Start()
    {
        if (Instance == null)
        {
            Deck = new CardCollection("Deck");
            BuildDeck();
            if (Deck == null)
                Deck = new CardCollection("Deck");
            Deck.Shuffle();
            IsDeckReady = true;
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Deck.Size == 0)
            IsDeckReady = false;
        // Play turn like normal.
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
        // Load the collections.
        List<string> collectionList = new List<string>();
        try
        {
            using (
                StreamReader reader =
                    new StreamReader(new FileStream(Application.dataPath + @"/TextFiles/Collections.txt",
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
            using (
                StreamReader reader =
                    new StreamReader(
                        new FileStream(Application.dataPath + @"/TextFiles/Collections/" + col + @".txt",
                            FileMode.OpenOrCreate)))
            {
                while (!reader.EndOfStream)
                {
                    GameObject c = Instantiate(GameObject.Find("Card"));
                    c.AddComponent<Card>();
                    Card cardComponent = c.GetComponent<Card>();
                    cardComponent.name = reader.ReadLine();
                    cardComponent.AddProperty("Collection", col, "1");
                    string s = reader.ReadLine();
                    cardComponent.SetExpInfo(s);
                    s = reader.ReadLine();
                    while (s != @"|" && s != null)
                    {
                        string[] separated = s.Split('\\');
                        cardComponent.AddProperty(separated[0], separated[1], separated[2]);
                        s = reader.ReadLine();
                    }
                    Deck.AddCards(cardComponent);
                }
            }
        }
    }

    public void PlaySelect()
    {
        if (SoundEffectSource.isPlaying)
            SoundEffectSource.Stop();
        SoundEffectSource.clip = SelectSound;
        SoundEffectSource.Play();
    }

    public void PlayDeselect()
    {
        if (SoundEffectSource.isPlaying)
            SoundEffectSource.Stop();
        SoundEffectSource.clip = DeselectSound;
        SoundEffectSource.Play();
    }

    public void PlayExpand()
    {
        if (SoundEffectSource.isPlaying)
            SoundEffectSource.Stop();
        SoundEffectSource.clip = ExpandSound;
        SoundEffectSource.Play();
    }
}
