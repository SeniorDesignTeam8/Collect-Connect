using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    public static bool IsDeckReady { get; private set; }
    public static BoardManager Instance;
    public GameObject[] Players;
    public GameObject KeywordContainer;
    public GameObject keywordPrefab;
    public GameObject NodeOne;
    public int Columns = 8, Rows = 8;
    public static CardCollection Deck;
    public static bool IsCardExpanded;
    public AudioSource SoundEffectSource;
    public AudioClip SelectSound;
    public AudioClip DeselectSound;
    public AudioClip ExpandSound;
    public AudioClip PlaceSound;
    private List<string> _keywordList;

    private List<GameObject> _keywordNodes;
    private List<Player> _playerScriptRefs;
    private bool _isGameStarted;
    private bool _isFirstCardPlay;
    private bool _isPlayerCardSelected;
    private bool _isBoardCardSelected;
    private int _currentPlayer = 0;
    private bool _isTurnOver;
    private readonly List<Vector3> _gridPositions = new List<Vector3>();
    private int[] _scoreboard;

    private void Awake()
    {
        _isGameStarted = false;
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
            _playerScriptRefs = new List<Player>();
            foreach (GameObject player in Players)
                _playerScriptRefs.Add(player.GetComponent<Player>());
            _keywordList = new List<string>();
            _scoreboard = new int[Players.Length];
            _keywordNodes = new List<GameObject>();
            _isFirstCardPlay = true;
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
        // First, check if all players have drawn their cards.
        // If so, then populate the players' word banks.
        if (!_isGameStarted)
        {
            bool allHandsDrawn = Players.All(t => !t.GetComponent<Player>().IsDrawingCards);

            if (allHandsDrawn)
            {
                _keywordList.Clear();
                //Clear and (re?)populate the word banks.
                foreach (GameObject t in Players)
                {
                    _keywordList.AddRange(t.GetComponent<Player>().GetKeywords());
                }
                // Remove any duplicates, then we're ready to start.
                // TODO: Might place a first card at center of the board.
                _keywordList = _keywordList.Distinct().ToList();
                PopulateKeywords();
                _isGameStarted = true;
            }

        }

        if (Deck.Size == 0)
            IsDeckReady = false;
        // Play turn like normal.
        if (_isFirstCardPlay)
        {
            if (_isPlayerCardSelected)
            {
                foreach (Card c in _playerScriptRefs[_currentPlayer].GetHand())
                {
                    if (c.IsSelected())
                    {
                        ConnectionManager.CreateConnection(c.GetComponent<RectTransform>());
                        c.SetIsSelected(false);
                        c.SetIsOnBoard(true);
                        PlayPlace();
                        _isPlayerCardSelected = false;
                        _isFirstCardPlay = false;
                        _isTurnOver = true;
                    }
                }
            }
        }
        else
        {
            
        }
    }

    private void LateUpdate()
    {
        if (!_isTurnOver)
            return;
        _currentPlayer++;
        _currentPlayer %= Players.Length;
        //TODO: Set keyword list to scroll Rect
        PopulateKeywords();
        _isTurnOver = false;
        _isPlayerCardSelected = false;
        _isBoardCardSelected = false;
    }

    private void PopulateKeywords()
    {

        //Clear and (re?)populate the word banks.
        foreach (GameObject t in Players)
        {
            _keywordList.AddRange(t.GetComponent<Player>().GetKeywords());
        }
        

        // clear the list
        foreach (Transform child in KeywordContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }




        foreach (string str in _keywordList)
        {
            GameObject go = Instantiate(keywordPrefab) as GameObject;
            go.GetComponentInChildren<Text>().text = str;
            go.transform.SetParent(KeywordContainer.transform);

            Vector3 scale = transform.localScale;
            scale.x = 1;
            scale.y = 1;
            scale.z = 1;
            go.transform.localScale = scale;
            go.SetActive(true);

            //Debug.Log(str);
        }

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

    private void PlaySelect()
    {
        if (SoundEffectSource.isPlaying)
            SoundEffectSource.Stop();
        SoundEffectSource.clip = SelectSound;
        SoundEffectSource.Play();
    }

    private void PlayDeselect()
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

    private void PlayPlace()
    {
        if (SoundEffectSource.isPlaying)
            SoundEffectSource.Stop();
        SoundEffectSource.clip = PlaceSound;
        SoundEffectSource.Play();
    }

    public void CardExpand(Card card)  //find card and player to expand
    {
        foreach (Player p in _playerScriptRefs)
        {
            foreach (Card c in p.GetHand())
            {
                if (c.name != card.name)
                    continue;
                p.CardExpansion(c, p);
                return;
            }
        }
    }

    public void CardUnexpand(Card card)  //find card and player to unexpand
    {
        foreach (Player p in _playerScriptRefs)
        {
            foreach (Card c in p.GetHand())
            {
                if (c.name != card.name)
                    continue;
                p.CardShrink(c, p);
                return;
            }
        }
    }

    public bool TryAddCard(Card cardA, Card boardCard, string keyword)
    {
        if (cardA.gameObject.GetComponent<GraphNode>() == null)
            cardA.gameObject.AddComponent<GraphNode>();
        if (cardA.DoesPropertyExist(keyword) && boardCard.DoesPropertyExist(keyword))
        {
            foreach (GameObject keyNode in _keywordNodes)
            {
                if (keyNode.transform.FindChild("Text").gameObject.GetComponent<Text>().text == keyword)
                {
                    // The keyword is already a node. Use it.
                    ConnectionManager.CreateConnection(cardA.gameObject.GetComponent<RectTransform>(), keyNode.GetComponent<RectTransform>());
                    return true;
                }
            }
            // Couldn't find the keyword in an existing node. Add it and connect both cards to it.
            GameObject newKeyNode = Instantiate(NodeOne); // Copy the template keyword node.
            newKeyNode.transform.FindChild("Text").gameObject.GetComponent<Text>().text = keyword; // Set the text of the new keyword node.
            _keywordNodes.Add(newKeyNode); // Add the keyword to the list of keyword nodes.
            // Connect both cards to the new keyword node.
            ConnectionManager.CreateConnection(boardCard.gameObject.GetComponent<RectTransform>(), newKeyNode.GetComponent<RectTransform>());
            ConnectionManager.CreateConnection(cardA.gameObject.GetComponent<RectTransform>(), newKeyNode.GetComponent<RectTransform>());
            return true;
        }
        return false;
    }
    public void SelectCardInHand(Card card)
    {
        bool cardFound = _playerScriptRefs[_currentPlayer].GetHand().Cast<Card>().Any(c => c.name == card.name && !c.IsOnBoard());
        // First, check if the card is in the current player's hand.
        if (cardFound) // Check for an already selected card in the player's hand.
        {
            foreach (Card c in _playerScriptRefs[_currentPlayer].GetHand())
            {
                if (!c.IsSelected() || c.IsOnBoard()) // Skip cards that aren't selected or are on the board.
                    continue;
                if (c.name == card.name) // Is the card already selected?
                {
                    card.SetIsSelected(false); // If so, deselect the card
                    PlayDeselect();
                    _isPlayerCardSelected = false;
                    return;
                }
                c.SetIsSelected(false); // Deselect the other card, then select this one.
                card.SetIsSelected(true);
                PlaySelect();
                return;
            }
            card.SetIsSelected(true);
            PlaySelect();
            _isPlayerCardSelected = true;
        }
    }

    public void SelectCardOnBoard(Card card)
    {
        bool cardFound = false;
        foreach (Player p in _playerScriptRefs)
        {
            foreach (Card c in p.GetHand())
            {
                if (c.IsOnBoard() && c.name == card.name)
                {
                    cardFound = true;
                    break;
                }
            }
            if (cardFound)
                break;
        }
        if (cardFound)
        {
            foreach (Player p in _playerScriptRefs)
            {
                foreach (Card c in p.GetHand())
                {
                    if (c.IsOnBoard() && c.IsSelected())
                    {
                        if (c.name == card.name)
                        {
                            c.SetIsSelected(false);
                            PlayDeselect();
                            _isBoardCardSelected = false;
                            return;
                        }
                        c.SetIsSelected(false);
                        card.SetIsSelected(true);
                        PlaySelect();
                        return;
                    }
                }
            }
            card.SetIsSelected(true);
            PlaySelect();
            _isBoardCardSelected = true;
        }
    }

}
