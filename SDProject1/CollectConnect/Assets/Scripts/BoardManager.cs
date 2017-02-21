﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System.Xml;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;

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
    public int CurrentPlayer;
    private bool _isTurnOver;
    private bool _playedTurn;
    public GameObject VetEnhance;
    public GameObject VetCard1;
    public GameObject VetCard2;
    public GameObject ConnectionBackground;
    public GameObject VetConnectionWordTxt;
    public List<Card> PlayCardList;
    public List<string> PlayKeywordList;
    public Button VetBtnLeft;
    public Button VetBtnRight;
    private int _listCount;
    private Card _copyCardLeft;
    private Card _copyCardRight;
    public List<bool> VetResultList;
    private bool _afterVet;
    private bool _vetStartBool;
    private bool _hitVetBtn;
    private readonly List<Vector3> _gridPositions = new List<Vector3>();
    private int[] _scoreboard;
    private int _playerNumber;
    private static IDbConnection _dbconn;




    private void Awake()
    {
        _isGameStarted = false;
        _isTurnOver = false;
        IsDeckReady = false;
        _playedTurn = false;
        DisableVet();
        VetResultList = new List<bool>();
        _afterVet = false;
        _vetStartBool = false;
        _hitVetBtn = false;
        _playerNumber = 0;

        for (int i = 0; i < 4; i++) //prefill _verResult list
        {
            VetResultList.Add(true);
        }
    }

    private void Start()
    {
        if (Instance == null)
        {
            Debug.Log(Application.dataPath);
            string conn = "URI=file:" + Application.dataPath + "/CollectConnectDB.db"; //Path to database.
            _dbconn = (IDbConnection)new SqliteConnection(conn);
            _dbconn.Open(); //Open connection to the database.

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
            _playerScriptRefs[0].SetAiControl(true);
            _keywordList = new List<string>();
            _scoreboard = new int[Players.Length];
            _keywordNodes = new List<GameObject>();
            PlayCardList = new List<Card>();
            _isFirstCardPlay = true;
            VetBtnLeft.GetComponent<Button>().onClick.AddListener(VetBtnSelected);
            VetBtnRight.GetComponent<Button>().onClick.AddListener(VetBtnSelected);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    internal Player GetCurrentPlayer()
    {
        return _playerScriptRefs[CurrentPlayer];
    }

    // Update is called once per frame
    private void Update()
    {
        // First, check if all players have drawn their cards.
        // If so, then populate the players' word banks.
        if (!_isGameStarted)
        {
            bool allHandsDrawn = _playerScriptRefs.All(p => !p.IsDrawingCards);

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
            if (!_isPlayerCardSelected)
                return;
            foreach (Card c in _playerScriptRefs[CurrentPlayer].GetHand())
            {
                if (c.IsSelected())
                {
                    //ConnectionManager.CreateConnection(c.GetComponent<RectTransform>());
                    c.SetIsOnBoard(true);
                    c.SetIsSelected(false);

                    PlayPlace();
                    // c.gameObject.AddComponent<NodeMovement>();
                    _isPlayerCardSelected = false;
                    _isFirstCardPlay = false;
                    _isTurnOver = true;
                }
            }
        }
        else if (AllCardsPlayed())
        {
            _isGameStarted = false;
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

            //after tri-select, add cards to list for vetting (currentPlayer, card1, card2, keyword)
            PlayCardList.Add(cardA);
            PlayCardList.Add(cardB);
            PlayKeywordList.Add(CurrentPlayer.ToString());
            PlayKeywordList.Add(_currentKeyword);

            if (!_vetStartBool)
            {
                Debug.Log("Starting vet setup.");
                 StartCoroutine("VetSetUp");

                _vetStartBool = true;
            }

            if (_hitVetBtn == true) //rotate through vet y/n responses (yellow btn hit)
            {
                while (_playerScriptRefs[_playerNumber].playerVetted == true) //if blue y/n btn hit
                {
                    _playerScriptRefs[_playerNumber].VetShrink();
                    VetResultList[_playerNumber] = _playerScriptRefs[_playerNumber].VetResult; //pull player's result
                    _playerNumber++;

                    if (_playerNumber < 4) //if hit y/n button
                       {
                        _playerScriptRefs[_playerNumber].VetExpansion(); //orange screen
                        StartCoroutine("VetDecisionTimer", _playerScriptRefs[_playerNumber]);
                       }

                    if (_playerScriptRefs[3].playerVetted == true)
                    {
                        _hitVetBtn = false;
                        _playerScriptRefs[3].VetShrink();
                        Destroy(_copyCardLeft.gameObject); //delete clones
                        Destroy(_copyCardRight.gameObject);
                        DisableVet(); //shrink vet visuals
                        _hitVetBtn = false; //reset
                        _afterVet = true; //individual vetting done
                    }

                    //TODO: need to differ between AI and human players
                }
            }

            if (_afterVet)  //get the vet result, true for yes/valid, false for no/invalid
            {
                if (GetVetResult())
                {
                    //Add this connection with cardA and cardB and keyword
                    if (AddCardsToBoard(cardA, cardB, _currentKeyword))
                    {
                        if (cardA != null)
                        {
                            GetCurrentPlayer().IncreaseScore(cardA.GetPts(_currentKeyword));
                            GetCurrentPlayer().PlayerScore.GetComponent<Text>().text = "" + GetCurrentPlayer().Score;
                        }
                        else
                        {
                            Debug.Log("CardA is null. Null Pointer Exception.");
                        }
                        _currentKeyword = "";
                        _isTurnOver = true;
                        _vetStartBool = false;
                        _afterVet = false;
                    }
                    else
                    {
                        _currentKeyword = "";
                    }
                }
                else
                {
                    //the players vetted against the connection. Reset the cards and pass.
                    _currentKeyword = "";
                    cardA.gameObject.GetComponent<Renderer>().enabled = false;
                    PassBtnHit();
                }
            }

        }
    }

    private bool AllCardsPlayed()
    {
        return (from p in _playerScriptRefs from Card c in p.GetHand() select c).All(c => c.IsOnBoard());
    }

    private void LateUpdate()
    {
        if (!_isTurnOver)
            return;
        if (!_isGameStarted)
            return;
        TimerScript.Timeleft = 90;
        CurrentPlayer++;
        Debug.Log("player's turn" + CurrentPlayer);
        CurrentPlayer %= Players.Length;
        //TODO: Set keyword list to scroll Rect
        PopulateKeywords();
        _isTurnOver = false;
        _isPlayerCardSelected = false;
        _isBoardCardSelected = false;
        _playedTurn = false;
        _afterVet = false;
    }

    private void PopulateKeywords()
    {
        Debug.Log("Populating keywords");
        //Clear and (re?)populate the word banks.
        foreach (Player p in _playerScriptRefs)
        {
            _keywordList.AddRange(p.GetKeywords());
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
            btn.onClick.AddListener(() =>
            {
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
            btn.onClick.AddListener(() =>
            {
                Debug.Log(go.GetComponentInChildren<Text>().text + " Clicked!");
                _currentKeyword = go.GetComponentInChildren<Text>().text;
            });
            Vector3 scale = transform.localScale;
            scale.x = 1.0f;
            scale.y = 1.0f;
            scale.z = 1.0f;
            //go.transform.Rotate(0.0f, 0.0f, -90.0f);
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
            btn.onClick.AddListener(() =>
            {
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
            btn.onClick.AddListener(() =>
            {
                Debug.Log(go.GetComponentInChildren<Text>().text + " Clicked!");
                _currentKeyword = go.GetComponentInChildren<Text>().text;
            });
            Vector3 scale = transform.localScale;
            scale.x = 1.0f;
            scale.y = 1.0f;
            scale.z = 1.0f;
            //go.transform.Rotate(0.0f, 0.0f, 90.0f);
            go.transform.localScale = scale;
            go.SetActive(true);

            //Debug.Log(str);
        }
    }


    private static void BuildDeck()
    {

        IDbCommand dbcmd = _dbconn.CreateCommand();


        // Load the collections.
        List<string> collectionList = new List<string>();
        List<int> collectionIdList = new List<int>();
        try
        {
            const string sqlQuery = "SELECT * FROM sets"; // get id of last card inserted into cards table
            dbcmd.CommandText = sqlQuery;
            IDataReader rd = dbcmd.ExecuteReader();
            while (rd.Read())
            {
                collectionIdList.Add(rd.GetInt32(0));
                collectionList.Add(rd.GetString(1));
            }
            rd.Close();
            rd = null;

            collectionList = collectionList.Distinct().ToList(); // Remove any duplicates.
            collectionIdList = collectionIdList.Distinct().ToList(); // Remove any duplicates.
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }
        // Load the artifacts from each collection to make cards from them. Then add them to their respective lists.
        foreach (string col in collectionList)
        {
            int index = collectionList.IndexOf(col);
            int setId = collectionIdList[index];

            string sqlQuery = "SELECT * FROM cards INNER JOIN sets ON cards.setID = sets.setID WHERE cards.setID = " + setId;// get id of last card inserted into cards table
            dbcmd.CommandText = sqlQuery;
            IDataReader rd = dbcmd.ExecuteReader();
            while (rd.Read())
            {
                GameObject c = Instantiate(GameObject.Find("Card"));
                c.AddComponent<Card>();
                Card cardComponent = c.GetComponent<Card>();
                cardComponent.name = (string)rd["cardDisplayTitle"];
                cardComponent.AddProperty("Collection", col, "1");
                byte[] raw = (byte[])rd["cardDescription"];
                string s = System.Text.Encoding.UTF8.GetString(raw);
                cardComponent.SetExpInfo(s);
                int cardId = (int)(long)rd["cardID"];

                string keywordQuery = "SELECT * FROM attributes NATURAL JOIN parameters NATURAL JOIN cards NATURAL JOIN parameters_attributes WHERE cardID = " + cardId;
                IDbCommand kwCmd = _dbconn.CreateCommand();
                kwCmd.CommandText = keywordQuery;
                IDataReader kwReader = kwCmd.ExecuteReader();
                while (kwReader.Read())
                {
                    cardComponent.AddProperty((string)kwReader["parameter"], (string)kwReader["attribute"], (int)(long)kwReader["pointValue"] + "");
                }
                kwReader.Close();
                kwReader = null;
                Deck.AddCards(cardComponent);
            }
            rd.Close();
            rd = null;
        }
    }
    //populates db from text files
    //private static void BuildDeck()
    //{

    //    IDbCommand dbcmd = dbconn.CreateCommand();


    //    // Load the collections.
    //    List<string> collectionList = new List<string>();
    //    try
    //    {
    //        using (
    //            StreamReader reader =
    //                new StreamReader(new FileStream(Application.dataPath + "/TextFiles/Collections.txt",
    //                    FileMode.OpenOrCreate)))
    //        {
    //            while (!reader.EndOfStream)
    //            {
    //                collectionList.Add(reader.ReadLine());

    //            }
    //        }
    //        collectionList = collectionList.Distinct().ToList(); // Remove any duplicates.
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogException(e);
    //        throw;
    //    }
    //    // Load the artifacts from each collection to make cards from them. Then add them to their respective lists.
    //    foreach (string col in collectionList)
    //    {
    //        using (
    //            StreamReader reader =
    //                new StreamReader(
    //                    new FileStream(Application.dataPath + "/TextFiles/Collections/" + col + ".txt",
    //                        FileMode.OpenOrCreate)))
    //        {
    //            while (!reader.EndOfStream)
    //            {
    //                GameObject c = Instantiate(GameObject.Find("Card"));
    //                c.AddComponent<Card>();
    //                Card cardComponent = c.GetComponent<Card>();
    //                cardComponent.name = reader.ReadLine();
    //                cardComponent.AddProperty("Collection", col, "1");
    //                string s = reader.ReadLine();
    //                cardComponent.SetExpInfo(s);
    //                s = reader.ReadLine();
    //                while (s != @"|" && s != null)
    //                {
    //                    string[] separated = s.Split('\\');
    //                    cardComponent.AddProperty(separated[0], separated[1], separated[2]);
    //                    s = reader.ReadLine();
    //                }
    //                Deck.AddCards(cardComponent);

    //                string nonQ = "INSERT into cards (cardDisplayTitle,cardDescription) VALUES ('" + cardComponent.name + "',@param)";
    //                dbcmd.CommandType = CommandType.Text;
    //                dbcmd.CommandText = nonQ;
    //                dbcmd.Parameters.Add(new SqliteParameter("@param", cardComponent.GetExpInfo()));
    //                //string nonQ = "INSERT into cards (cardDisplayTitle,cardDescription) VALUES ('"+cardComponent.name+"','"+cardComponent.GetExpInfo()+"')";
    //                Debug.Log(dbcmd.CommandText);
    //                dbcmd.ExecuteNonQuery();

    //                string sqlQuery = "SELECT last_insert_rowid()";// get id of last card inserted into cards table
    //                dbcmd.CommandText = sqlQuery;
    //                IDataReader rd = dbcmd.ExecuteReader();
    //                int cardID = -1;
    //                while (rd.Read())
    //                {
    //                    cardID = rd.GetInt32(0);

    //                    Debug.Log("lastRow= " + cardID);
    //                }
    //                rd.Close();
    //                rd = null;

    //                foreach(Card.CardProperty prop in cardComponent.PropertyList)
    //                {
    //                    int paramID = -1;
    //                    int attributeID = -1;

    //                    string q = "INSERT into parameters (parameter) VALUES ('" + prop.PropertyName + "')";
    //                    dbcmd.CommandType = CommandType.Text;
    //                    dbcmd.CommandText = q;
    //                    dbcmd.ExecuteNonQuery();

    //                    sqlQuery = "SELECT last_insert_rowid()";// get id of last card inserted into cards table
    //                    dbcmd.CommandText = sqlQuery;
    //                    rd = dbcmd.ExecuteReader();
    //                    while (rd.Read())
    //                    {
    //                        paramID = rd.GetInt32(0);
    //                    }
    //                    rd.Close();
    //                    rd = null;

    //                    q = "INSERT into attributes (attribute) VALUES ('" + prop.PropertyValue + "')";
    //                    dbcmd.CommandType = CommandType.Text;
    //                    dbcmd.CommandText = q;
    //                    dbcmd.ExecuteNonQuery();

    //                    sqlQuery = "SELECT last_insert_rowid()";// get id of last card inserted into cards table
    //                    dbcmd.CommandText = sqlQuery;
    //                    rd = dbcmd.ExecuteReader();
    //                    while (rd.Read())
    //                    {
    //                        attributeID = rd.GetInt32(0);
    //                    }
    //                    rd.Close();
    //                    rd = null;

    //                    q = "INSERT into parameters_attributes (cardID,parameterID,attributeID,pointValue) VALUES (@param1,@param2,@param3,@param4)";
    //                    dbcmd.CommandType = CommandType.Text;
    //                    dbcmd.CommandText = q;
    //                    dbcmd.Parameters.Add(new SqliteParameter("@param1", cardID));
    //                    dbcmd.Parameters.Add(new SqliteParameter("@param2", paramID));
    //                    dbcmd.Parameters.Add(new SqliteParameter("@param3", attributeID));
    //                    dbcmd.Parameters.Add(new SqliteParameter("@param4", prop._pointValue));
    //                    dbcmd.ExecuteNonQuery();

    //                }

    //            }
    //        }
    //    }
    //    dbcmd.Dispose();
    //    dbcmd = null;
    //}

    // original
    //private static void BuildDeck()
    //{
    //    // Load the collections.
    //    List<string> collectionList = new List<string>();
    //    try
    //    {
    //        using (
    //            StreamReader reader =
    //                new StreamReader(new FileStream(Application.dataPath + "/TextFiles/Collections.txt",
    //                    FileMode.OpenOrCreate)))
    //        {
    //            while (!reader.EndOfStream)
    //            {
    //                collectionList.Add(reader.ReadLine());
    //            }
    //        }
    //        collectionList = collectionList.Distinct().ToList(); // Remove any duplicates.
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogException(e);
    //        throw;
    //    }
    //    // Load the artifacts from each collection to make cards from them. Then add them to their respective lists.
    //    foreach (string col in collectionList)
    //    {
    //        using (
    //            StreamReader reader =
    //                new StreamReader(
    //                    new FileStream(Application.dataPath + "/TextFiles/Collections/" + col + ".txt",
    //                        FileMode.OpenOrCreate)))
    //        {
    //            while (!reader.EndOfStream)
    //            {
    //                GameObject c = Instantiate(GameObject.Find("Card"));
    //                c.AddComponent<Card>();
    //                Card cardComponent = c.GetComponent<Card>();
    //                cardComponent.name = reader.ReadLine();
    //                cardComponent.AddProperty("Collection", col, "1");
    //                string s = reader.ReadLine();
    //                cardComponent.SetExpInfo(s);
    //                s = reader.ReadLine();
    //                while (s != @"|" && s != null)
    //                {
    //                    string[] separated = s.Split('\\');
    //                    cardComponent.AddProperty(separated[0], separated[1], separated[2]);
    //                    s = reader.ReadLine();
    //                }
    //                Deck.AddCards(cardComponent);
    //            }
    //        }
    //    }
    //}

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

    public void CardExpand(Card card) //find card and player to expand
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

    public void CardUnexpand(Card card) //find card and player to unexpand
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

    private bool AddCardsToBoard(Card cardA, Card boardCard, string keyword)
    {
        bool validPlay = true;
        if (cardA.gameObject.GetComponent<GraphNode>() == null)
            cardA.gameObject.AddComponent<GraphNode>();
        if (!cardA.DoesPropertyExist(keyword) || !boardCard.DoesPropertyExist(keyword))
            validPlay = false;
        foreach (GameObject keyNode in _keywordNodes)
        {
            if (keyNode.transform.FindChild("Text").gameObject.GetComponent<Text>().text != keyword)
                continue;
            // The keyword is already a node. Use it.
            ConnectionManager.CreateConnection(cardA.gameObject.GetComponent<RectTransform>(),
                keyNode.GetComponent<RectTransform>());
            keyNode.transform.position = CalculatePosition(keyNode);
            foreach (
                Connection connection in
                ConnectionManager.FindConnections(cardA.gameObject.GetComponent<RectTransform>()))
            {
                SetDirectionsAndColor(connection);
                connection.UpdateName();
                connection.UpdateCurve();
            }
            cardA.SetIsOnBoard(true);
            PlayPlace();
            cardA.SetIsSelected(false);
            boardCard.SetIsSelected(false);
            //cardA.gameObject.AddComponent<MobileNode>();
            return true;
        }
        // Couldn't find the keyword in an existing node. Add it and connect both cards to it.
        GameObject newKeyNode = Instantiate(NodeOne); // Copy the template keyword node.
        newKeyNode.transform.FindChild("Text").gameObject.GetComponent<Text>().text = keyword;
            // Set the text of the new keyword node.
        newKeyNode.name = keyword;
        _keywordNodes.Add(newKeyNode); // Add the keyword to the list of keyword nodes.
        // Connect both cards to the new keyword node.
        ConnectionManager.CreateConnection(boardCard.gameObject.GetComponent<RectTransform>(),
            newKeyNode.GetComponent<RectTransform>());
        ConnectionManager.CreateConnection(cardA.gameObject.GetComponent<RectTransform>(),
            newKeyNode.GetComponent<RectTransform>());
        newKeyNode.transform.position = (cardA.gameObject.transform.position +
                                         boardCard.gameObject.transform.position)/2;
        //newKeyNode.AddComponent<NodeMovement>();
        foreach (
            Connection connection in
            ConnectionManager.FindConnections(newKeyNode.gameObject.GetComponent<RectTransform>()))
        {
            SetDirectionsAndColor(connection);
            connection.UpdateName();
            connection.UpdateCurve();
        }
        cardA.SetIsOnBoard(true);
        PlayPlace();
        cardA.SetIsSelected(false);
        boardCard.SetIsSelected(false);
        //cardA.gameObject.AddComponent<MobileNode>();

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
                connection.points[0].direction = ConnectionPoint.ConnectionDirection.South;
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
        connection.points[0].color = Color.black;
        connection.points[1].color = Color.black;
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
        return location/numConnections;
    }

    public void SelectCardInHand(Card card)
    {
        Debug.Log("Attempting to select hand card: " + card.name);
        bool cardFound = _playerScriptRefs[CurrentPlayer].GetHand().Cast<Card>().Any(c => c.name == card.name && !c.IsOnBoard());
        // First, check if the card is in the current player's hand.
        if (!cardFound)
            return;
        foreach (Card c in _playerScriptRefs[CurrentPlayer].GetHand())
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
        Debug.Log("Attempting to select board card: " + card.name);
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

    public void PassBtnHit() //player hit pass button
    {
        _isTurnOver = true;
        foreach (Card c in from p in _playerScriptRefs from Card c in p.GetHand() where c.IsSelected() select c)
        {
            c.SetIsSelected(false); // Deselect any selected cards.
        }
        PlayKeywordList.Add(CurrentPlayer.ToString()); //add player passed  //TODO: FIX THIS!!!
        PlayKeywordList.Add("Pass");
        //LateUpdate();
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
        foreach (Card c in _playerScriptRefs[CurrentPlayer].GetHand())
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

    public bool GetIsTurnOver()
    {
        return _isTurnOver;
    }

    public bool GetIsStarted()
    {
        return _isGameStarted;
    }

    private void DisableVet() //disable vet screen
    {
        VetEnhance.gameObject.GetComponent<Renderer>().enabled = false;
        VetCard1.gameObject.GetComponent<Renderer>().enabled = false;
        VetCard2.gameObject.GetComponent<Renderer>().enabled = false;
        ConnectionBackground.gameObject.GetComponent<Renderer>().enabled = false;
        VetConnectionWordTxt.gameObject.GetComponent<Text>().enabled = false;
        VetBtnRight.gameObject.SetActive(false);
        VetBtnLeft.gameObject.SetActive(false);
    }

    private void EnableVet() //enable vet screen
    {
        VetEnhance.gameObject.GetComponent<Renderer>().enabled = true;
        ConnectionBackground.gameObject.GetComponent<Renderer>().enabled = true;
        VetConnectionWordTxt.gameObject.GetComponent<Text>().enabled = true;

        VetBtnRight.gameObject.SetActive(true);
        VetBtnLeft.gameObject.SetActive(true);
    }

    private IEnumerator VetSetUp()  //timer before vet screen pops up
    {
        yield return new WaitForSeconds(1.0f);
        Debug.Log("Enabling vet.");
        EnableVet();

        VetConnectionWordTxt.gameObject.GetComponent<Text>().text = PlayKeywordList[++_listCount];
        _listCount--;

        _copyCardLeft = Instantiate(PlayCardList[_listCount++], new Vector3(0f, 0f, 0f), Quaternion.identity);
        _copyCardLeft.transform.position = VetCard1.gameObject.transform.position;
        _copyCardLeft.transform.localScale = Vector3.one;

        _copyCardRight = Instantiate(PlayCardList[_listCount++], new Vector3(0f, 0f, 0f), Quaternion.identity);
        _copyCardRight.transform.position = VetCard2.gameObject.transform.position;
        _copyCardRight.transform.localScale = Vector3.one;
        Debug.Log("Starting vet timer.");
        StartCoroutine("VetTimer");  //start vet timer for vetting allowed
    }

    private IEnumerator VetTimer() //timer for players to select vet
    {

        yield return new WaitForSeconds(5.0f);

        if (!_hitVetBtn) //= no one hit vet btn -> next player's turn
        {
            Destroy(_copyCardLeft.gameObject); //delete clones
            Destroy(_copyCardRight.gameObject);
            Debug.Log("Disabling vet.");
            DisableVet(); //shrink vet visuals

            _afterVet = true;
        }
    }

    private void VetBtnSelected()  //someone hit yellow btn!
    {
        Debug.Log("Vet button selected.");
        VetBtnRight.gameObject.SetActive(false);    //disable vet btns
        VetBtnLeft.gameObject.SetActive(false);

        _hitVetBtn = true;
        _playerNumber = 0;

        //tResult[_playerNumber] = true; //reset all results to true

        VetResultList[0] = true;    //TODO: DONT HARDCODE FIRST AI
        _playerScriptRefs[_playerNumber].playerVetted = true; //first AI done
        _playerScriptRefs[_playerNumber].YesNoBtnHit = true;

    }

    private IEnumerator VetDecisionTimer(Player p)
    {
        yield return new WaitForSeconds(5.0f);

        if (!p.playerVetted)  //if player didn't vote
        {
            Debug.Log(p.name + " did not vote.");
            p.VetShrink();
            p.playerVetted = true;
            VetResultList[_playerNumber] = true;    //auto set to agree
        }

    }

    private bool GetVetResult()
    {
        Debug.Log("Getting vet results.");
        int yesCount = 0, noCount = 0;
        foreach (bool vet in VetResultList)
        {
            if (vet)
            {
                yesCount++;
            }
            else
            {
                noCount++;
            }
        }
        _hitVetBtn = false; //reset btn
        return yesCount >= noCount;
    }
}

