using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public int Score { get; private set; }
    public bool IsDrawingCards { get; private set; }

    public GameObject[] CardPlaceholders;
    public GameObject PlayerScore;
    public GameObject ExpCardBackground; // The expanded card placeholder.
    public GameObject ExpCardImage; // Expand card Image
    public GameObject ExpCardTitle; // Title of expanded card.
    public GameObject ExpCardInfo; // Extended info of expanded card.
    public const int HandSize = 4;
    private bool[] _slotStatus = new bool[HandSize]; // True if taken, false if available.
    private string _playerName; // The player's name (internally).
    public CardCollection PlayerHand; // Represents the player's cards.
    private Vector3 _expCardPosition;
    private Vector3 _expCardScale;
    public bool IsAiControlled;
    public GameObject PlayerPopUpEnhance;
    public GameObject PlayerPopUpEnhanceShadow;
    public GameObject VetText;
    public Button VetYesBtn;
    public Button VetNoBtn;
    public Button LeaveGameBtn;
    public Button JoinGameBtn;
    public bool PlayerVetted;
    public bool YesNoBtnHit;
    public bool VetResult;
    public Card Card1;
    public Card Card2;
    public string ConnectionKeyword;

    public GameObject PlayerPiece;

    public GameObject LocationOnBoard1;
    public GameObject LocationOnBoard2;
    public GameObject LocationOnBoard3;
    public GameObject LocationOnBoard4;
    public GameObject LocationOnBoard5;

    public GameObject VetPlayerPiece;

    public GameObject VoteText;
    public Button VoteBtnP1;
    public Button VoteBtnP2;
    public Button VoteBtnP3;
    public Button VoteBtnP4;
    public bool PlayerVoted;
    public GameObject VoteCardLeft;
    public GameObject VoteCardRight;
    public GameObject VoteKeywordTxt;
    public GameObject VoteConnection;
    public Button VotePassBtn;
    public int VotedForWho;
    public Card CopyCardLeft;
    public Card CopyCardRight;
   public GameObject VotePlayerPiece;

    public GameObject BlockOff;

    private string _vetHumanText;
    private string _aiText;
    private string _voteHumanText;

    public static bool[] PassArray =
    {
        false, false, false, false
    };

    private static readonly float[] AiPassThresholds =
    {
        0.05f, 0.05f, 0.05f, 0.05f
    };

	private void Awake()
	{
		#if !UNITY_EDITOR
		  
		        if (Debug.isDebugBuild)
		        {
		            Random.InitState(42);
		        }
		    
		#endif

		JoinGameBtn.gameObject.SetActive(false);// Race condition forced me to put this here
	}

    private void Start()
    {
		//Debug.Log ("Player " + PlayerPiece.ToString () + "   " + PassArray [0] + PassArray [1] + PassArray [2] + PassArray [3]);
        IsDrawingCards = true;
        _playerName = gameObject.name.Replace(" ", "").ToLower();
        // Remove spaces and change to all lowercase to standardize.
        PlayerHand = new CardCollection(gameObject.name + "'s Hand");
        PlayerScore.GetComponent<Text>();
        ExpCardBackground.gameObject.GetComponent<Renderer>().enabled = false; //make card expansion invisible to user
        ExpCardImage.gameObject.GetComponent<Renderer>().enabled = false;
        ExpCardTitle.gameObject.GetComponent<Text>().enabled = false;
        ExpCardInfo.gameObject.GetComponent<Text>().enabled = false;
        PlayerPopUpEnhance.gameObject.GetComponent<Renderer>().enabled = false;
        PlayerPopUpEnhanceShadow.gameObject.GetComponent<Renderer>().enabled = false;
        VetText.gameObject.GetComponent<Text>().enabled = false;
        VetYesBtn.gameObject.SetActive(false);
        VetNoBtn.gameObject.SetActive(false);
        VetYesBtn.GetComponent<Button>().onClick.AddListener(OnYesBtnHit);
        VetNoBtn.GetComponent<Button>().onClick.AddListener(OnNoBtnHit);
        LeaveGameBtn.GetComponent<Button>().onClick.AddListener(OnLeaveBtnHit);
        JoinGameBtn.GetComponent<Button>().onClick.AddListener(OnJoinBtnHit);
        PlayerVetted = false;
        YesNoBtnHit = false;
        VetResult = true;
        PlayerVoted = false;
        LocationOnBoard1.gameObject.GetComponent<Renderer>().enabled = false;
        LocationOnBoard2.gameObject.GetComponent<Renderer>().enabled = false;
        LocationOnBoard3.gameObject.GetComponent<Renderer>().enabled = false;
        LocationOnBoard4.gameObject.GetComponent<Renderer>().enabled = false;
        LocationOnBoard5.gameObject.GetComponent<Renderer>().enabled = false;
        VoteText.gameObject.GetComponent<Text>().enabled = false;
        VoteBtnP1.gameObject.SetActive(false);
        VoteBtnP2.gameObject.SetActive(false);
        VoteBtnP3.gameObject.SetActive(false);
        VoteBtnP4.gameObject.SetActive(false);
        VoteBtnP1.GetComponent<Button>().onClick.AddListener(VotePlayer1);
        VoteBtnP2.GetComponent<Button>().onClick.AddListener(VotePlayer2);
        VoteBtnP3.GetComponent<Button>().onClick.AddListener(VotePlayer3);
        VoteBtnP4.GetComponent<Button>().onClick.AddListener(VotePlayer4);
        VotePassBtn.GetComponent<Button>().onClick.AddListener(VotePlayer1);    //auto vote for player 1
        VotePassBtn.gameObject.SetActive(false);
        VotedForWho = 0;
        VotePlayerPiece.gameObject.GetComponent<Renderer>().enabled = false;
        PlayerPiece.gameObject.GetComponent<Renderer>().enabled = false;
        BlockOff.gameObject.GetComponent<Renderer>().enabled = false;
        
        _aiText = "AI is thinking...";
        _vetHumanText = "Do you agree with this connection?";
        _voteHumanText = "Which connection was the most outrageous?";
        VetPieceShrink();
    }

    private void Update()
    {
        if (!BoardManager.Instance.GetIsStarted() && !IsDrawingCards)
            return;
        if (PlayerHand.Size < HandSize)
        {
            if (BoardManager.IsDeckReady)
            {
                Card c = BoardManager.Deck.Draw();
                PlayerHand.AddCards(c);
                c.MoveToBoard();
            }
            else
            {
                IsDrawingCards = false;
            }
        }
        else
            IsDrawingCards = false;

        if (IsAiControlled && BoardManager.Instance.GetCurrentPlayer() == this &&
              !BoardManager.Instance.IsTurnOver && BoardManager.Instance.GetIsStarted()
              && BoardManager.CurrentPhase != GamePhase.Voting
              && BoardManager.CurrentPhase != GamePhase.Vetting)
        {

            //Debug.Log("AI Control: " + name);
            bool alreadyPlayed = false;
            List<int> unplayedCardIndices = new List<int>();
            foreach (Card c in BoardManager.Instance.GetPlayersUnplayedCards())
            {
                if (!c.IsOnBoard() && PlayerHand.IndexOf(c) != -1 && c.GetComponent<Renderer>().enabled)
                {
                    unplayedCardIndices.Add(PlayerHand.IndexOf(c));
                }
            }
            if (unplayedCardIndices.Count == 0)
            {
                BoardManager.Instance.PassBtnHit();
                return;
            }
            int randomIndex = Random.Range(0, unplayedCardIndices.Count);
            Card pickedCard = PlayerHand.At(unplayedCardIndices[randomIndex]);
            BoardManager.Instance.SelectCardInHand(pickedCard);
            CardCollection playedCards = BoardManager.Instance.GetPlayedCards();

            //for (int i = 0; i < playedCards.Size; i++)
            //{
            //    Debug.Log("Testing layer of card: " + playedCards.At(i).name + " Layer = " + playedCards.At(i).gameObject.layer);
            //    if (playedCards.At(i).gameObject.layer == 2)
            //    {

            //        Debug.Log("Removed card: " + playedCards.At(i).name);
            //        playedCards.RemoveAt(i);     
            //    }
            //}
            //Debug.Log("AI trying to play...  ");
            //if (playedCards.Size == 0)
            //{
            //    //Debug.Log("First played card.");
            //    //No played cards, so must be first played card.
            //}
            //else//{


            float passChance = Random.Range(0.0f, 1.0f);
            if (passChance <= AiPassThresholds[BoardManager.Instance.CurrentPlayer])
            {
                //Debug.Log("AI Passed.");
                BoardManager.Instance.PassBtnHit();
            }
            else
            {
                playedCards.Shuffle();
                // More organized way of choosing a random card than just picking a random index.
                float aiValidPlayChance = Random.Range(0.0f, 1.0f);
                foreach (Card c in playedCards)
                {
                    //Card c = playedCards.At(0);
                    //Debug.Log("trying a card in hand...");
                    List<Card.CardProperty> commonProps = c.FindCommonProperties(pickedCard);
                    //random index to determine if valid play should happen 80% of the time...
                    if (aiValidPlayChance < 0.8)
                    {
                        if (commonProps.Count <= 0)
                        {
                            //Debug.Log("no common props");
                            //c = playedCards.At(2);
                            continue;
                        }
                        BoardManager.Instance.SelectCardOnBoard(c);
                        ShufflePropertyList(ref commonProps);
                        BoardManager.Instance.SelectKeyword(commonProps[0]);
                        alreadyPlayed = true;
                        //Debug.Log("AI play valid");
                        break;
                    }
                    //...otherwise this invalid play should happen
                    BoardManager.Instance.SelectCardOnBoard(c);
                    BoardManager.Instance.SelectKeyword(c.PropertyList.First());
                    alreadyPlayed = true;
                    //Debug.Log("AI play invalid");
                    break;
                }
                if (!alreadyPlayed && playedCards.Size > 0)
                {
                    //...otherwise this invalid play should happen
                    int randomindex = Random.Range(0, playedCards.Size);
                    BoardManager.Instance.SelectCardOnBoard(playedCards.At(randomindex));
                    BoardManager.Instance.SelectKeyword(playedCards.At(randomindex).PropertyList.First());
                    //Debug.Log("AI play invalid after");
                    //break;
                }

            }
        }

    }


    public void IncreaseScore(int reward)
    {
        Score += reward;
        //Debug.Log("Score: " + reward);
        PlayerScore.gameObject.GetComponent<Text>().text = Score.ToString(); //display score
    }

    //public void ReduceScore(int penalty)
    //{
    //    Score -= penalty;
    //}

    public void PlaceCard(Card c, Vector3 rotation)
    {
        for (int i = 0; i < CardPlaceholders.Length; i++)
        {
            if (_slotStatus[i])
                continue;
            c.transform.position = CardPlaceholders[i].transform.position + new Vector3(0.0f, 0.0f, -5.0f);
            c.transform.Rotate(rotation, Space.Self);
            _slotStatus[i] = true;
            break;
        }
    }

    public IEnumerable<string> GetKeywords()
    {
        // Get the property value string from the property list in each card.
        List<string> keywords =
            (from Card c in PlayerHand from prop in c.PropertyList select prop.PropertyValue).ToList();
        // Remove any duplicates and return.
        return keywords.Distinct().ToList();
    }

    public CardCollection GetHand()
    {
        return PlayerHand;
    }

    public void CardExpansion(Card card) //Expand card
    {
            ExpCardBackground.gameObject.GetComponent<Renderer>().enabled = true;
            ExpCardImage.gameObject.GetComponent<Renderer>().enabled = true;
            ExpCardTitle.gameObject.GetComponent<Text>().text = card.name;
            ExpCardInfo.gameObject.GetComponent<Text>().text = card.GetExpInfo();
            ExpCardTitle.gameObject.GetComponent<Text>().enabled = true;
            ExpCardInfo.gameObject.GetComponent<Text>().enabled = true;
            _expCardPosition = card.gameObject.transform.position;
            _expCardScale = card.gameObject.transform.localScale;
            card.gameObject.transform.position = ExpCardImage.transform.position;
            card.gameObject.transform.localScale = ExpCardImage.gameObject.GetComponent<Renderer>().bounds.extents;
            //Make card appear in expand
    }

    public void CardShrink(Card card) //Shrink card
    {
        ExpCardBackground.gameObject.GetComponent<Renderer>().enabled = false;
        ExpCardImage.gameObject.GetComponent<Renderer>().enabled = false;
        ExpCardTitle.gameObject.GetComponent<Text>().enabled = false;
        ExpCardInfo.gameObject.GetComponent<Text>().enabled = false;
        card.gameObject.transform.position = _expCardPosition;
        card.gameObject.transform.localScale = _expCardScale;
        //Make card disappear in expand
    }

    public void SetAiControl(bool aiControlled)
    {
        IsAiControlled = aiControlled;
    }

    private static void ShufflePropertyList(ref List<Card.CardProperty> propList)
    {
        for (int i = 0; i < propList.Count; i++)
        {
            Card.CardProperty temp = propList[i];
            int randIndex = Random.Range(0, propList.Count);
            propList[i] = propList[randIndex];
            propList[randIndex] = temp;
        }
    }

    public void VetExpansion()
    {
        PlayerPopUpEnhance.gameObject.GetComponent<Renderer>().enabled = true;
        PlayerPopUpEnhanceShadow.gameObject.GetComponent<Renderer>().enabled = true;
        BlockOff.gameObject.GetComponent<Renderer>().enabled = false;

        if (!IsAiControlled) //if human is playing
        {
            VetText.gameObject.GetComponent<Text>().text = _vetHumanText;
            VetText.gameObject.GetComponent<Text>().enabled = true;
            VetYesBtn.gameObject.SetActive(true);
            VetNoBtn.gameObject.SetActive(true);
        }
        else //if AI is playing
        {
            VetText.gameObject.GetComponent<Text>().text = _aiText;
            VetText.gameObject.GetComponent<Text>().enabled = true;

            //Turn on block off
            BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
        }

        PlayerVetted = false;
        YesNoBtnHit = false;
        JoinGameBtn.gameObject.SetActive(false);    //turn off joining and leaving game
        LeaveGameBtn.gameObject.SetActive(false);
    }

    public void VetShrink()
    {
        PlayerPopUpEnhance.gameObject.GetComponent<Renderer>().enabled = false;
        PlayerPopUpEnhanceShadow.gameObject.GetComponent<Renderer>().enabled = false;
        BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
        VetText.gameObject.GetComponent<Text>().enabled = false;
        VetYesBtn.gameObject.SetActive(false);
        VetNoBtn.gameObject.SetActive(false);
        //playerVetted = false;
        //YesNoBtnHit = false;

        if (IsAiControlled) //after vet over display leave or join buttons
        {
            JoinGameBtn.gameObject.SetActive(true);
        }
        else
        {
            LeaveGameBtn.gameObject.SetActive(true);
        }
    }

    private void OnYesBtnHit()
    {
        BoardManager.Instance.PlaySelect();
        VetResult = true;
        PlayerVetted = true;
        YesNoBtnHit = true;
    }

    private void OnNoBtnHit()
    {
        BoardManager.Instance.PlaySelect();
        VetResult = false;
        PlayerVetted = true;
        YesNoBtnHit = true;
    }

    public void OnLeaveBtnHit()
    {
        BoardManager.Instance.PlaySelect();
       // Debug.Log("leave btn hit");

		//if the player can leave
		if (CanLeave () == true) 
		{
			BlockOff.gameObject.GetComponent<Renderer> ().enabled = true;
			JoinGameBtn.gameObject.SetActive (true);
			LeaveGameBtn.gameObject.SetActive (false);
			SetAiControl (true);
		}
    }

    private void OnJoinBtnHit()
    {
        BoardManager.Instance.PlaySelect();
        //Debug.Log("Join btn hit");
        JoinGameBtn.gameObject.SetActive(false);
        LeaveGameBtn.gameObject.SetActive(true);
        SetAiControl(false);

        if (BoardManager.Instance.GetCurrentPlayer() == this)
        {
            BlockOff.gameObject.GetComponent<Renderer>().enabled = false;
        }
    }

    public void PlayerVoteExpansion()
    {
        PlayerVoted = false; //reset
        PlayerPopUpEnhance.gameObject.GetComponent<Renderer>().enabled = true;
        PlayerPopUpEnhanceShadow.gameObject.GetComponent<Renderer>().enabled = true;
        BlockOff.gameObject.GetComponent<Renderer>().enabled = false;

        if (!IsAiControlled) //if human is playing
        {
            VoteText.gameObject.GetComponent<Text>().text = _voteHumanText;
            VoteText.gameObject.GetComponent<Text>().enabled = true;

            //if there are no players you can vote for
            if (BoardManager.Instance.CantVotePlayerList.All(b => b))
            {
                VotePassBtn.gameObject.SetActive(true);

            }   //if current player is has play, but rest can't vote for 
            else if (BoardManager.Instance.CantVotePlayerList[BoardManager.Instance.PlayerNumber] == false &&
                        BoardManager.Instance.CantVotePlayerList.FindAll(c => c == false).Count == 1)
            {
                VotePassBtn.gameObject.SetActive(true);
            }

            if (!BoardManager.Instance.CantVotePlayerList[0]) //turn on voting buttons
                VoteBtnP1.gameObject.SetActive(true);

            if (!BoardManager.Instance.CantVotePlayerList[1])
                VoteBtnP2.gameObject.SetActive(true);

            if (!BoardManager.Instance.CantVotePlayerList[2])
                VoteBtnP3.gameObject.SetActive(true);

            if (!BoardManager.Instance.CantVotePlayerList[3])
                VoteBtnP4.gameObject.SetActive(true);
        }
        else //if AI is playing
        {
            VoteText.gameObject.GetComponent<Text>().text = _aiText;
            VoteText.gameObject.GetComponent<Text>().enabled = true;
        }
    }

    public void PlayerVoteShrink()
    {
        PlayerPopUpEnhance.gameObject.GetComponent<Renderer>().enabled = false;
        PlayerPopUpEnhanceShadow.gameObject.GetComponent<Renderer>().enabled = false;
        VoteText.gameObject.GetComponent<Text>().enabled = false;
        VoteBtnP1.gameObject.SetActive(false);
        VoteBtnP2.gameObject.SetActive(false);
        VoteBtnP3.gameObject.SetActive(false);
        VoteBtnP4.gameObject.SetActive(false);
        VotePassBtn.gameObject.SetActive(false);
        BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
    }

    public void VoteMainExpansion()
    {
        VoteCardLeft.gameObject.GetComponent<Renderer>().enabled = true;
        VoteCardRight.gameObject.GetComponent<Renderer>().enabled = true;
        VoteKeywordTxt.gameObject.GetComponent<Text>().enabled = true;
        VoteConnection.gameObject.GetComponent<Renderer>().enabled = true;
        VotePlayerPiece.gameObject.GetComponent<Renderer>().enabled = true;

        JoinGameBtn.gameObject.SetActive(false);    //turn off joining and leaving game
        LeaveGameBtn.gameObject.SetActive(false);

        BlockOff.gameObject.GetComponent<Renderer>().enabled = true;
    }

    public void VoteMainShrink()
    {
        VoteCardLeft.gameObject.GetComponent<Renderer>().enabled = false;
        VoteCardRight.gameObject.GetComponent<Renderer>().enabled = false;
        VoteKeywordTxt.gameObject.GetComponent<Text>().enabled = false;
        VoteConnection.gameObject.GetComponent<Renderer>().enabled = false;
        VotePlayerPiece.gameObject.GetComponent<Renderer>().enabled = false;
        PlayerVoted = false; //reset

        if (IsAiControlled) //after vet over display leave or join buttons
        {
            JoinGameBtn.gameObject.SetActive(true);
        }
        else
        {
            LeaveGameBtn.gameObject.SetActive(true);
        }
        //_postVote = true;
    }

    private void VotePlayer1()
    {
        PlayerVoted = true;
        VotedForWho = 1;
        BoardManager.Instance.PlaySelect();
        VotePassBtn.gameObject.SetActive(false); //if used, make disappear
    }

    private void VotePlayer2()
    {
        PlayerVoted = true;
        VotedForWho = 2;
        BoardManager.Instance.PlaySelect();

    }

    private void VotePlayer3()
    {
        PlayerVoted = true;
        VotedForWho = 3;
        BoardManager.Instance.PlaySelect();
    }

    private void VotePlayer4()
    {
        PlayerVoted = true;
        VotedForWho = 4;
        BoardManager.Instance.PlaySelect();
    }

    public void VetPieceExpansion()  //display turn player piece
    {
        VetPlayerPiece.gameObject.GetComponent<Renderer>().enabled = true;
    }

    public void VetPieceShrink()  //shrink turn player piece
    {
        VetPlayerPiece.gameObject.GetComponent<Renderer>().enabled = false;
    }

    public void PlayerPieceExpansion()  //display turn player piece
    {
        PlayerPiece.gameObject.GetComponent<Renderer>().enabled = true;
    }

    public void PlayerPieceShrink()  //shrink turn player piece
    {
        PlayerPiece.gameObject.GetComponent<Renderer>().enabled = false;
    }


	//checks to see if the player can leave
	private bool CanLeave()
	{
		int count = 0;

		for(int i = 0; i < BoardManager.Instance.Players.Length; i++)
		{
			if (BoardManager.Instance._playerScriptRefs [i].IsAiControlled == false) 
			{
				count++;
			}
		}

		if (count > 1)
			return true;
		else
			return false;
	}
}
