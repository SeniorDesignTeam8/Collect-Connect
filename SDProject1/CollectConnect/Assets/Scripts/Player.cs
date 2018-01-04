using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System;
using System.IO;

public class Player : MonoBehaviour
{
    public int Score { get; private set; }
    public bool IsDrawingCards { get; private set; }
    public AIPlay aiPlayer;
    public GameObject[] CardPlaceholders;
    public GameObject PlayerScore;

    public GameObject ExpCardBackground; // The expanded card placeholder.
    public GameObject ExpCardImage; // Expand card Image
    public GameObject ExpCardTitle; // Title of expanded card.
    public GameObject ExpCardInfo; // Extended info of expanded card.
    public GameObject ExpCardLoc; //Expanded Location of Card
    public GameObject CardLocImage; //The image of the source
    public GameObject ImageIconName;
    public GameObject ContText;
    public Text additinalInfo;

    public int HandSize = 4;
    public bool[] _slotStatus; // True if taken, false if available.
    private string _playerName; // The player's name (internally).
    public CardCollection PlayerHand; // Represents the player's cards.
    private Vector3 _expCardPosition;
    private Vector3 _expCardScale;
    public bool IsAiControlled;

    public GameObject PlayerPopUpEnhance;
    public GameObject PlayerPopUpEnhanceShadow;

    public Button LeaveGameBtn;
    public Button JoinGameBtn;
    public bool PlayerVetted;
    public bool YesNoBtnHit;
    public bool VetResult;

    public Card Card1;
    public Card Card2;
    public string ConnectionKeyword;
	public int SwitchesLeft = 3;

    public GameObject PlayerPiece;

    public GameObject LocationOnBoard1;
    public GameObject LocationOnBoard2;
    public GameObject LocationOnBoard3;
    public GameObject LocationOnBoard4;
    public GameObject LocationOnBoard5;

    public bool PlayerVoted;

    public int VotedForWho;
    public Card CopyCardLeft;
    public Card CopyCardRight;

    public GameObject BlockOff;


    public GameObject switchToken1;
    public GameObject switchToken2;
    public GameObject switchToken3;

    public static bool[] PassArray =
    {
        false, false, false, false
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
        HandSize = 4;
        _slotStatus = new bool[HandSize];
       
        IsDrawingCards = true;
        _playerName = gameObject.name.Replace(" ", "").ToLower();
        // Remove spaces and change to all lowercase to standardize.
        PlayerHand = new CardCollection(gameObject.name + "'s Hand");

        PlayerScore.GetComponent<Text>();
        ExpCardBackground.gameObject.GetComponent<Renderer>().enabled = false; //make card expansion invisible to user
        ExpCardImage.gameObject.GetComponent<Renderer>().enabled = false;
        ExpCardTitle.gameObject.GetComponent<Text>().enabled = false;
        ExpCardInfo.gameObject.GetComponent<Text>().enabled = false;
        ExpCardLoc.gameObject.GetComponent<Text>().enabled = false;
        CardLocImage.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        ImageIconName.gameObject.GetComponent<Text>().enabled = false;
        ContText.gameObject.GetComponent<Text>().enabled = false;
        PlayerPopUpEnhance.gameObject.GetComponent<Renderer>().enabled = false;
        PlayerPopUpEnhanceShadow.gameObject.GetComponent<Renderer>().enabled = false;
        additinalInfo.enabled = false;
 
        LeaveGameBtn.GetComponent<Button>().onClick.AddListener(OnLeaveBtnHit);
        JoinGameBtn.GetComponent<Button>().onClick.AddListener(OnJoinBtnHit);
        PlayerVetted = true;
        YesNoBtnHit = false;
        VetResult = true;
        PlayerVoted = true;

        LocationOnBoard1.gameObject.GetComponent<Renderer>().enabled = false;
        LocationOnBoard2.gameObject.GetComponent<Renderer>().enabled = false;
        LocationOnBoard3.gameObject.GetComponent<Renderer>().enabled = false;
        LocationOnBoard4.gameObject.GetComponent<Renderer>().enabled = false;
        LocationOnBoard5.gameObject.GetComponent<Renderer>().enabled = false;

        PlayerPiece.gameObject.GetComponent<Renderer>().enabled = false;
        BlockOff.gameObject.GetComponent<Renderer>().enabled = false;
 
    }

    private void Update()
    {
        if (!BoardManager.Instance.GetIsStarted() && !IsDrawingCards)
            return;
		
        if (PlayerHand.Size < HandSize)
        {
			//Debug.Log ("PlayerHand.Size = " + PlayerHand.Size + " Player = " + this.ToString () + " GamePhase = " + BoardManager.CurrentPhase.ToString ());

            if (BoardManager.IsDeckReady)
            {
				//Debug.Log ("PlayerHand.Size = " + PlayerHand.Size + " Player = " + this.ToString () + " GamePhase = " + BoardManager.CurrentPhase.ToString ());
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

        if (IsAiControlled && BoardManager.Instance.GetCurrentPlayer() == this && !BoardManager.Instance.IsTurnOver && BoardManager.Instance.GetIsStarted()
              && BoardManager.CurrentPhase != GamePhase.Voting && BoardManager.CurrentPhase != GamePhase.Vetting)
        {
            aiPlayer.PlayAI(PlayerHand);
        }

    }


    public void IncreaseScore(int reward)
    {
        Score += reward;
        //Debug.Log("Score: " + reward);
        PlayerScore.gameObject.GetComponent<Text>().text = Score.ToString(); //display score
    }

    public void PlaceCard(Card c, Vector3 rotation)
    {
        try
        {
            //Debug.Log(CardPlaceholders.Length.ToString());
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
        catch(Exception e)
        {
            Debug.Log(e);
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

        Text expAndLocText = ExpCardLoc.gameObject.GetComponent<Text>();
        expAndLocText.enabled = true;
        
        expAndLocText.text = card.GetLocation() + "\n"+ card.GetContributor();

        ImageIconName.gameObject.GetComponent<Text>().enabled = true;

        additinalInfo.enabled = true;
        additinalInfo.text = card.GetMedium() +card.GetYear()+ card.GetLanguage();
       //Text contributorText = ContText.gameObject.GetComponent<Text>();
       //contributorText.enabled = true;
       //contributorText.text = card.GetContributor();

        SetSourceSprite(card);

        _expCardPosition = card.gameObject.transform.position;
        _expCardScale = card.gameObject.transform.localScale;
        card.GetComponent<SpriteRenderer>().sprite = card.cardFullPic;
        card.gameObject.transform.position = ExpCardImage.transform.position;
        
        //card.transform.localScale = ExpCardImage.gameObject.GetComponent<Renderer>().bounds.extents;
        //Make card appear in expan
    }



    public void SetSourceSprite(Card card)
    {
        string imageLoc = card.GetSourceLoc();
        string imageFileName;
        //Debug.Log(imageLoc);
        switch (imageLoc)
        {

            //NEW LOCATION NEED TO BE ADDED?
            //case is the database entry for Card.location
            //imageFileName is the location of the image which is currentyl stored in the SourceImages folder
            case "Kelsey":
                imageFileName = "source-kelsey.png";
                break;
            case "AAEL":
                imageFileName = "source-duderstadt.png";
                break;
            case "Clark":
                imageFileName = "source-clark.png";
                break;
            case "Clements":
                imageFileName = "source-clements.png";
                break;
            case "The Arb":
                imageFileName = "source-botanical.png";
                break;
            case "Hatcher":
                imageFileName = "source-hatcher.png";
                break;
            case "Mardigian":
                imageFileName = "source-mardigian.png";
                break;
            case "Bentley":
                imageFileName = "source-spColl.png";
                break;
            case "UMMAA":
                imageFileName = "source-ummaa.png";
                break;
            case "UMMA":
                imageFileName = "source-umma.png";
                break;
            case "CVGA":
                imageFileName = "source-cvga.png";
                break;
            case "LRC":
                imageFileName = "source-LRC.png";
                break;
            case "Shapiro":
                imageFileName = "source-shapiro.png";
                break;
            default:
                imageFileName = "EMPTY";
                break;
        }


        if (imageFileName != "EMPTY")
        {
            byte[] fileData = File.ReadAllBytes(Application.dataPath + "/SourceImages/" + imageFileName);
            //Debug.Log(Application.dataPath.ToString() + "/SourceImages/" + imageFileName);
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(fileData);

            Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            CardLocImage.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            CardLocImage.gameObject.GetComponent<SpriteRenderer>().sprite = mySprite;
            ImageIconName.gameObject.GetComponent<Text>().text = imageLoc;
        }
        else
            ImageIconName.gameObject.GetComponent<Text>().enabled = false;
        
    }

    public void CardShrink(Card card) //Shrink card
    {
        ExpCardBackground.gameObject.GetComponent<Renderer>().enabled = false;
        ExpCardImage.gameObject.GetComponent<Renderer>().enabled = false;
        ExpCardTitle.gameObject.GetComponent<Text>().enabled = false;
        ExpCardInfo.gameObject.GetComponent<Text>().enabled = false;
        ExpCardLoc.gameObject.GetComponent<Text>().enabled = false;
        ImageIconName.gameObject.GetComponent<Text>().enabled = false;
        additinalInfo.enabled = false;

        if (CardLocImage.gameObject.GetComponent<SpriteRenderer>().enabled == true)
        {
            CardLocImage.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            ImageIconName.gameObject.GetComponent<Text>().enabled = false;
        }

        ContText.gameObject.GetComponent<Text>().enabled = false;
        card.GetComponent<SpriteRenderer>().sprite = card.cardSmallPic;
        card.gameObject.transform.position = _expCardPosition;
        card.gameObject.transform.localScale = _expCardScale;
        //Make card disappear in expand
    }

    public void SetAiControl(bool aiControlled)
    {
        IsAiControlled = aiControlled;
    }

    public void VetExpansion()
    {
        PlayerVetted = true;
    }

    public void VetShrink()
    {
  
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
        PlayerVoted = true; //reset
       
    }

    public void PlayerVoteShrink()
    {

    }

    public void VoteMainExpansion()
    {
 
    }

    public void VoteMainShrink()
    {

    }

    private void VotePlayer1()
    {
        PlayerVoted = true;
    }

    private void VotePlayer2()
    {
        PlayerVoted = true;
    }

    private void VotePlayer3()
    {
        PlayerVoted = true;
    }

    private void VotePlayer4()
    {
        PlayerVoted = true;
    }

    public void VetPieceExpansion()  //display turn player piece
    {
       
    }

    public void VetPieceShrink()  //shrink turn player piece
    {
      
    }

    public void PlayerPieceExpansion()  //display turn player piece
    {
       
    }

    public void PlayerPieceShrink()  //shrink turn player piece
    {
     
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

	public void RedealCards()
	{
        //Debug.Log ("PlayerHand.Size = " + PlayerHand.Size + " Player = " + this.ToString () + " GamePhase = " + BoardManager.CurrentPhase.ToString ());
        IsDrawingCards = false;
        if (PlayerHand.Size < HandSize)
		{
			IsDrawingCards = true;
			if (BoardManager.IsDeckReady || BoardManager.CurrentPhase == GamePhase.Playing)
			{
				//Debug.Log ("PlayerHand.Size = " + PlayerHand.Size + " Player = " + this.ToString () + " GamePhase = " + BoardManager.CurrentPhase.ToString ());
				Card c = BoardManager.Deck.Draw();
				PlayerHand.AddSingleCard(c);
				c.MoveToBoard();
			}
			else
			{
				IsDrawingCards = false;
			}
		}

	}

    public void RemoveToken()
    {
        switch (SwitchesLeft)
        {
            case 1:
                switchToken1.SetActive(false);
                break;
            case 2:
                switchToken2.SetActive(false);
                break;
            case 3:
                switchToken3.SetActive(false);
                break;

        };
    }
}
