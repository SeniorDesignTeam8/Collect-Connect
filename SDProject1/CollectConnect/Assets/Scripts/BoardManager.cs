using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public static bool IsDeckReady { get; private set; }
    public static BoardManager Instance;
    public GameObject[] Players;
    public GameObject KeywordContainerP1;
    public GameObject KeywordContainerP2;
    public GameObject KeywordContainerP3;
    public GameObject KeywordContainerP4;
    public GameObject KeywordPrefab;
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
    private string _currentKeyword;
    private List<GameObject> _keywordNodes;
    private List<Player> _playerScriptRefs;
    private bool _isGameStarted;
    private bool _isFirstCardPlay;
    private bool _isPlayerCardSelected;
    private bool _isBoardCardSelected;
    private bool _isKeywordSelected;
    private int _currentPlayer;
    private bool _isTurnOver;

    //private readonly Color[] _playerColors =
    //{
    //    Color.red, Color.blue, Color.green, Color.yellow
    //};

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

    internal Player GetCurrentPlayer()
    {
        return _playerScriptRefs[_currentPlayer];
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
            Debug.Log("First card play");
            if (!_isPlayerCardSelected)
                return;
            foreach (Card c in _playerScriptRefs[_currentPlayer].GetHand())
            {
                if (c.IsSelected())
                {
                    //ConnectionManager.CreateConnection(c.GetComponent<RectTransform>());
                    c.SetIsOnBoard(true);
                    c.SetIsSelected(false);

                    PlayPlace();
                    _isPlayerCardSelected = false;
                    _isFirstCardPlay = false;
                    _isTurnOver = true;
                }
            }
        }
        else
        {
            //tri select check
            Card cardA = null, cardB = null;
            if (!_isBoardCardSelected || !_isPlayerCardSelected || string.IsNullOrEmpty(_currentKeyword))
                return;
            foreach (Player p in _playerScriptRefs)
            {
                foreach (Card c in p.GetHand())
                {
                    if (c.IsOnBoard() && c.IsSelected())
                    {
                        //This is the card on the game board
                        cardB = c;
                    }
                    else if (c.IsSelected())
                    {
                        //This is the card in the players hand
                        cardA = c;
                    }
                }
            }
            //Call tryaddcard with cardA and cardB
            if (TryAddCard(cardA, cardB, _currentKeyword))
            {
                //scoring
                Debug.Log("Try Add Card Worked.");
                _isTurnOver = true;
                _currentKeyword = "";
            }
            else
            {
                _currentKeyword = "";
                Debug.Log("Try Add Card Failed.");
            }
        }
    }

    private void LateUpdate()
    {
        if (!_isTurnOver)
            return;
        _currentPlayer++;
        _currentPlayer %= Players.Length;
        Debug.Log("Ending player's turn");
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
        _keywordList = _keywordList.Distinct().ToList();
        _keywordList.Sort();
        // clear the list
        // TODO Possibly combine KeywordContainers into an array?
        foreach (Transform child in KeywordContainerP1.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in KeywordContainerP2.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in KeywordContainerP3.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in KeywordContainerP4.transform)
        {
            Destroy(child.gameObject);
        }

        // TODO: Possibly combine these into one block? A lot of repetition.
        foreach (string str in _keywordList)
        {
            GameObject go = Instantiate(KeywordPrefab);
            go.GetComponentInChildren<Text>().text = str;
            go.transform.SetParent(KeywordContainerP1.transform);
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => {
                Debug.Log(go.GetComponentInChildren<Text>().text + " Clicked!");
                _currentKeyword = go.GetComponentInChildren<Text>().text;
            });
            Vector3 scale = transform.localScale;
            scale.x = 1.0f;
            scale.y = 1.0f;
            scale.z = 1.0f;

            go.transform.Rotate(0.0f, 0.0f, 180.0f);
            go.transform.localScale = scale;
            go.SetActive(true);

            //Debug.Log(str);
        }

        foreach (string str in _keywordList)
        {
            GameObject go = Instantiate(KeywordPrefab);
            go.GetComponentInChildren<Text>().text = str;
            go.transform.SetParent(KeywordContainerP2.transform);
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => {
                Debug.Log(go.GetComponentInChildren<Text>().text + " Clicked!");
                _currentKeyword = go.GetComponentInChildren<Text>().text;
            });
            Vector3 scale = transform.localScale;
            scale.x = 1.0f;
            scale.y = 1.0f;
            scale.z = 1.0f;
            go.transform.Rotate(0.0f, 0.0f, -90.0f);
            go.transform.localScale = scale;
            go.SetActive(true);

            //Debug.Log(str);
        }

        foreach (string str in _keywordList)
        {
            GameObject go = Instantiate(KeywordPrefab);
            go.GetComponentInChildren<Text>().text = str;
            go.transform.SetParent(KeywordContainerP3.transform);
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => {
                Debug.Log(go.GetComponentInChildren<Text>().text + " Clicked!");
                _currentKeyword = go.GetComponentInChildren<Text>().text;
            });
            Vector3 scale = transform.localScale;
            scale.x = 1.0f;
            scale.y = 1.0f;
            scale.z = 1.0f;
            go.transform.localScale = scale;
            go.SetActive(true);

            //Debug.Log(str);
        }

        foreach (string str in _keywordList)
        {
            GameObject go = Instantiate(KeywordPrefab);
            go.GetComponentInChildren<Text>().text = str;
            go.transform.SetParent(KeywordContainerP4.transform);
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => { Debug.Log(go.GetComponentInChildren<Text>().text + " Clicked!");
                _currentKeyword = go.GetComponentInChildren<Text>().text;
            });
            Vector3 scale = transform.localScale;
            scale.x = 1.0f;
            scale.y = 1.0f;
            scale.z = 1.0f;
            go.transform.Rotate(0.0f, 0.0f, 90.0f);
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
                    new StreamReader(new FileStream(Application.dataPath + "/TextFiles/Collections.txt",
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
                        new FileStream(Application.dataPath + "/TextFiles/Collections/" + col + ".txt",
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

    private void PlayExpand()
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
        Player p = FindOwningPlayer(card);
        foreach (Card c in p.GetHand())
        {
            if (c.name != card.name)
                continue;
            p.CardExpansion(c);
            PlayExpand();
            return;
        }
    }

    public void CardUnexpand(Card card)  //find card and player to unexpand
    {
        Player p = FindOwningPlayer(card);
        foreach (Card c in p.GetHand())
        {
            if (c.name != card.name)
                continue;
            p.CardShrink(c);
            PlayDeselect();
            return;
        }
    }

    private bool TryAddCard(Card cardA, Card boardCard, string keyword)
    {
        if (cardA.gameObject.GetComponent<GraphNode>() == null)
            cardA.gameObject.AddComponent<GraphNode>();
        if (!cardA.DoesPropertyExist(keyword) || !boardCard.DoesPropertyExist(keyword))
            return false;
        foreach (GameObject keyNode in _keywordNodes)
        {
            if (keyNode.transform.FindChild("Text").gameObject.GetComponent<Text>().text != keyword)
                continue;
            // The keyword is already a node. Use it.
            ConnectionManager.CreateConnection(cardA.gameObject.GetComponent<RectTransform>(), keyNode.GetComponent<RectTransform>());
            keyNode.transform.position = CalculatePosition(keyNode);
            foreach (Connection connection in ConnectionManager.FindConnections(cardA.gameObject.GetComponent<RectTransform>()))
            {
                SetDirectionsAndColor(connection);
                connection.UpdateName();
                connection.UpdateCurve();
            }
            cardA.SetIsOnBoard(true);
            PlayPlace();
            cardA.SetIsSelected(false);
            boardCard.SetIsSelected(false);
            return true;
        }
        // Couldn't find the keyword in an existing node. Add it and connect both cards to it.
        GameObject newKeyNode = Instantiate(NodeOne); // Copy the template keyword node.
        newKeyNode.transform.FindChild("Text").gameObject.GetComponent<Text>().text = keyword; // Set the text of the new keyword node.
        newKeyNode.name = keyword;
        _keywordNodes.Add(newKeyNode); // Add the keyword to the list of keyword nodes.
        // Connect both cards to the new keyword node.
        ConnectionManager.CreateConnection(boardCard.gameObject.GetComponent<RectTransform>(), newKeyNode.GetComponent<RectTransform>());
        ConnectionManager.CreateConnection(cardA.gameObject.GetComponent<RectTransform>(), newKeyNode.GetComponent<RectTransform>());
        newKeyNode.transform.position = (cardA.gameObject.transform.position +
                                         boardCard.gameObject.transform.position) / 2;
        foreach (Connection connection in ConnectionManager.FindConnections(newKeyNode.gameObject.GetComponent<RectTransform>()))
        {
            SetDirectionsAndColor(connection);
            connection.UpdateName();
            connection.UpdateCurve();
        }
        cardA.SetIsOnBoard(true);
        PlayPlace();
        cardA.SetIsSelected(false);
        boardCard.SetIsSelected(false);
        return true;
    }

    private void SetDirectionsAndColor(Connection connection)
    {
        Card c = connection.target[0].gameObject.GetComponent<Card>();
        Player p = FindOwningPlayer(c);
        int playerIndex = _playerScriptRefs.IndexOf(p);

        switch (playerIndex)
        {
            case 0:
                connection.points[0].direction = ConnectionPoint.ConnectionDirection.North;
                connection.points[1].direction = ConnectionPoint.ConnectionDirection.North;
                break;
            case 1:
                connection.points[0].direction = ConnectionPoint.ConnectionDirection.North;
                connection.points[1].direction = ConnectionPoint.ConnectionDirection.West;
                break;
            case 2:
                connection.points[0].direction = ConnectionPoint.ConnectionDirection.North;
                connection.points[1].direction = ConnectionPoint.ConnectionDirection.South;
                break;
            case 3:
                connection.points[0].direction = ConnectionPoint.ConnectionDirection.North;
                connection.points[1].direction = ConnectionPoint.ConnectionDirection.East;
                break;
            default:
                return; // Should never reach here.
        }
        //connection.points[0].color = _playerColors[playerIndex];
        //connection.points[1].color = _playerColors[playerIndex];
    }

    private static Vector3 CalculatePosition(GameObject keyNode)
    {
        int numConnections = 0;
        Vector3 location = Vector3.zero;
        foreach (Connection conn in ConnectionManager.FindConnections(keyNode.GetComponent<RectTransform>()))
        {
            if (conn.target[0].gameObject.name == keyNode.name) // Check one end of the connection.
            {
                location += conn.target[1].gameObject.transform.position;
            }
            else
            {
                location += conn.target[0].gameObject.transform.position;
            }
            numConnections++;
        }
        return location / numConnections;
    }

    public void SelectCardInHand(Card card)
    {
        bool cardFound = false;
        foreach (Card c in _playerScriptRefs[_currentPlayer].GetHand())
        {
            if (c.name == card.name && !c.IsOnBoard())
            {
                cardFound = true;
                break;
            }
        }
        // First, check if the card is in the current player's hand.
        if (!cardFound)
            return;
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

    public void SelectCardOnBoard(Card card)
    {
        Player p = FindOwningPlayer(card);
        bool cardFound = p.GetHand().Cast<Card>().Any(c => c.IsOnBoard() && c.name == card.name);
        if (!cardFound)
            return;
        foreach (Player player in _playerScriptRefs)
        {
            foreach (Card c in player.GetHand())
            {
                if (!c.IsOnBoard() || !c.IsSelected())
                    continue;
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
        card.SetIsSelected(true);
        PlaySelect();
        _isBoardCardSelected = true;
    }

    public void PassBtnHit()  //player hit pass button
    {
        _isTurnOver = true;
        LateUpdate();
    }

    public Player FindOwningPlayer(Card card)
    {
        return _playerScriptRefs.FirstOrDefault(p => p.GetHand().Cast<Card>().Any(c => c.name == card.name));
    }

    public CardCollection GetPlayedCards()
    {
        CardCollection coll = new CardCollection("Board Cards");
        foreach (Card c in _playerScriptRefs.SelectMany(p => (from Card c in p.GetHand() where c.IsOnBoard() select c)))
        {
            coll.AddCards(c); // Add all cards that are on the board to the collection.
        }
        return coll;
    }

    public CardCollection GetPlayersUnplayedCards()
    {
        CardCollection coll = new CardCollection("Unplayed Cards");
        foreach (Card c in _playerScriptRefs[_currentPlayer].GetHand())
        {
            if (!c.IsOnBoard())
                coll.AddCards(c);
        }
        return coll;
    }

    public void SelectKeyword(Card.CardProperty prop)
    {
        if (_keywordList.Contains(prop.PropertyValue))
            _currentKeyword = prop.PropertyValue;
    }
}
