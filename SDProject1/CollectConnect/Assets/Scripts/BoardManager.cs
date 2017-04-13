using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    private const int MaxNumKeywordPicks = 5;
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
    public GameObject MasterKeywordList; // Includes the GridLayoutGroup and label.
    public static CardCollection Deck;
    public static bool IsCardExpanded;
    public AudioSource SoundEffectSource;
    public AudioClip SelectSound;
    public AudioClip DeselectSound;
    public AudioClip ExpandSound;
    public AudioClip PlaceSound;
    private List<string> _keywordList, _copyList; // _copyList contains ALL the keywords. _keywordList just contains the 20 for the game.
    private string _currentKeyword, _previousKeyword, _removedKeyword;
    private List<GameObject> _keywordNodes;
    public List<Player> _playerScriptRefs { get; private set; }
    private bool _isGameStarted;
    private bool _isFirstCardPlay;
    private bool _isPlayerCardSelected;
    private bool _isBoardCardSelected;
    private bool _isKeywordSelected;
    private bool _isGameListGenerated;
    public int CurrentPlayer;
    public bool IsTurnOver { get; private set; }
    private bool _playedTurn;
    public GameObject VetEnhance;
    public GameObject VetEnhanceShadow;
    public GameObject VetCard1;
    public GameObject VetCard2;
    public GameObject ConnectionBackground;
    public GameObject VetConnectionWordTxt;
    public GameObject VetText;
    private Card _copyCardLeft;
    private Card _copyCardRight;
    public List<bool> VetResultList;
    private List<string> _currentKeywordList = new List<string>(); // Contains the currently selected keywords. Don't load them into the _keywordList until we're ready to start the game.
    private bool _afterVet;
    private bool _isFirstListGen = true;
    private bool _hitVetBtn;
    private int[] _scoreboard;
    public int PlayerNumber;
    private static IDbConnection _dbconn;
    private TimerScript _ts;
    public Button PassBtnP1;
    public Button PassBtnP2;
    public Button PassBtnP3;
    public Button PassBtnP4;

    public GameObject VoteEnhance;
    public GameObject VoteEnhanceShadow;
    public List<int> VoteResultsList;
    public List<bool> CantVotePlayerList;
    public List<int> LegalVotePlayerList;
    private bool _afterVote;

    public GameObject InHandGlow;
    public GameObject OnBoardGlow;

    public static GamePhase CurrentPhase = GamePhase.PreGame;

    private int _numSelections; // The number of keywords the current player has picked during Research.
    private bool _aiThinkingDone;
    private PlayerSelection _playerSelection;
    private GridLayoutGroup _keywordGrid; // Contains the 20 keyword button GameObjects in the word bank.
    private List<Text> _graphicalKeyList = new List<Text>(); // Contains the list of Text components in the word bank buttons.
    public Button WordBankBtn;

    private void Awake()
    {
        _isGameStarted = false;
        IsDeckReady = false;
        _playedTurn = false;
        VetResultList = new List<bool>();
        VoteResultsList = new List<int>();
        CantVotePlayerList = new List<bool>();
        LegalVotePlayerList = new List<int>();
        _afterVet = false;
        _hitVetBtn = false;
        PlayerNumber = 0;
        _aiThinkingDone = false;

        for (int i = 0; i < 4; i++) //prefill lists
        {
            VetResultList.Add(true);
            VoteResultsList.Add(1);
            CantVotePlayerList.Add(false);
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
            _isGameListGenerated = false;
            _playerScriptRefs = new List<Player>();
            foreach (GameObject player in Players)
                _playerScriptRefs.Add(player.GetComponent<Player>());
            _playerScriptRefs[0].SetAiControl(true);    //set first player to be AI controlled

            switch (PlayerPrefs.GetInt("PlayerNumber")) //set other players to AI from player selection screen
            {
                case 1:
                    _playerScriptRefs[1].OnLeaveBtnHit();
                    _playerScriptRefs[3].OnLeaveBtnHit();
                    break;
                case 2:
                    _playerScriptRefs[3].OnLeaveBtnHit();
                    break;
            }
            _keywordGrid = MasterKeywordList.GetComponentInChildren<GridLayoutGroup>();
            _keywordList = new List<string>();
            _copyList = new List<string>();
            _scoreboard = new int[Players.Length];
            _keywordGrid.GetComponentsInChildren(_graphicalKeyList);
            Debug.Log("Size of graphical key list: " + _graphicalKeyList.Count);
            _keywordNodes = new List<GameObject>();
            _isFirstCardPlay = true;
            PassBtnP1.GetComponent<Button>().onClick.AddListener(PassBtnHit);
            PassBtnP2.GetComponent<Button>().onClick.AddListener(PassBtnHit);
            PassBtnP3.GetComponent<Button>().onClick.AddListener(PassBtnHit);
            PassBtnP4.GetComponent<Button>().onClick.AddListener(PassBtnHit);
            PassBtnP1.gameObject.SetActive(true);
            PassBtnP2.gameObject.SetActive(true);
            PassBtnP3.gameObject.SetActive(true);
            PassBtnP4.gameObject.SetActive(true);
            InHandGlow.GetComponent<Renderer>().enabled = false;
            OnBoardGlow.GetComponent<Renderer>().enabled = false;

            WordBankBtn.gameObject.SetActive(true);
            WordBankBtn.GetComponent<Button>().onClick.AddListener(EndKeywordPick);

            DisableVet();
            DisableVote();

            _afterVote = false;

            _ts = FindObjectOfType<TimerScript>();
            //_ts.stopTimer();

        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
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
        if (CurrentPhase != GamePhase.Research && !_isGameStarted)
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
                _copyList = _keywordList; // TODO Make sure this doesn't alias.
                _keywordList = PickSubset(_keywordList.Distinct().ToList());
                PopulateKeywords();


                if (_isFirstListGen)
                {
                    UpdateScoring();
                    _isFirstListGen = false;
                }

                //turn on all player block offs
                foreach (Player p in _playerScriptRefs)
                {
                    p.BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
                }

                CurrentPhase = GamePhase.Research;
                MasterKeywordList.SetActive(true);
                //_isGameStarted = true;
            }
        }
        else if (CurrentPhase == GamePhase.Research)
        {
            //turn off player's block off 
            _playerScriptRefs[CurrentPlayer].BlockOff.gameObject.GetComponent<Renderer>().enabled = false;

            if (_playerScriptRefs[CurrentPlayer].IsAiControlled)
            {
                if (_numSelections < 5)
                {
                    int[] indices = Enumerable.Range(0, _keywordList.Count).ToArray();
                    Debug.Log("Last value: " + indices[indices.Length - 1]);
                    Shuffle(ref indices);
                    foreach (int index in indices)
                    {
                        if (!_currentKeywordList.Contains(_keywordList[index])) // TODO I have seen this line throw an ArguementOutOfRangeException.
                        {
                            _currentKeyword = _keywordList[index];
                            _numSelections++;
                            break;
                        }
                    }
                }
                else
                {
                    EndKeywordPick();
                }
                Debug.Log("AI done picking...");
            }
            if (_previousKeyword != _currentKeyword && !_currentKeywordList.Contains(_currentKeyword))
            {
                _previousKeyword = _currentKeyword;
                _currentKeywordList.Add(_currentKeyword);
                foreach (Text t in _graphicalKeyList)
                {
                    if (string.IsNullOrEmpty(t.text))
                    {
                        t.text = _currentKeyword;
                        Debug.Log("Now displaying " + _currentKeyword);
                        t.GetComponentInParent<Image>().enabled = true;
                        t.GetComponentInParent<Button>().interactable = true;
                        Text t1 = t; // Prevent varied behavior (caused by different compiler versions)
                        t.GetComponentInParent<Button>().onClick.RemoveAllListeners();
                        t.GetComponentInParent<Button>().onClick.AddListener(() =>
                        {
                            if (_graphicalKeyList.IndexOf(t1) / MaxNumKeywordPicks == CurrentPlayer) // was this a keyword picked by the current player?
                            {
                                PlaySelect();
                                _removedKeyword = t1.text;
                                _currentKeywordList.Remove(t1.text);
                                t1.text = "";
                                _currentKeyword = "";
                                _previousKeyword = "";
                                t1.GetComponentInParent<Image>().enabled = false;
                                t1.GetComponentInParent<Button>().interactable = false;
                                _numSelections--;
                            }
                        });
                        break;
                    }
                }

            }
        }

        if (Deck.Size == 0)
            IsDeckReady = false;
        // Play turn like normal.
        if (_isFirstCardPlay)
        {
            //display first player's piece
            _playerScriptRefs[CurrentPlayer].PlayerPieceExpansion();

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

                    //first card played by AI
                    _playerScriptRefs[CurrentPlayer].ConnectionKeyword = "First Card Played";

                    //c.gameObject.AddComponent<NodeMovement>();
                    _isPlayerCardSelected = false;
                    _isFirstCardPlay = false;
                    IsTurnOver = true;
                }
            }
        }
        else if (AllCardsPlayed())
        {
            _isGameStarted = false;
            CurrentPhase = GamePhase.PostGame;
            // TODO Go to end game screen here.
            //collect player scores for end game screen
            PlayerPrefs.SetInt("Player1Score", _playerScriptRefs[0].Score);
            PlayerPrefs.SetInt("Player2Score", _playerScriptRefs[1].Score);
            PlayerPrefs.SetInt("Player3Score", _playerScriptRefs[2].Score);
            PlayerPrefs.SetInt("Player4Score", _playerScriptRefs[3].Score);

            SceneManager.LoadScene("EndGame");  //using for testing

        }
        else if (CurrentPhase != GamePhase.Voting)
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
                        //This is the card in the player's hand
                        cardA = c;
                    }
                }
            }

            //after tri-select, add cards to list for vetting (currentPlayer, card1, card2, keyword)
            //store connection in player
            _playerScriptRefs[CurrentPlayer].Card1 = cardA;
            _playerScriptRefs[CurrentPlayer].Card2 = cardB;
            _playerScriptRefs[CurrentPlayer].ConnectionKeyword = _currentKeyword;

            if (CurrentPhase != GamePhase.Vetting)  //if vetting hasn't started
            {
                Debug.Log("Starting vet setup.");
                VetSetUp();
                _ts.CancelInvoke();
            }
            else if (_playerScriptRefs[PlayerNumber].PlayerVetted) //if vetting has started and player hit yes/no btn
            {
                _playerScriptRefs[PlayerNumber].VetShrink();

                VetResultList[PlayerNumber] = _playerScriptRefs[PlayerNumber].VetResult; //pull player's result 
                PlayerNumber++;

                if (PlayerNumber < 4)
                {
                    if (_playerScriptRefs[PlayerNumber].IsAiControlled)  //AI Controlled
                    {
                        if (!_aiThinkingDone)
                        {
                            _playerScriptRefs[PlayerNumber].VetExpansion(); //individual player screens
                            StartCoroutine("VetAiDecision"); //start AI decision timer
                            _aiThinkingDone = true;
                        }
                        else if (_aiThinkingDone)
                        {
                            Debug.Log("Doing nothing"); //leave in here do not delete!! It's needed
                        }
                    }
                    else
                    {
                        if (PlayerNumber == CurrentPlayer) //if player who played connection is "Vetting"
                        {
                            //skip over them and vote yes
                            _playerScriptRefs[PlayerNumber].YesNoBtnHit = true;
                            _playerScriptRefs[PlayerNumber].VetResult = true;
                            _playerScriptRefs[PlayerNumber].PlayerVetted = true;
                        }
                        else
                        {
                            _playerScriptRefs[PlayerNumber].VetExpansion(); //individual player screens 
                        }
                    }
                }

                if (_playerScriptRefs[3].PlayerVetted)
                {
                    _hitVetBtn = false;
                    _playerScriptRefs[3].VetShrink();
                    Destroy(_copyCardLeft.gameObject); //delete clones
                    Destroy(_copyCardRight.gameObject);
                    DisableVet(); //shrink vet visuals
                    ToggleCardsOn();
                    _afterVet = true; //all done vetting
                    PlayerNumber = 0;
                    _aiThinkingDone = false; //reset
                }
            }

            if (_afterVet) //get the vet result, true for yes/valid, false for no/invalid
            {
                if (GetVetResult())
                {
                    //Add this connection with cardA and cardB and keyword
                    if (AddCardsToBoard(cardA, cardB, _currentKeyword))
                    {
                        ResetPassArray();
                        if (cardA != null)
                        {
                            Debug.Log(cardA);
                            Card.CardProperty prop = cardA.GetPropertyFromKeyword(_currentKeyword);
                            Debug.Log("Prop's value: " + prop.PropertyValue);
                            GetCurrentPlayer().IncreaseScore(cardA.GetPts(prop));
                            GetCurrentPlayer().PlayerScore.GetComponent<Text>().text = "" + GetCurrentPlayer().Score;
                            _keywordList.Remove(_currentKeyword);
                            PopulateKeywords();
                            IsTurnOver = true;
                            _hitVetBtn = false; //reset btn
                            _afterVet = false;
                            CurrentPhase = GamePhase.Playing;
                        }
                        else
                        {
                            Debug.Log("CardA is null. Null Pointer Exception.");
                        }
                        _currentKeyword = "";
                    }
                    else
                    {
                        _currentKeyword = "";
                    }
                }
                else
                {
                    //the players vetted against the connection. Reset the cards and pass.
                    ResetPassArray();
                    _playerScriptRefs[CurrentPlayer].Card1 = null;
                    _playerScriptRefs[CurrentPlayer].Card2 = null;
                    _playerScriptRefs[CurrentPlayer].ConnectionKeyword = "Vetted Against";
                    _currentKeyword = "";
                    if (cardA != null)
                    {
                        cardA.gameObject.GetComponent<Renderer>().enabled = false;
                        cardA.SetIsOnBoard(false);
                        cardA.SetIsSelected(false);

                        cardA.gameObject.layer = 2; //"destroyed"
                    }

                    //Destroy(cardA.gameObject);
                    CurrentPhase = GamePhase.Playing;
                    IsTurnOver = true;
                    _hitVetBtn = false; //reset btn
                    _afterVet = false;
                }
            }
        }
        else
        {
            //RUN VOTING
            _ts.CancelInvoke();
            if (_playerScriptRefs[PlayerNumber].PlayerVoted) //if player voted
            {
                _playerScriptRefs[PlayerNumber].PlayerVoteShrink();
                PlayerNumber++;

                if (PlayerNumber < 4)
                {
                    if (_playerScriptRefs[PlayerNumber].IsAiControlled) //if AI controlled
                    {
                        _playerScriptRefs[PlayerNumber].PlayerVoteExpansion();
                        StartCoroutine("VoteAiDecision"); //start AI decision timer
                        _aiThinkingDone = true;
                    }
                    else
                    {
                        //expand next player's voting
                        _playerScriptRefs[PlayerNumber].PlayerVoteExpansion();
                    }
                }

                if (_playerScriptRefs[3].PlayerVoted) //last player to vote
                {
                    _playerScriptRefs[3].PlayerVoteShrink();
                    GetVoteResult();    //distribute points
                    ToggleCardsOn();    //enable card movement
                    DisableVote();      //shrink voting screen
                    PlayerNumber = 0;
                    _aiThinkingDone = false; //reset

                    foreach (Player p in _playerScriptRefs)  //destroy main player cards
                    {
                        if (p.CopyCardLeft != null)
                        {
                            Destroy(p.CopyCardLeft.gameObject);
                        }

                        if (p.CopyCardRight != null)
                        {
                            Destroy(p.CopyCardRight.gameObject);
                        }
                    }

                    for (int i = 0; i < 4; i++) //rest listing
                    {
                        CantVotePlayerList[i] = false;
                    }

                    _afterVote = true;
                    //CurrentPlayer = 0;    //start round after voting (for late update)
                    CurrentPhase = GamePhase.Playing;
                }
            }
            _ts.InvokeRepeating("DecreaseTime", 1, 1);
        }
    }

    private static void Shuffle(ref int[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            int temp = arr[i];
            int newIndex = Random.Range(0, arr.Length);
            arr[i] = arr[newIndex];
            arr[newIndex] = temp;
        }
    }

    private static List<string> PickSubset(IList<string> wordList)
    {
        for (int i = 0; i < wordList.Count; i++)
        {
            int index = Random.Range(0, wordList.Count);
            string temp = wordList[index];
            wordList[index] = wordList[i];
            wordList[i] = temp;
        }
        List<string> subList = new List<string>();
        for (int i = 0; i < 20; i++)
        {
            int index = Random.Range(0, wordList.Count);
            subList.Add(wordList[index]);
            wordList.RemoveAt(index);
        }
        subList.Sort();
        return subList;
    }

    private bool AllCardsPlayed()
    {
        return (from p in _playerScriptRefs from Card c in p.GetHand() select c).All(c => c.IsOnBoard());
    }

    private void LateUpdate()
    {
        if (!IsTurnOver)
            return;
        if (!_isGameStarted)
            return;

        _ts.CancelInvoke();
        TimerScript.Timeleft = 120;
        _ts.CircleSlider.fillAmount = 1.0f;
        _ts.InvokeRepeating("DecreaseTime", 1, 1);

        if (CurrentPhase != GamePhase.Voting)
        {
            //shrink player piece
            _playerScriptRefs[CurrentPlayer].PlayerPieceShrink();

            //turn glows off
            OnBoardGlow.gameObject.GetComponent<Renderer>().enabled = false;
            InHandGlow.gameObject.GetComponent<Renderer>().enabled = false;

            CurrentPlayer++;

            if (_afterVote == true)
            {
                CurrentPlayer = 0;
                _afterVote = false;
            }
            
            if (CurrentPlayer == 4) //all players have played in round 
            {
                //Run voting
                VoteSetUp();
                CurrentPlayer--;
            }
            else
            {
                //shrink player piece
                _playerScriptRefs[CurrentPlayer].PlayerPieceExpansion();

                CurrentPlayer %= Players.Length;
            }

            switch (CurrentPlayer)
            {
                case 0:
                    PassBtnP1.gameObject.SetActive(true);
                    PassBtnP2.gameObject.SetActive(false);
                    PassBtnP3.gameObject.SetActive(false);
                    PassBtnP4.gameObject.SetActive(false);
                    KeywordContainerP2.gameObject.layer = 2;
                    KeywordContainerP3.gameObject.layer = 2;
                    KeywordContainerP4.gameObject.layer = 2;

                    //Turning on/off player blocking
                    _playerScriptRefs[0].BlockOff.gameObject.GetComponent<Renderer>().enabled = false;
                    _playerScriptRefs[1].BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
                    _playerScriptRefs[2].BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
                    _playerScriptRefs[3].BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
                    break;

                case 1:
                    PassBtnP1.gameObject.SetActive(false);
                    PassBtnP2.gameObject.SetActive(true);
                    PassBtnP3.gameObject.SetActive(false);
                    PassBtnP4.gameObject.SetActive(false);

                    if (_playerScriptRefs[CurrentPlayer].IsAiControlled)
                    {
                        PassBtnP2.gameObject.SetActive(false);
                    }

                    KeywordContainerP2.gameObject.layer = 5;
                    KeywordContainerP3.gameObject.layer = 2;
                    KeywordContainerP4.gameObject.layer = 2;

                    //Turning on/off player blocking
                    _playerScriptRefs[0].BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
                    _playerScriptRefs[1].BlockOff.gameObject.GetComponent<Renderer>().enabled = false;
                    _playerScriptRefs[2].BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
                    _playerScriptRefs[3].BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
                    break;

                case 2:
                    PassBtnP1.gameObject.SetActive(false);
                    PassBtnP2.gameObject.SetActive(false);
                    PassBtnP3.gameObject.SetActive(true);
                    PassBtnP4.gameObject.SetActive(false);
                    KeywordContainerP2.gameObject.layer = 2;
                    KeywordContainerP3.gameObject.layer = 5;
                    KeywordContainerP4.gameObject.layer = 2;

                    //Turning on/off player blocking
                    _playerScriptRefs[0].BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
                    _playerScriptRefs[1].BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
                    _playerScriptRefs[2].BlockOff.gameObject.GetComponent<Renderer>().enabled = false;
                    _playerScriptRefs[3].BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
                    break;

                case 3:
                    PassBtnP1.gameObject.SetActive(false);
                    PassBtnP2.gameObject.SetActive(false);
                    PassBtnP3.gameObject.SetActive(false);
                    PassBtnP4.gameObject.SetActive(true);

                    if (_playerScriptRefs[CurrentPlayer].IsAiControlled ||
                        CurrentPhase == GamePhase.Voting)
                    {
                        PassBtnP4.gameObject.SetActive(false);
                    }

                    KeywordContainerP2.gameObject.layer = 2;
                    KeywordContainerP3.gameObject.layer = 2;
                    KeywordContainerP4.gameObject.layer = 5;

                    //Turning on/off player blocking
                    _playerScriptRefs[0].BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
                    _playerScriptRefs[1].BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
                    _playerScriptRefs[2].BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
                    _playerScriptRefs[3].BlockOff.gameObject.GetComponent<Renderer>().enabled = false;
                    break;
            }

            if (CurrentPhase != GamePhase.Voting)
            {
                //PopulateKeywords();
                CurrentPhase = GamePhase.Playing;
                IsTurnOver = false;
                _isPlayerCardSelected = false;
                _isBoardCardSelected = false;
                _playedTurn = false;
            }

        }
    }

    private void PopulateKeywords()
    {
        Debug.Log("Populating keywords");

        //Clear and (re?)populate the word banks.
        if (CurrentPhase == GamePhase.Research || CurrentPhase == GamePhase.PreGame)
        {
            foreach (Player p in _playerScriptRefs)
            {
                _keywordList.AddRange(p.GetKeywords());
            }
            _keywordList = _keywordList.Distinct().ToList();
            _keywordList = PickSubset(_keywordList);
        }
        else if (!_isGameListGenerated && CurrentPhase == GamePhase.Playing)
        {
            _keywordList = _currentKeywordList;
            _keywordList.Sort();
            _isGameListGenerated = true;
        }
        else
        {
            _keywordList = _keywordList.Distinct().ToList();
        }
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
                if (CurrentPlayer == 0)
                {
                    if (CurrentPhase == GamePhase.Research)
                    {
                        if (_numSelections < 5 && _currentKeyword != go.GetComponentInChildren<Text>().text && !_currentKeywordList.Contains(go.GetComponentInChildren<Text>().text))
                        {
                            PlaySelect();
                            _currentKeyword = go.GetComponentInChildren<Text>().text;
                            _numSelections++;
                        }
                    }
                    else
                    {
                        PlaySelect();
                        _currentKeyword = go.GetComponentInChildren<Text>().text;
                    }
                }
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
                if (CurrentPlayer == 1) // TODO Change to 0 to test functionality until AI can pick 5 keywords.
                {
                    if (CurrentPhase == GamePhase.Research)
                    {
                        if (_numSelections < 5 && _currentKeyword != go.GetComponentInChildren<Text>().text && !_currentKeywordList.Contains(go.GetComponentInChildren<Text>().text))
                        {
                            PlaySelect();
                            Debug.Log("Setting current keyword to: " + go.GetComponentInChildren<Text>().text);
                            _currentKeyword = go.GetComponentInChildren<Text>().text;
                            _numSelections++;
                        }
                    }
                    else
                    {
                        PlaySelect();
                        _currentKeyword = go.GetComponentInChildren<Text>().text;
                    }
                }
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
                if (CurrentPlayer == 2) // TODO Change to 0 to test functionality until AI can pick 5 keywords.
                {
                    if (CurrentPhase == GamePhase.Research)
                    {
                        if (_numSelections < 5 && _currentKeyword != go.GetComponentInChildren<Text>().text && !_currentKeywordList.Contains(go.GetComponentInChildren<Text>().text))
                        {
                            PlaySelect();
                            _currentKeyword = go.GetComponentInChildren<Text>().text;
                            _numSelections++;
                        }
                    }
                    else
                    {
                        PlaySelect();
                        _currentKeyword = go.GetComponentInChildren<Text>().text;
                    }
                }
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
                if (CurrentPlayer == 3) // TODO Change to 0 to test functionality until AI can pick 5 keywords.
                {
                    if (CurrentPhase == GamePhase.Research)
                    {
                        if (_numSelections < 5 && _currentKeyword != go.GetComponentInChildren<Text>().text && !_currentKeywordList.Contains(go.GetComponentInChildren<Text>().text))
                        {
                            PlaySelect();
                            _currentKeyword = go.GetComponentInChildren<Text>().text;

                            _numSelections++;
                        }
                    }
                    else
                    {
                        PlaySelect();
                        _currentKeyword = go.GetComponentInChildren<Text>().text;
                    }
                }
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
                s = (string)rd["imageLocation"];
                cardComponent.SetImageLocation(s);

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

    public void PlaySelect()
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
        foreach (Card c in from Card c in p.GetHand() where c.name == card.name select c)
        {
            p.CardExpansion(c);
            PlayExpand();
            return;
        }
    }

    public void CardUnexpand(Card card) //find card and player to unexpand
    {
        Player p = FindOwningPlayer(card);
        foreach (Card c in from Card c in p.GetHand() where c.name == card.name select c)
        {
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
            ResetPassArray();
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
                                         boardCard.gameObject.transform.position) / 2;
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

    private static void ResetPassArray()
    {
        for (int i = 0; i < Player.PassArray.Length; i++)
        {
            Player.PassArray[i] = false;
        }
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
                connection.points[1].direction = ConnectionPoint.ConnectionDirection.East;
                break;
            case 2:
                connection.points[0].direction = ConnectionPoint.ConnectionDirection.North;
                connection.points[1].direction = ConnectionPoint.ConnectionDirection.South;
                break;
            case 3:
                connection.points[0].direction = ConnectionPoint.ConnectionDirection.North;
                connection.points[1].direction = ConnectionPoint.ConnectionDirection.West;
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
        return location / numConnections;
    }

    public void SelectCardInHand(Card card)
    {
        Debug.Log("Attempting to select hand card: " + card.name);
        bool cardFound =
            _playerScriptRefs[CurrentPlayer].GetHand().Cast<Card>().Any(c => c.name == card.name && !c.IsOnBoard());

        // First, check if the card is in the current player's hand.
        if (!cardFound)
        {
            Debug.Log("Card not found");
            return;
        }

        foreach (Card c in _playerScriptRefs[CurrentPlayer].GetHand())
        {
            if (!c.IsSelected() || c.IsOnBoard()) // Skip cards that aren't selected or are on the board.
            {
                continue;
            }

            if (c.name == card.name) // Is the card already selected?
            {
                card.SetIsSelected(false); // If so, deselect the card
                PlayDeselect();

                //turn glow off
                InHandGlowOff(card);

                _isPlayerCardSelected = false;
                return;
            }
            c.SetIsSelected(false); // Deselect the other card, then select this one.
            card.SetIsSelected(true);
            PlaySelect();

            //glow on
            InHandGlowOn(card);

            return;
        }
        card.SetIsSelected(true);
        PlaySelect();

        _isPlayerCardSelected = true;

        //glow on
        InHandGlowOn(card);

    }

    public void SelectCardOnBoard(Card card)
    {
        Debug.Log("Attempting to select board card: " + card.name);
        Player p = FindOwningPlayer(card);
        bool cardFound =
            p.GetHand().Cast<Card>().Any(c => c.IsOnBoard() && c.name == card.name);

        if (!cardFound)
            return;
        foreach (Player player in _playerScriptRefs)
        {
            foreach (Card c in player.GetHand())
            {
                if (!c.IsOnBoard() || !c.IsSelected())
                {
                    continue;
                }

                if (c.name == card.name)
                {
                    c.SetIsSelected(false);
                    PlayDeselect();
                    _isBoardCardSelected = false;

                    //turn glow off
                    OnBoardGlowOff(card);

                    return;
                }
                c.SetIsSelected(false);
                card.SetIsSelected(true);
                PlaySelect();

                //glow on
                OnBoardGlowOn(card);

                return;
            }
        }
        card.SetIsSelected(true);
        PlaySelect();
        _isBoardCardSelected = true;

        //glow on
        OnBoardGlowOn(card);
    }

    public void PassBtnHit() //player hit pass button
    {
        _playerScriptRefs[CurrentPlayer].Card1 = null;
        _playerScriptRefs[CurrentPlayer].Card2 = null;
        _playerScriptRefs[CurrentPlayer].ConnectionKeyword = "Passed";
        Player.PassArray[CurrentPlayer] = true;
        IsTurnOver = true;
        PlaySelect();
        foreach (Card c in from p in _playerScriptRefs from Card c in p.GetHand() where c.IsSelected() select c)
        {
            c.SetIsSelected(false); // Deselect any selected cards.
        }
        if (Player.PassArray.Any(b => !b))
        {
            return;
        }
        _isGameStarted = false;
        CurrentPhase = GamePhase.PostGame;
        // TODO Go to end game screen here.
        //collect player scores for end game screen
        PlayerPrefs.SetInt("Player1Score", _playerScriptRefs[0].Score);
        PlayerPrefs.SetInt("Player2Score", _playerScriptRefs[1].Score);
        PlayerPrefs.SetInt("Player3Score", _playerScriptRefs[2].Score);
        PlayerPrefs.SetInt("Player4Score", _playerScriptRefs[3].Score);
        SceneManager.LoadScene("EndGame");  //using for testing

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
            if (c.gameObject.layer == 0)
                coll.AddCards(c); // Add all cards that are on the board to the collection.
        }
        return coll;
    }

    public CardCollection GetPlayersUnplayedCards()
    {
        CardCollection coll = new CardCollection("Unplayed Cards");
        foreach (Card c in from Card c in _playerScriptRefs[CurrentPlayer].GetHand() where !c.IsOnBoard() select c)
        {
            coll.AddCards(c);
        }
        return coll;
    }

    public void SelectKeyword(Card.CardProperty prop)
    {
        if (_keywordList.Contains(prop.PropertyValue))
            _currentKeyword = prop.PropertyValue;
    }

    public bool GetIsStarted()
    {
        return _isGameStarted;
    }

    private void DisableVet() //disable vet screen
    {
        VetEnhance.gameObject.GetComponent<Renderer>().enabled = false;
        VetEnhanceShadow.gameObject.GetComponent<Renderer>().enabled = false;
        VetCard1.gameObject.GetComponent<Renderer>().enabled = false;
        VetCard2.gameObject.GetComponent<Renderer>().enabled = false;
        ConnectionBackground.gameObject.GetComponent<Renderer>().enabled = false;
        VetConnectionWordTxt.gameObject.GetComponent<Text>().enabled = false;
        VetText.gameObject.GetComponent<Text>().enabled = false;
        _playerScriptRefs[CurrentPlayer].VetPieceShrink();
    }

    private void EnableVet() //enable vet screen
    {
        VetEnhance.gameObject.GetComponent<Renderer>().enabled = true;
        VetEnhanceShadow.gameObject.GetComponent<Renderer>().enabled = true;
        ConnectionBackground.gameObject.GetComponent<Renderer>().enabled = true;
        VetConnectionWordTxt.gameObject.GetComponent<Text>().enabled = true;
        VetText.gameObject.GetComponent<Text>().enabled = true;
        _playerScriptRefs[CurrentPlayer].VetPieceExpansion();

        PassBtnP1.gameObject.SetActive(false);
        PassBtnP2.gameObject.SetActive(false);
        PassBtnP3.gameObject.SetActive(false);
        PassBtnP4.gameObject.SetActive(false);
    }

    private void VetSetUp()  //timer before vet screen pops up
    {
        Debug.Log("Enabling vet.");
        CurrentPhase = GamePhase.Vetting;
        for (int i = 0; i < 4; i++)
        {
            VetResultList[i] = true;    //reset result list
            _playerScriptRefs[i].PlayerVetted = false; //reset all player vetted
        }

        EnableVet();
        ToggleCardsOff();

        VetConnectionWordTxt.gameObject.GetComponent<Text>().text = _playerScriptRefs[CurrentPlayer].ConnectionKeyword; //store card connection for vet and vote 

        _copyCardLeft = Instantiate(_playerScriptRefs[CurrentPlayer].Card1, new Vector3(0f, 0f, 0f), Quaternion.identity);
        _copyCardLeft.transform.position = VetCard1.gameObject.transform.position;
        _copyCardLeft.transform.localScale = VetCard1.gameObject.GetComponent<Renderer>().bounds.extents;

        _copyCardRight = Instantiate(_playerScriptRefs[CurrentPlayer].Card2, new Vector3(0f, 0f, 0f), Quaternion.identity);
        _copyCardRight.transform.position = VetCard2.gameObject.transform.position;
        _copyCardRight.transform.localScale = VetCard2.gameObject.GetComponent<Renderer>().bounds.extents;

        PlayerNumber = 0;
        _hitVetBtn = true;

        VetResultList[0] = CheckConnection();
        Debug.Log("AI vetted " + VetResultList[0]);
        _playerScriptRefs[PlayerNumber].PlayerVetted = true; //first AI done 
        _playerScriptRefs[PlayerNumber].YesNoBtnHit = true;
        _playerScriptRefs[PlayerNumber].VetResult = VetResultList[0];

    }

    private IEnumerator VetAiDecision() //timer for AI to "think"
    {
        yield return new WaitForSeconds(4.0f);

        int rindex = Random.Range(0, 101);
        bool aiVet = rindex > 50;
        VetResultList[PlayerNumber] = aiVet;
        _playerScriptRefs[PlayerNumber].PlayerVetted = true;
        _aiThinkingDone = false; //reset
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
        return yesCount >= noCount;
    }

    private void ToggleCardsOff()
    {
        CardCollection playedCards = GetPlayedCards();
        foreach (Player p in _playerScriptRefs)
        {
            foreach (Card c in p.GetHand())
            {
                c.gameObject.layer = 2;
            }
        }
        foreach (Card c in playedCards)
        {
            c.gameObject.layer = 2;
        }
    }

    private void ToggleCardsOn()
    {
        CardCollection playedCards = GetPlayedCards();
        foreach (Player p in _playerScriptRefs)
        {
            foreach (Card c in p.GetHand())
            {
                c.gameObject.layer = 0;
            }
        }
        foreach (Card c in playedCards)
        {
            c.gameObject.layer = 0;
        }
    }

    private void DisableVote() //disable vote screen
    {
        VoteEnhance.gameObject.GetComponent<Renderer>().enabled = false;
        VoteEnhanceShadow.gameObject.GetComponent<Renderer>().enabled = false;
        LegalVotePlayerList.Clear();    //clear player voting list for AI voting

        foreach (Player p in _playerScriptRefs) //shrink main voting pieces 
        {
            p.VoteMainShrink();
        }
    }

    private void EnableVote() //enable vote screen
    {
        VoteEnhance.gameObject.GetComponent<Renderer>().enabled = true;
        VoteEnhanceShadow.gameObject.GetComponent<Renderer>().enabled = true;
        //Debug.Log("Hi, Mom!");
        foreach (Player p in _playerScriptRefs) //expand main voting pieces 
        {
            p.VoteMainExpansion();
        }
    }

    private void VoteSetUp()  //vote screen pops up
    {
        Debug.Log("Enabling voting.");
        CurrentPhase = GamePhase.Voting;
        EnableVote();
        ToggleCardsOff();

        for (int i = 0; i < 4; i++)
        {
            VoteResultsList[i] = 1; //reset result list
        }

        PlayerNumber = 0; //reset for voting below
        foreach (Player p in _playerScriptRefs)
        {
            p.VoteKeywordTxt.gameObject.GetComponent<Text>().text = p.ConnectionKeyword;

            if (p.ConnectionKeyword != "Passed" && p.ConnectionKeyword != "Vetted Against" && p.ConnectionKeyword != "First Card Played")
            {
                //don't display cards if player passed or vetted against or if first card
                //don't allow players to vote for this play
                p.VoteKeywordTxt.gameObject.GetComponent<Text>().text = p.ConnectionKeyword;

                p.CopyCardLeft = Instantiate(p.Card1, new Vector3(0f, 0f, 0f), Quaternion.identity);
                p.CopyCardLeft.transform.position = p.VoteCardLeft.gameObject.transform.position;
                p.CopyCardLeft.transform.localScale = Vector3.one;
                p.VoteCardLeft.gameObject.GetComponent<Renderer>().enabled = false;

                p.CopyCardRight = Instantiate(p.Card2, new Vector3(0f, 0f, 0f), Quaternion.identity);
                p.CopyCardRight.transform.position = p.VoteCardRight.gameObject.transform.position;
                p.CopyCardRight.transform.localScale = Vector3.one;
                p.VoteCardRight.gameObject.GetComponent<Renderer>().enabled = false;
                PlayerNumber++;
                LegalVotePlayerList.Add(PlayerNumber++); //add valid players to list for AI voting
                PlayerNumber--;
            }
            else
            {
                //disable voting buttons for those players
                CantVotePlayerList[PlayerNumber] = true;
                PlayerNumber++;

                //don't display card holders
                p.VoteCardLeft.gameObject.GetComponent<Renderer>().enabled = false;
                p.VoteCardRight.gameObject.GetComponent<Renderer>().enabled = false;
            }
        }
        PlayerNumber = 0; //reset

        //AI Voting
        _playerScriptRefs[PlayerNumber].PlayerVoteExpansion();
        VoteResultsList[0] = 1; //preset for AI
        _playerScriptRefs[PlayerNumber].PlayerVoted = true;
    }

    private void GetVoteResult()
    {
        Debug.Log("Getting vote results.");
        //VoteResultsList.Clear();
        foreach (Player p in _playerScriptRefs)
        {
            if (p.VotedForWho == 1)
            {
                _playerScriptRefs[0].IncreaseScore(1);
            }
            if (p.VotedForWho == 2)
            {
                _playerScriptRefs[1].IncreaseScore(1);
            }
            if (p.VotedForWho == 3)
            {
                _playerScriptRefs[2].IncreaseScore(1);
            }
            if (p.VotedForWho == 4)
            {
                _playerScriptRefs[3].IncreaseScore(1);
            }
        }
    }

    private bool CheckConnection()
    {
        List<Card.CardProperty> commonProps = _copyCardRight.FindCommonProperties(_copyCardLeft);
        return commonProps.Any(key => key.PropertyValue == _currentKeyword);
    }

    private struct KeywordFreq
    {
        public readonly string KeywordName;
        public int KeywordFreqs;
        public bool IsProcessed;

        public KeywordFreq(string keyword, int freq = 0)
        {
            KeywordName = keyword;
            KeywordFreqs = freq;
            IsProcessed = false;
        }

        public void IncreaseFreq()
        {
            KeywordFreqs++;
        }

        public void SetIsProcessed(bool processed)
        {
            IsProcessed = processed;
        }
    }

    private void UpdateScoring()
    {
        //Debug.Log("Blah");
        int tier1 = 2;
        int tier2 = 4;
        int tier3 = 6;
        List<KeywordFreq> scoring = new List<KeywordFreq>();

        foreach (string keywordValue in _keywordList)
        {
            KeywordFreq freq = new KeywordFreq(keywordValue);

            foreach (Player p in _playerScriptRefs)
            {
                foreach (Card c in p.PlayerHand)
                {
                    foreach (Card.CardProperty prop in c.PropertyList)
                    {
                        if (prop.PropertyValue == keywordValue)
                        {
                            freq.IncreaseFreq();
                        }
                    }
                }
            }
            scoring.Add(freq);
        }
        //check if card is in player hand not just in deck
        Debug.Log(scoring.Count);
        foreach (KeywordFreq freq in scoring)
        {
            //if (!freq.IsProcessed)
            // {
            foreach (Player p in _playerScriptRefs)
            {
                //check if card is in player hand not just in deck
                foreach (Card c in p.PlayerHand)
                {
                    // foreach (Card.CardProperty prop in c.PropertyList.Where(prop => prop.PropertyValue == freq.KeywordName))
                    //  {
                    for (int i = 0; i < c.PropertyList.Count; i++)
                    {
                        if (c.PropertyList[i].PropertyValue == freq.KeywordName)
                        {
                            Card.CardProperty temp = c.PropertyList[i];

                            Debug.Log("Checking " + freq.KeywordName);
                            //set cards pt values based on occurance
                            if (freq.KeywordFreqs >= 10)
                            {
                                //prop.SetPointValue(tier1);
                                temp.PointValue = tier1;
                            }
                            if (freq.KeywordFreqs < 10 && freq.KeywordFreqs > 4)
                            {
                                //prop.SetPointValue(tier2);
                                temp.PointValue = tier2;
                            }
                            if (freq.KeywordFreqs >= 0 && freq.KeywordFreqs <= 4)
                            {
                                //prop.SetPointValue(tier3);
                                temp.PointValue = tier3;
                            }
                            c.PropertyList[i] = temp;
                        }
                        //freq.SetIsProcessed(true);
                    }
                }
            }
            //}
            Debug.Log(freq.KeywordName + " " + freq.KeywordFreqs + " is processed!");
        }
    }

    private IEnumerator VoteAiDecision() //timer for AI to "think"
    {
        yield return new WaitForSeconds(4.0f);

        if (LegalVotePlayerList.Count != 0) //something to vote on
        {
            var random = new System.Random();
            int playerSelection = random.Next(LegalVotePlayerList.Count);
            _playerScriptRefs[PlayerNumber].VotedForWho = playerSelection;
        }
        _playerScriptRefs[PlayerNumber].PlayerVoted = true;    //move to next player
    }

    public void EndKeywordPick() // TODO This was originally the EndResearchPhase method. AI should be able to call this after it picks its keywords.
    {
        //turn on player's block off
        _playerScriptRefs[CurrentPlayer].BlockOff.gameObject.GetComponent<Renderer>().enabled = true;

        if (CurrentPlayer == Players.Length - 1 && _numSelections == MaxNumKeywordPicks) // If it's the last player's turn to pick & they chose 5 keywords...
        {
            PlaySelect();
            CurrentPhase = GamePhase.Playing; // Then let's start the game!
            _isGameStarted = true;
            MasterKeywordList.SetActive(false);
            _keywordList = _currentKeywordList;
            _numSelections = 0;
            _removedKeyword = "";
            _currentKeyword = "";
            _previousKeyword = "";
            WordBankBtn.gameObject.SetActive(false);
            PopulateKeywords();
            CurrentPlayer = 0;
            _ts.StartTimer(); // TODO add timer to Research stage.
        }
        else if (_numSelections == MaxNumKeywordPicks) // TODO AI will have to increment _numSelections for this to trigger.
                                                       // It's not the last player's turn, so let's check if they have 5 keywords.
        {
            PlaySelect();
            Debug.Log("Ding!");
            _removedKeyword = "";
            _currentKeyword = "";
            _previousKeyword = "";
            _keywordList = PickSubset(_copyList);
            PopulateKeywords();
            CurrentPlayer++; // Next player's turn to pick keywords.
            _numSelections = 0;
        }
    }

    private void InHandGlowOn(Card card)
    {
        RectTransform rt = (RectTransform)card.transform;

        if (rt.rect.width > rt.rect.height) //card is horizontal
        {
            transform.Rotate(90, 0, 0);
        }

        InHandGlow.GetComponent<Renderer>().enabled = true;
        InHandGlow.transform.position = card.gameObject.transform.position;
    }

    private void OnBoardGlowOn(Card card)
    {
        RectTransform rt = (RectTransform)card.transform;

        if (rt.rect.width > rt.rect.height) //card is horizontal
        {
            transform.Rotate(0, 0, 90);
        }

        OnBoardGlow.GetComponent<Renderer>().enabled = true;
        OnBoardGlow.transform.position = card.gameObject.transform.position;
    }

    private void InHandGlowOff(Card card)
    {
        RectTransform rt = (RectTransform)card.transform;

        if (rt.rect.width > rt.rect.height) //card was rotated, rotate back 
        {
            transform.Rotate(0, 0, -90);
        }

        InHandGlow.GetComponent<Renderer>().enabled = false;
    }

    private void OnBoardGlowOff(Card card)
    {
        RectTransform rt = (RectTransform)card.transform;

        if (rt.rect.width > rt.rect.height) //card was rotated, rotate back 
        {
            transform.Rotate(-90, 0, 0);
        }

        OnBoardGlow.GetComponent<Renderer>().enabled = false;
    }
}