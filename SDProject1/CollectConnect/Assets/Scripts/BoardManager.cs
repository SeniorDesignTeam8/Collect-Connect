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
	public int GridSpacing = 1;


    private const int MaxNumKeywordPicks = 5;
    public static bool IsDeckReady { get; private set; }
    public static BoardManager Instance;
    public GameObject[] Players;
    public GameObject KeywordContainerP2;
    public GameObject KeywordPrefab;
    public GameObject NodeOne;
    public int Columns = 8, Rows = 8;
	public int cardIndex;
    public GameObject MasterKeywordList; // Includes the GridLayoutGroup and label.
    public static CardCollection Deck;
    public static bool IsCardExpanded;

    public List<string> _keywordList, _copyList; // _copyList contains ALL the keywords. _keywordList just contains the 20 for the game.
    public string _currentKeyword, _previousKeyword, _removedKeyword;
	public GameObject _currentKeywordButton;
    public List<GameObject> _keywordNodes;
	public Transform BoardGrid;
    public List<Player> _playerScriptRefs { get; private set; }
    public bool _isGameStarted;
    private bool _isFirstCardPlay;
    private bool _isPlayerCardSelected;
    private bool _isBoardCardSelected;
    private bool _isKeywordSelected;
    private bool _isGameListGenerated;
    public int CurrentPlayer;
    public bool IsTurnOver { get; private set; }
    public object WaitForTurn { get; private set; }

    private bool _playedTurn;
    public GameObject VetCard1;
    public GameObject VetCard2;
    private Card _copyCardLeft;
    private Card _copyCardRight;
    public List<bool> VetResultList;
    private List<string> _currentKeywordList = new List<string>(); // Contains the currently selected keywords. Don't load them into the _keywordList until we're ready to start the game.
    private bool AfterVet;
    private bool _isFirstListGen = true;
    private bool _hitVetBtn;
    private int[] _scoreboard;
    public int PlayerNumber;
    public static IDbConnection _dbconn;
    private TimerScript _ts;
    public Button PassBtnP1;
    public Button PassBtnP2;
    public Button PassBtnP3;
    public Button PassBtnP4;

    public List<int> VoteResultsList;
    public List<bool> CantVotePlayerList;
    public List<int> LegalVotePlayerList;
    private bool _afterVote;

    public GameObject InHandGlow;
    public GameObject OnBoardGlow;

    public static GamePhase CurrentPhase = GamePhase.PreGame;
	public static PlayerTurnPhase CurrPlayer = PlayerTurnPhase.Player1;
    private int _numSelections; // The number of keywords the current player has picked during Research.
    private bool _aiThinkingDone;
    private PlayerSelection _playerSelection;
    private GridLayoutGroup _keywordGrid; // Contains the 20 keyword button GameObjects in the word bank.
    private List<Text> _graphicalKeyList = new List<Text>(); // Contains the list of Text components in the word bank buttons.
    public Button WordBankBtn;
	public GameObject prevKeyNode;
    public GameObject test;

	public const float timeOut = 300.0f; //5 minutes
	public float countdown;
	public bool countOver;
	public GameObject resetBoard;
	public bool canCheckPlayers;

	public List<Text> KeywordList;
    public GameObject mostRecentCard1;
    public GameObject mostRecentCard2;
    public GameObject mostRecentKeyword;
    public GameObject mostRecentPlayer;

    public GameObject loadingScreen;
    public int keyWordCount;
    public List<String> playedKeywords;
    public SoundMan sound;

    private void Awake()
    {
        _isGameStarted = false;
        IsDeckReady = false;
        _playedTurn = false;
        CantVotePlayerList = new List<bool>();
        LegalVotePlayerList = new List<int>();
        AfterVet = false;
        _hitVetBtn = false;
        PlayerNumber = 0;
        _aiThinkingDone = false;
        countdown = timeOut;
        countOver = false;
        canCheckPlayers = false;

#if !UNITY_EDITOR
        if (Debug.isDebugBuild)
        {
            Random.InitState(42);
            CardCollection.SetSeed(42);
        }
#endif
        for (int i = 0; i < 4; i++) //prefill lists
        {
            VetResultList.Add(true);
            VoteResultsList.Add(1);
            CantVotePlayerList.Add(false);
        }

    }



    private void Start()
    {
        keyWordCount = 0;
        playedKeywords.Clear();

        if (Instance == null)
        {
            //Debug.Log(Application.dataPath);
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
				_playerScriptRefs [2].LeaveGameBtn.gameObject.SetActive (false);
                _playerScriptRefs[3].OnLeaveBtnHit();
                    break;
                case 2:
                    _playerScriptRefs[3].OnLeaveBtnHit();
                    break;
            }

			canCheckPlayers = true;
            _keywordList = new List<string>();
            _copyList = new List<string>();
            _scoreboard = new int[Players.Length];
            //Debug.Log("Size of graphical key list: " + _graphicalKeyList.Count);
            _keywordNodes = new List<GameObject>();
            _isFirstCardPlay = true;
            PassBtnP1.GetComponent<Button>().onClick.AddListener(PassBtnHit);
            PassBtnP2.GetComponent<Button>().onClick.AddListener(PassBtnHit);
            PassBtnP3.GetComponent<Button>().onClick.AddListener(PassBtnHit);
            PassBtnP4.GetComponent<Button>().onClick.AddListener(PassBtnHit);
           
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
		if (Input.anyKeyDown) 
		{
			countdown = timeOut;
		}
		else if (countOver == false)
		{
			countdown -= Time.deltaTime;
			//Debug.Log ("Countdown: " + countdown.ToString ());
			if (countdown <= 0.0f)
			{
				countOver = true;
				resetBoard.gameObject.transform.position = new Vector3 (0,1,-10);
				StartCoroutine (RestartingGame ());
			}
		}

        //quit application 
        if (Input.GetKeyDown((KeyCode.Escape)))
        {
            Application.Quit();
        }

		//Checks to see if Default Player can leave the game
		if (canCheckPlayers)
		{
			CanDefaultPlayerLeave ();
		}
        
        // First, check if all players have drawn their cards.
        // If so, then populate the players' word banks.
        if (CurrentPhase != GamePhase.Research && !_isGameStarted)
        {
            bool allHandsDrawn = _playerScriptRefs.All(p => !p.IsDrawingCards);

            if (allHandsDrawn)
            {
                _keywordList.Clear(); //MIGHT NEED TO ACTUALLY REPOPULATE 
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
                MasterKeywordList.SetActive(false); //Set to true if we want keyword phase back
                _isGameStarted = true;
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
                    //Debug.Log("Last value: " + indices[indices.Length - 1]);
                    Shuffle(ref indices);
                    foreach (int index in indices)
                    {
                       // if (!_currentKeywordList.Contains(_keywordList[index])) // TODO I have seen this line throw an ArguementOutOfRangeException.
                        {
                         //   _currentKeyword = _keywordList[index];
                         //   _numSelections++;
                         //   break;
                        }
                    }
                }
                else
                {
                    EndKeywordPick();
                }
                //Debug.Log("AI done picking...");
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
                        //Debug.Log("Now displaying " + _currentKeyword);
                        t.GetComponentInParent<Image>().enabled = true;
                        t.GetComponentInParent<Button>().interactable = true;
                        Text t1 = t; // Prevent varied behavior (caused by different compiler versions)
                        t.GetComponentInParent<Button>().onClick.RemoveAllListeners();
                        t.GetComponentInParent<Button>().onClick.AddListener(() =>
                        {
                            if (_graphicalKeyList.IndexOf(t1) / MaxNumKeywordPicks == CurrentPlayer) // was this a keyword picked by the current player?
                            {
                                sound.PlaySelect();
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

                    sound.PlayPlace();

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
                //Debug.Log("Starting vet setup.");
                VetSetUp();
                _ts.CancelInvoke();
                _ts._isPaused = true;
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
                            //Debug.Log("Doing nothing"); //leave in here do not delete!! It's needed
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
                    AfterVet = true; //all done vetting
                    PlayerNumber = 0;
                    _aiThinkingDone = false; //reset
                }
            }

            if (AfterVet) //get the vet result, true for yes/valid, false for no/invalid
            {
                if (GetVetResult())
                {
                    //Add this connection with cardA and cardB and keyword
                    if (AddCardsToBoard(cardA, cardB, _currentKeyword))
                    {
                        ResetPassArray();
                        if (cardA != null)
                        {
                            //Debug.Log(cardA);
                            Card.CardProperty prop = cardA.GetPropertyFromKeyword(_currentKeyword);
                            //Debug.Log("Prop's value: " + prop.PropertyValue);
                            GetCurrentPlayer().IncreaseScore(cardA.GetPts(prop));
                            GetCurrentPlayer().PlayerScore.GetComponent<Text>().text = "" + GetCurrentPlayer().Score;
                            //_keywordList.Remove(_currentKeyword);

                            PopulateKeywords();
                            IsTurnOver = true;
                            _hitVetBtn = false; //reset btn
                            AfterVet = false;
                            CurrentPhase = GamePhase.Playing;

							//ColorKeywordButton ();

                        }
                        //else
                        //{
                        //    Debug.Log("CardA is null. Null Pointer Exception.");
                        //}
                        _currentKeyword = "";
						//GetCurrentPlayer ().HandSize--; //Enables it so the cards can be redealt
						//GetCurrentPlayer ().RedealCards ();
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
                    AfterVet = false;
                }
            }
        }
        else
        {
            //RUN VOTING
            _ts.CancelInvoke();
            _ts._isPaused = true;
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
            _ts._isPaused = false;
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
//        for (int i = 0; i < wordList.Count; i++)
//        {
//            int index = Random.Range(0, wordList.Count);
//            string temp = wordList[index];
//            wordList[index] = wordList[i];
//            wordList[i] = temp;
//        }
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
        _ts._isPaused = true;
        TimerScript.Timeleft = 120;
        _ts.CircleSlider.fillAmount = 1.0f;
        _ts.InvokeRepeating("DecreaseTime", 1, 1);
        _ts._isPaused = false;

        if (CurrentPhase != GamePhase.Voting)
        {
            //shrink player piece
            _playerScriptRefs[CurrentPlayer].PlayerPieceShrink();

            //turn glows off
            OnBoardGlow.gameObject.GetComponent<Renderer>().enabled = false;
            InHandGlow.gameObject.GetComponent<Renderer>().enabled = false;

            CurrentPlayer++;

            if (_afterVote)
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
        //Debug.Log("Populating keywords");

        //Clear and (re?)populate the word banks.
        if (CurrentPhase == GamePhase.Research || CurrentPhase == GamePhase.PreGame)
        {
            foreach (Player p in _playerScriptRefs)
            {
                _keywordList.AddRange(p.GetKeywords());
            }

            _keywordList = _keywordList.Distinct().ToList();
            _keywordList = PickSubset(_keywordList);

//			int i = 0;
//			foreach (string t in _keywordList) //putting the keywords into the wordbank list
//			{
//				_graphicalKeyList [i].text = t.ToString();
//				i++;
//			}
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

        foreach (Transform child in KeywordContainerP2.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (string str in _keywordList)
        {
			SetupKeywordButton (KeywordContainerP2, str, 1);
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

			string sqlQuery = "SELECT * FROM cards INNER JOIN sets ON cards.setID = sets.setID WHERE cards.setID = " + setId + " AND cards.Location<>''";// get id of last card inserted into cards table
			//Debug.Log(sqlQuery);
			dbcmd.CommandText = sqlQuery;
			IDataReader rd = dbcmd.ExecuteReader();
			while (rd.Read())
			{
				GameObject c = Instantiate(GameObject.Find("Card"));
				c.AddComponent<Card>();
				Card cardComponent = c.GetComponent<Card>();
				cardComponent.name = (string)rd["cardDisplayTitle"];
				//cardComponent.AddProperty("Collection", col, "1");
				string raw = (string)rd["cardDescription"];
				string s = raw;
				cardComponent.SetExpInfo(s);
				int cardId = (int)(long)rd["cardID"];
				s = (string)rd["imageFileName"];
				cardComponent.SetID (cardId);
				cardComponent.SetImageLocation(s);
                raw = (string)rd["Location"];
                string loc = raw;
                cardComponent.SetSourceLoc(loc);

                string keywordQuery = "SELECT cat.category, param.parameter, attr.attribute FROM cards c NATURAL JOIN categories_parameters_attributes cpa LEFT OUTER JOIN categories cat ON cpa.categoryID = cat.categoryID LEFT OUTER JOIN parameters param ON cpa.parameterID = param.parameterID LEFT OUTER JOIN attributes attr ON cpa.attributeID = attr.attributeID WHERE cardID = " + cardId + " ORDER BY CATEGORY, ATTRIBUTE, PARAMETER" ;
				//Debug.Log (keywordQuery);
				IDbCommand kwCmd = _dbconn.CreateCommand();
				kwCmd.CommandText = keywordQuery;
				IDataReader kwReader = kwCmd.ExecuteReader();
				//Debug.Log(kwCmd.ToString() + "    " + kwReader.ToString() + "    " + (int)(long)kwReader["cpa.pointValue"]);
				while (kwReader.Read())
				{
                    //string attr;
                    string rawString = (string)kwReader["category"];
                    string cat = rawString;

                    rawString = (string)kwReader["parameter"];
                    string  param = rawString;

                    //Debug.Log(cardId.ToString());

                    //  rawString = (string)kwReader["attribute"];
                    // attr = rawString;

                    //Debug.Log ("Reading Data from Card: " + cardId);
                    //Debug.Log (kwReader ["parameter"] + "    " + kwReader ["attribute"] );
                    //if((string)kwReader ["parameter"] != "NULL" && (string)kwReader ["attribute"]  != "NULL")

                    //cardComponent.AddProperty((string)kwReader["category"],(string)kwReader["parameter"],  /*(string)kwReader["attribute"], /*(int)(long)kwReader["cpa.pointValue"]+*/  "0");
                    string attr;

                    if (cat == "Concept")
                    {
                        cardComponent.AddProperty(cat,param, "0");
                    }
                    else if(cat == "Location")
                    {
                        rawString = (string)kwReader["attribute"];
                        attr = rawString;
                        
                        cardComponent.SetLocation(attr + ": "+param + "\n");
                    }
                    else if (cat == "Person")
                    {
                        rawString = (string)kwReader["attribute"];
                        attr = rawString;
                        cardComponent.SetContributor(attr + ": " + param + "\n");
                    }
                    else if (cat == "Medium")
                    {
                       
                        cardComponent.SetMedium(cat + ": " + param + "\n");
                    }
                    else if(cat == "Year")
                    {
                      if (kwReader["attribute"] != DBNull.Value)
                        {
                            rawString = (string)kwReader["attribute"];
                            attr = rawString;
                            cardComponent.SetYear(attr + ": " + param + "\n");
                        }
                      else
                        {
                            cardComponent.SetYear(cat + ": " + param + "\n");
                        }
                    }
                    else if(cat == "Language")
                    {
                        cardComponent.SetLanguage(cat + ": " + param + "\n");
                    }

                }
				kwReader.Close();
				kwReader = null;
				Deck.AddCards(cardComponent);
			}
			rd.Close();
			rd = null;
		}
	}

    public void CardExpand(Card card) //find card and player to expand
    {
        Player p = FindOwningPlayer(card);
        foreach (Card c in from Card c in p.GetHand() where c.GetImageLocation() == card.GetImageLocation() select c)
        {
            if (card.IsOnBoard())
            {
                p = _playerScriptRefs[0];
            }

            p.CardExpansion(c);
            sound.PlayExpand();
            return;
        }
    }

    public void CardUnexpand(Card card) //find card and player to unexpand
    {
        Player p = FindOwningPlayer(card);
        foreach (Card c in from Card c in p.GetHand() where c.GetImageLocation() == card.GetImageLocation() select c)
        {
            if (card.IsOnBoard())
            {
                p = _playerScriptRefs[0];
            }

            p.CardShrink(c);
            sound.PlayDeselect();
            return;
        }
    }

    public bool AddCardsToBoard(Card cardA, Card boardCard, string keyword)
    {
        bool validPlay = true;
        bool shouldDisable = false;

    

        if (!cardA.DoesPropertyExist(keyword) || !boardCard.DoesPropertyExist(keyword))
            validPlay = false;

        
	

        foreach (GameObject keyNode in _keywordNodes)
        {
            if (keyNode.transform.Find("Text").gameObject.GetComponent<Text>().text != keyword)
                continue;
            // The keyword is already a node. Use it.

			shouldDisable = SnapToKeyword.CornerSnap(cardA, boardCard, keyNode);

            if(shouldDisable)
            {
                _keywordList.Remove(_currentKeyword);
                playedKeywords.Remove(_currentKeyword);
                _keywordList.Find(x => x.Contains(_currentKeyword));
                _playerScriptRefs[CurrentPlayer].IncreaseScore(4);
            }

            cardA.SetIsOnBoard(true);
            sound.PlayPlace();
            cardA.SetIsSelected(false);
            boardCard.SetIsSelected(false);

			cardIndex = GetCurrentPlayer ().GetHand ().IndexOf (cardA); //Find the index of the current card played
			NewCard (cardIndex); //Prepare the hand for a new card

            //cardA.gameObject.AddComponent<MobileNode>();
            ResetPassArray();
			prevKeyNode = keyNode;

            if (_playerScriptRefs[CurrentPlayer].Score >= 20)
            {
                loadingScreen.gameObject.SetActive(true);
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

            SetMostRecent(cardA, boardCard, keyword);
            playedKeywords.Add(_currentKeyword);
            playedKeywords.Distinct();
            sound.PlayOnePoint();
            return true;
        }

        // Couldn't find the keyword in an existing node. Add it and connect both cards to it.
        GameObject newKeyNode = Instantiate(NodeOne); // Copy the template keyword node.
        newKeyNode.transform.Find("Text").gameObject.GetComponent<Text>().text = keyword;
        // Set the text of the new keyword node.
        newKeyNode.name = keyword;
        _keywordNodes.Add(newKeyNode); // Add the keyword to the list of keyword nodes.
  
		this.GetComponent<GridClass>().SnapToGrid(newKeyNode);
		shouldDisable = SnapToKeyword.CornerSnap(cardA, boardCard, newKeyNode);

        cardA.SetIsOnBoard(true);
        sound.PlayPlace();
        cardA.SetIsSelected(false);
        boardCard.SetIsSelected(false);
        //cardA.gameObject.AddComponent<MobileNode>();
		WriteDB.WriteConnection(keyword,cardA.GetID(), boardCard.GetID(),_dbconn);
		cardIndex = GetCurrentPlayer ().GetHand ().IndexOf (cardA); //Find the index of the current card played
		NewCard (cardIndex); //Prepare the hand for a new card
		//newKeyNode.gameObject.transform.SetParent (BoardGrid); 

		//newKeyNode.gameObject.transform.position = SnapToGrid(transform,GridSpacing);
		prevKeyNode = newKeyNode;

        if (_playerScriptRefs[CurrentPlayer].Score >= 20)
        {
            loadingScreen.gameObject.SetActive(true);
            _isGameStarted = false;
            CurrentPhase = GamePhase.PostGame;
            // TODO Go to end game screen here.
            //collect player scores for end game screen
            PlayerPrefs.SetInt("Player1Score", _playerScriptRefs[0].Score);
            PlayerPrefs.SetInt("Player2Score", _playerScriptRefs[1].Score);
            PlayerPrefs.SetInt("Player3Score", _playerScriptRefs[2].Score);
            PlayerPrefs.SetInt("Player4Score", _playerScriptRefs[3].Score);
            SceneManager.LoadScene("EndGame"); 
        }
        
        SetMostRecent(cardA, boardCard, keyword);
        playedKeywords.Add(_currentKeyword);
        playedKeywords.Distinct();
        sound.PlayOnePoint();
        keyWordCount++;
        
        CheckKeywordAmount();
        return true;
    }

    public void CheckKeywordAmount()
    {
        Debug.Log(keyWordCount);
        if (keyWordCount >= 12)
        {
            _keywordList.Clear();
            keyWordCount = 0;

            for(int i = 0; i < playedKeywords.Count(); i++)
            {
                _keywordList.Add(playedKeywords[i]);
            }

            playedKeywords.Distinct();
        }
        else
            return;
    }

    private void SetMostRecent(Card cardA, Card boardCard, string keyword)
    {
        DestroyAllChildren(mostRecentCard1);
        Card tempCard = Instantiate(cardA);
        tempCard.transform.position = mostRecentCard1.transform.position;
        tempCard.transform.parent = mostRecentCard1.transform;

        DestroyAllChildren(mostRecentCard2);
        tempCard = Instantiate(boardCard);
        tempCard.transform.position = mostRecentCard2.transform.position;
        tempCard.transform.parent = mostRecentCard2.transform;
        //mostRecentCard1.GetComponentInChildren<SpriteRenderer>().sprite = cardA.GetComponent<SpriteRenderer>().sprite;
        mostRecentCard2.GetComponentInChildren<SpriteRenderer>().sprite = boardCard.GetComponent<SpriteRenderer>().sprite;
        mostRecentKeyword.GetComponentInChildren<Text>().text = keyword;
        mostRecentPlayer.GetComponent<SpriteRenderer>().sprite = this.GetComponent<MainSceneCharacters>().Images[CurrentPlayer].sprite;
    }

    public void DestroyAllChildren(GameObject Parent)
    {
        foreach (Transform child in Parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

    }

    public static void ResetPassArray()
    {
        for (int i = 0; i < Player.PassArray.Length; i++)
        {
            Player.PassArray[i] = false;
        }
    }

    public void SelectCardInHand(Card card)
    {
        //Debug.Log("Attempting to select hand card: " + card.name);
        bool cardFound =
            _playerScriptRefs[CurrentPlayer].GetHand().Cast<Card>().Any(c => c.name == card.name && !c.IsOnBoard());

        // First, check if the card is in the current player's hand.
        if (!cardFound)
        {
            //Debug.Log("Card not found");
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
                sound.PlayDeselect();

                //turn glow off
                InHandGlowOff(card);
                _playerScriptRefs[CurrentPlayer].Card1 = null; 
                _isPlayerCardSelected = false;
                return;
            }

            c.SetIsSelected(false); // Deselect the other card, then select this one.
            card.SetIsSelected(true);
            sound.PlaySelect();

            //glow on
            _playerScriptRefs[CurrentPlayer].Card1 = card;
            InHandGlowOn(card);

            return;
        }
        card.SetIsSelected(true);
        sound.PlaySelect();

			
        _isPlayerCardSelected = true;

        //glow on
        _playerScriptRefs[CurrentPlayer].Card1 = card;
        InHandGlowOn(card);

    }

    public void SelectCardOnBoard(Card card)
    {
        //Debug.Log("Attempting to select board card: " + card.name);
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
                    sound.PlayDeselect();
                    _isBoardCardSelected = false;

                    //turn glow off
                    OnBoardGlowOff(card);

                    return;
                }
                c.SetIsSelected(false);
                card.SetIsSelected(true);
                sound.PlaySelect();
				//Debug.Log (card.ToString ());
                //glow on
                OnBoardGlowOn(card);

                return;
            }
        }
        card.SetIsSelected(true);
        sound.PlaySelect();
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
        sound.PlaySelect();
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
        
    }

    private void EnableVet() //enable vet screen
    {
  
    }

    private void VetSetUp()  //timer before vet screen pops up
    {
        //Debug.Log("Enabling vet.");
        CurrentPhase = GamePhase.Vetting;
  
        _copyCardLeft = Instantiate(_playerScriptRefs[CurrentPlayer].Card1, new Vector3(0f, 0f, 0f), Quaternion.identity);
        _copyCardLeft.transform.position = VetCard1.gameObject.transform.position;
        _copyCardLeft.transform.localScale = VetCard1.gameObject.GetComponent<Renderer>().bounds.extents;

        _copyCardRight = Instantiate(_playerScriptRefs[CurrentPlayer].Card2, new Vector3(0f, 0f, 0f), Quaternion.identity);
        _copyCardRight.transform.position = VetCard2.gameObject.transform.position;
        _copyCardRight.transform.localScale = VetCard2.gameObject.GetComponent<Renderer>().bounds.extents;

        PlayerNumber = 0;
        _hitVetBtn = true;

        VetResultList[0] = CheckConnection();
        //Debug.Log("AI vetted " + VetResultList[0]);
        _playerScriptRefs[PlayerNumber].PlayerVetted = true; //first AI done 
        _playerScriptRefs[PlayerNumber].YesNoBtnHit = true;
        _playerScriptRefs[PlayerNumber].VetResult = VetResultList[0];

    }

    private IEnumerator VetAiDecision() //timer for AI to "think"
    {
        yield return new WaitForSeconds(0.5f);

        int rindex = Random.Range(0, 101);
        bool aiVet = rindex > 50;
        VetResultList[PlayerNumber] = aiVet;
        _playerScriptRefs[PlayerNumber].PlayerVetted = true;
        _aiThinkingDone = false; //reset
    }

	private IEnumerator RestartingGame() //timer for AI to "think"
	{
		yield return new WaitForSeconds(3.0f);

		EndGame2.MainMenuTransition ();
	}

    private bool GetVetResult()
    {
        //Debug.Log("Getting vet results.");
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
        LegalVotePlayerList.Clear();    //clear player voting list for AI voting

        foreach (Player p in _playerScriptRefs) //shrink main voting pieces 
        {
            p.VoteMainShrink();
        }
    }

    private void EnableVote() //enable vote screen
    {
        //Debug.Log("Hi, Mom!");
        foreach (Player p in _playerScriptRefs) //expand main voting pieces 
        {
            p.VoteMainExpansion();
        }
    }

    private void VoteSetUp()  //vote screen pops up
    {
        //Debug.Log("Enabling voting.");
        CurrentPhase = GamePhase.Voting;
        EnableVote();
        ToggleCardsOff();

        for (int i = 0; i < 4; i++)
        {
            VoteResultsList[i] = 1; //reset result list
        }

        PlayerNumber = 0; //reset for voting below
       
        //AI Voting
        _playerScriptRefs[PlayerNumber].PlayerVoteExpansion();
        VoteResultsList[0] = 1; //preset for AI
        _playerScriptRefs[PlayerNumber].PlayerVoted = true;
    }

    private void GetVoteResult()
    {
        //Debug.Log("Getting vote results.");
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
        //Debug.Log(scoring.Count);
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

                            //Debug.Log("Checking " + freq.KeywordName);
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
            //Debug.Log(freq.KeywordName + " " + freq.KeywordFreqs + " is processed!");
        }
    }

    private IEnumerator VoteAiDecision() //timer for AI to "think"
    {
        yield return new WaitForSeconds(0.5f);

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
        
    }

    private void InHandGlowOn(Card card)
    {
		InHandGlow.GetComponent<Renderer>().enabled = true;
		InHandGlow.transform.position = card.gameObject.transform.position;

		RotateGlow (card, InHandGlow);      
    }

    public void OnBoardGlowOn(Card card)
    {  
		RotateGlow (card, OnBoardGlow);
		OnBoardGlow.GetComponent<Renderer>().enabled = true;
		OnBoardGlow.transform.position = card.gameObject.transform.position;
    }

    public void InHandGlowOff(Card card)
    {
		UnrotateGlow (card, InHandGlow);
        InHandGlow.GetComponent<Renderer>().enabled = false;
    }

    private void OnBoardGlowOff(Card card)
    {
		UnrotateGlow (card, OnBoardGlow);
        OnBoardGlow.GetComponent<Renderer>().enabled = false;
    }

	private void CanDefaultPlayerLeave()
	{
		int count = 0;

		for(int i = 0; i < Players.Length; i++) //Count how many Human Players there are
		{
			if (_playerScriptRefs [i].IsAiControlled == false) 
			{
				count++;
			}
		}

		if (count >= 2 && _playerScriptRefs [2].IsAiControlled == false)  //if there is more than 2 and one of them is the default player
		{
			_playerScriptRefs [2].LeaveGameBtn.gameObject.SetActive (true); //give them the option to leave
		} else
		{
			_playerScriptRefs [2].LeaveGameBtn.gameObject.SetActive (false); //else the default player must stay in
		}
	}

	//Rotate the glow if the card is horizontal
	private void RotateGlow(Card card, GameObject Glow)
	{
		if (card.isHorizontal == true)
		{ //card is horizontal
			//transform.Rotate(0.0f, 0.0f, 90.0f); 		 <---- this made the Canvas itself rotate
			Glow.gameObject.transform.eulerAngles = new Vector3 (0, 0, 90);
		}
		else
		{
			Glow.gameObject.transform.eulerAngles = new Vector3 (0, 0, 0);
		}
	}

	//unrotate the glow if the glow was rotated
	private void UnrotateGlow(Card card, GameObject Glow)
	{
		if (card.isHorizontal == true) 
		{ //card is horizontal
			//transform.Rotate(0.0f, 0.0f, 90.0f); 		 <---- this made the Canvas itself rotate
			Glow.gameObject.transform.eulerAngles = new Vector3 (0, 0, 0);
		} 
		else
		{
			Glow.gameObject.transform.eulerAngles = new Vector3 (0, 0, 90);
		}

	}

	//Container is the Container which is passed in
	private GameObject SetupKeywordButton(GameObject Container, string str, int playerTurn)
	{
		GameObject go = Instantiate(KeywordPrefab);
		go.name = str;
        var goText = go.GetComponentInChildren<Text>();
		goText.text = str;
		goText.resizeTextForBestFit = true;
		goText.resizeTextMaxSize = 10;
		goText.resizeTextMinSize = 1;
		goText.fontStyle = FontStyle.Bold;
		goText.alignByGeometry = true;

		go.transform.SetParent (Container.transform);

		Button btn = go.GetComponent<Button> ();

		ColorBlock btnColors = btn.colors;
		btnColors.normalColor = Color.white;
		btnColors.highlightedColor = Color.yellow;
		btnColors.pressedColor = Color.grey;

		if (CurrentPhase == GamePhase.Research && _currentKeywordList.Contains (str))
		{
			btnColors.normalColor = Color.grey;
		}

		btn.colors = btnColors;

		btn.onClick.AddListener (() => 
		{
			//Debug.Log(go.GetComponentInChildren<Text>().text + " Clicked!");

			switch (CurrentPlayer)
			{
				case 0:
					go.transform.Rotate (0.0f, 0.0f, 180.0f);
					break;
				default:
					break;
			}

				if (CurrentPhase == GamePhase.Research && CurrentPlayer == playerTurn)
				{
					if (_numSelections < 5 && _currentKeyword != go.GetComponentInChildren<Text> ().text && !_currentKeywordList.Contains (go.GetComponentInChildren<Text> ().text)) 
					{
                    sound.PlaySelect ();
						//Debug.Log("Setting current keyword to: " + go.GetComponentInChildren<Text>().text);
						_currentKeyword = go.GetComponentInChildren<Text> ().text;
						_currentKeywordButton = go;
						_numSelections++;
					}
				
//					else 
//					{
//						PlaySelect ();
//						_currentKeyword = go.GetComponentInChildren<Text> ().text;
//					}			
				}
				else if (CurrentPhase == GamePhase.Playing )//&& CurrentPlayer == playerTurn
				{
                sound.PlaySelect ();
					_currentKeyword = go.GetComponentInChildren<Text> ().text;
					_currentKeywordButton = go;
				}
		});
		
		Vector3 scale = transform.localScale;
		scale.x = 1.0f;
		scale.y = 1.0f;
		scale.z = 1.0f;
		//go.transform.Rotate(0.0f, 0.0f, -90.0f);
		go.transform.localScale = scale;
		go.SetActive (true);

        return go;
	}

	//adds a new card to the players hand
	public void NewCard(int cIndex)
	{
		GetCurrentPlayer ().HandSize++; //makes a spot in the players hand
		//GetCurrentPlayer ().GetHand ().RemoveAt (cIndex);
		GetCurrentPlayer ()._slotStatus [cIndex] = false; //registers an open spot in the hand
		GetCurrentPlayer ().RedealCards (); //adds the new card
		SwapCardPos (cIndex); //swaps the cards so the slot match up
	}

	//swaps old card with new card
	public void SwapCardPos(int cIndex)
	{
		Card temp = new Card();
		temp = GetCurrentPlayer ().GetHand ()._cardList [cIndex]; //Temp equals the card played
		GetCurrentPlayer ().GetHand ()._cardList [cIndex] = GetCurrentPlayer ().GetHand ()._cardList [GetCurrentPlayer ().GetHand ()._cardList.Count - 1]; //replace the played card with the drawn card
		GetCurrentPlayer ().GetHand ()._cardList [GetCurrentPlayer ().GetHand ()._cardList.Count - 1] = temp; //the last card in the deck is the played card
	}
}