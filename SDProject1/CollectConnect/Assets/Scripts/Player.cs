using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

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
    private CardCollection _playerHand; // Represents the player's cards.
    private Vector3 _expCardPosition;
    private Vector3 _expCardScale;

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
    }

    private void Update()
    {
        if (IsDrawingCards)
        {
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
        }
    }

    public void IncreaseScore(int reward)
    {
        Score += reward;
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
        List<string> keywords = (from Card c in _playerHand from prop in c._propertyList select prop.PropertyValue).ToList();
        // Remove any duplicates and return.
        return keywords.Distinct().ToList();
    }

    public CardCollection GetHand()
    {
        return _playerHand;
    }

    public void CardExpansion(Card card, Player player)  //Expand card
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
        //TODO Make card appear in expand
    }

    public void CardShrink(Card card, Player player)  //Shrink card
    {
        ExpCardPlace.gameObject.GetComponent<Renderer>().enabled = false;
        ExpCardImage.gameObject.GetComponent<Renderer>().enabled = false;
        ExpCardTitle.gameObject.GetComponent<Text>().enabled = false;
        ExpCardInfo.gameObject.GetComponent<Text>().enabled = false;
        card.gameObject.transform.position = _expCardPosition;
        card.gameObject.transform.localScale = _expCardScale;
        //TODO Make card disappear in expand
    }
}
