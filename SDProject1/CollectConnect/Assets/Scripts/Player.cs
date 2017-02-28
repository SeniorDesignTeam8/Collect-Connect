using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public int Score { get; private set; }
    public bool IsDrawingCards { get; private set; }

    public GameObject[] CardPlaceholders;
    public GameObject PlayerScore;
    public GameObject ExpCardPlace; // The expanded card placeholder.
    public GameObject ExpCardImage; // Expand card Image
    public GameObject ExpCardTitle; // Title of expanded card.
    public GameObject ExpCardInfo; // Extended info of expanded card.
    public const int HandSize = 5;
    private bool[] _slotStatus = new bool[HandSize]; // True if taken, false if available.
    private string _playerName; // The player's name (internally).
    public CardCollection _playerHand; // Represents the player's cards.
    private Vector3 _expCardPosition;
    private Vector3 _expCardScale;
    private bool _isAiControlled = false; // TODO Find a way to programatically change this.
    public GameObject VetEnhance;
    public GameObject VetText;
    public Button VetYesBtn;
    public Button VetNoBtn;
    public bool playerVetted;
    public bool YesNoBtnHit;
    public bool VetResult;
    public Card card1;
    public Card card2;
    public String connectionKeyword;
    public GameObject locationOnBoard1;
    public GameObject locationOnBoard2;
    public GameObject locationOnBoard3;
    public GameObject locationOnBoard4;
    public GameObject locationOnBoard5;

    private static readonly float[] AiPassThresholds =
    {
        0.2f, 0.25f, 0.2f, 0.25f
    };

    private void Start()
    {
        IsDrawingCards = true;
        _playerName = gameObject.name.Replace(" ", "").ToLower(); // Remove spaces and change to all lowercase to standardize.
        _playerHand = new CardCollection(gameObject.name + "'s Hand");
        PlayerScore.GetComponent<Text>();
        ExpCardPlace.gameObject.GetComponent<Renderer>().enabled = false;  //make card expansion invisible to user
        ExpCardImage.gameObject.GetComponent<Renderer>().enabled = false;
        ExpCardTitle.gameObject.GetComponent<Text>().enabled = false;
        ExpCardInfo.gameObject.GetComponent<Text>().enabled = false;
 		VetEnhance.gameObject.GetComponent<Renderer>().enabled = false;
        VetText.gameObject.GetComponent<Text>().enabled = false;
        VetYesBtn.gameObject.SetActive(false);
        VetNoBtn.gameObject.SetActive(false);
        VetYesBtn.GetComponent<Button>().onClick.AddListener(OnYesBtnHit);
        VetNoBtn.GetComponent<Button>().onClick.AddListener(OnNoBtnHit);
        playerVetted = false;
        YesNoBtnHit = false;
        VetResult = true;
        locationOnBoard1.gameObject.GetComponent<Renderer>().enabled = false;
        locationOnBoard2.gameObject.GetComponent<Renderer>().enabled = false;
        locationOnBoard3.gameObject.GetComponent<Renderer>().enabled = false;
        locationOnBoard4.gameObject.GetComponent<Renderer>().enabled = false;
        locationOnBoard5.gameObject.GetComponent<Renderer>().enabled = false;
    }

    private void Update()
    {
        if (!BoardManager.Instance.GetIsStarted() && !IsDrawingCards)
            return;
        if (_playerHand.Size < HandSize)
        {
            if (BoardManager.IsDeckReady)
            {
                Card c = BoardManager.Deck.Draw();
                _playerHand.AddCards(c);
                c.MoveToBoard();
            }
            else
            {
                IsDrawingCards = false;
            }
        }
        else
            IsDrawingCards = false;

        if (_isAiControlled && BoardManager.Instance.GetCurrentPlayer() == this &&
            !BoardManager.Instance.GetIsTurnOver() && BoardManager.Instance.GetIsStarted()
            && BoardManager.Instance._vetStartBool == false)
        {
            Debug.Log("AI Control: " + name);
            List<int> unplayedCardIndices = new List<int>();
            foreach (Card c in BoardManager.Instance.GetPlayersUnplayedCards())
            {
                if (!c.IsOnBoard() && _playerHand.IndexOf(c) != -1)
                {
                    unplayedCardIndices.Add(_playerHand.IndexOf(c));
                }
            }
            if (unplayedCardIndices.Count == 0)
                return;
            int randomIndex = Random.Range(0, unplayedCardIndices.Count);
            Card pickedCard = _playerHand.At(unplayedCardIndices[randomIndex]);
            BoardManager.Instance.SelectCardInHand(pickedCard);
            CardCollection playedCards = BoardManager.Instance.GetPlayedCards();
            if (playedCards.Size == 0)
            {
                Debug.Log("First played card.");
                //No played cards, so must be first played card.
            }
            else
            {
                float passChance = Random.Range(0.0f, 1.0f);
                if (passChance <= AiPassThresholds[BoardManager.Instance.CurrentPlayer])
                {
                    Debug.Log("AI Passed.");
                    BoardManager.Instance.PassBtnHit();
                }
                else
                {                    
                    playedCards.Shuffle(); // More organized way of choosing a random card than just picking a random index.
                    foreach (Card c in playedCards)
                    {
                        float aiValidPlayChance = Random.Range(0.0f, 1.0f);
                        List<Card.CardProperty> commonProps = c.FindCommonProperties(pickedCard);
                        //random index to determine if valid play should happen 80% of the time...
                        if (aiValidPlayChance < 0.8)
                        {
                            if (commonProps.Count <= 0)
                                continue;
                            BoardManager.Instance.SelectCardOnBoard(c);
                            ShufflePropertyList(ref commonProps);
                            BoardManager.Instance.SelectKeyword(commonProps[0]);
                            Debug.Log("AI play valid");
                        }
                        else
                        {
                            //...otherwise this invalid play should happen
                            Card badCard = c;
                            BoardManager.Instance.SelectCardOnBoard(badCard);
                            BoardManager.Instance.SelectKeyword(badCard.PropertyList.First());
                            Debug.Log("AI play invalid");
                        }
                    }
                }
            }
        }
    }


    public void IncreaseScore(int reward)
    {
        Score += reward;
        Debug.Log("Score: " + reward);
    }

    public void ReduceScore(int penalty)
    {
        Score -= penalty;
    }

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
        List<string> keywords = (from Card c in _playerHand from prop in c.PropertyList select prop.PropertyValue).ToList();
        // Remove any duplicates and return.
        return keywords.Distinct().ToList();
    }

    public CardCollection GetHand()
    {
        return _playerHand;
    }

    public void CardExpansion(Card card)  //Expand card
    {
        ExpCardPlace.gameObject.GetComponent<Renderer>().enabled = true;
        ExpCardImage.gameObject.GetComponent<Renderer>().enabled = true;
        ExpCardTitle.gameObject.GetComponent<Text>().text = card.name;
        ExpCardInfo.gameObject.GetComponent<Text>().text = card.GetExpInfo();
        ExpCardTitle.gameObject.GetComponent<Text>().enabled = true;
        ExpCardInfo.gameObject.GetComponent<Text>().enabled = true;
        _expCardPosition = card.gameObject.transform.position;
        _expCardScale = card.gameObject.transform.localScale;
        card.gameObject.transform.position = ExpCardImage.transform.position;
        card.gameObject.transform.localScale = Vector3.one;
        //Make card appear in expand
    }

    public void CardShrink(Card card)  //Shrink card
    {
        ExpCardPlace.gameObject.GetComponent<Renderer>().enabled = false;
        ExpCardImage.gameObject.GetComponent<Renderer>().enabled = false;
        ExpCardTitle.gameObject.GetComponent<Text>().enabled = false;
        ExpCardInfo.gameObject.GetComponent<Text>().enabled = false;
        card.gameObject.transform.position = _expCardPosition;
        card.gameObject.transform.localScale = _expCardScale;
        //Make card disappear in expand
    }

    public void SetAiControl(bool aiControlled)
    {
        _isAiControlled = aiControlled;
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
        VetEnhance.gameObject.GetComponent<Renderer>().enabled = true;
        VetText.gameObject.GetComponent<Text>().enabled = true;
        VetYesBtn.gameObject.SetActive(true);
        VetNoBtn.gameObject.SetActive(true);
        playerVetted = false;
        YesNoBtnHit = false;
    }

    public void VetShrink()
    {
        VetEnhance.gameObject.GetComponent<Renderer>().enabled = false;
        VetText.gameObject.GetComponent<Text>().enabled = false;
        VetYesBtn.gameObject.SetActive(false);
        VetNoBtn.gameObject.SetActive(false);
    }

    private void OnYesBtnHit()
    {
        VetResult = true;
        playerVetted = true;
        YesNoBtnHit = true;
    }

    private void OnNoBtnHit()
    {
        VetResult = false;
        playerVetted = true;
        YesNoBtnHit = true;
    }
}
