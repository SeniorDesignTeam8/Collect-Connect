﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Card : MonoBehaviour
{
    public struct CardProperty
    {
        public readonly string PropertyName;
        public readonly string PropertyValue;
        private int _pointValue;

        public CardProperty(string name, string value, string pointString = "0")
        {
            PropertyName = name;
            PropertyValue = value;
            if (!int.TryParse(pointString, out _pointValue))
                _pointValue = 0;
        }

        public bool Equals(CardProperty other)
        {
            return string.Equals(PropertyName, other.PropertyName) && string.Equals(PropertyValue, other.PropertyValue) && _pointValue == other._pointValue;
        }

        public void SetPointValue(int newValue)
        {
            _pointValue = newValue;
        }
    }

    public List<LineRenderer> LineList;
    private BoxCollider2D _collider;

    private SpriteRenderer _renderer;
    private const float ExpandedInfoDelay = 0.5f; // Time to wait before expanding.
    private float _mouseDownTime; // Time when last clicked, in seconds since game start.
    private bool _isOnBoard; // Specifies if the card is in play or in the deck.
    private bool _isExpanded; // Specifies if the card is currently in expanded view.
    private bool _isDragging;
    private Vector3 _originalPosition;
    private Vector3 _pointerDownPosition;
    private bool _isSpriteLoaded;
    private bool _isSelected; // Specifies if this card is selected.
    private bool _isTimerRunning; // If true, mouse is currently held down on card.
    private string _expandedInfo; // Information to display in expanded view.
    public readonly List<CardProperty> PropertyList = new List<CardProperty>();

    // Use this for initialization
    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _renderer = GetComponent<SpriteRenderer>();
        LineList = new List<LineRenderer>();
    }

    private void OnMouseDown()
    {
        Debug.Log("Mouse Down.");
        _mouseDownTime = Time.time; // Mark the current time as 0. After 2 seconds, expand card.
        _pointerDownPosition = Input.mousePosition;
        _isTimerRunning = true;
    }

    private void OnMouseUp()
    {
        Debug.Log("Mouse Up.");
        _isTimerRunning = false;
        if (_isDragging)
        {
            _isDragging = false;
            return;
        }
        if (_isExpanded) // Is the card expanded?
        {
            Debug.Log("Shrinking " + name);
            BoardManager.Instance.CardUnexpand(this);

            _isExpanded = false;
        }
        else if (IsOnBoard()) // Is the card on the board? If not, then select from player's hand.
        {
            BoardManager.Instance.SelectCardOnBoard(this);
        }
        else // It's in a player's hand. If it's the owning player's turn, then select it.
        {
            BoardManager.Instance.SelectCardInHand(this);
        }
        // Otherwise, do nothing (might change to deselect other card in future).
    }

    private void Update()
    {
        if (!_isSpriteLoaded) // Wait for sprite to load before anything else.
        {
            _isSpriteLoaded = SetSprite();
            return;
        }
        if (!_isTimerRunning || !(Time.time - _mouseDownTime >= ExpandedInfoDelay))
            return;
        _isTimerRunning = false;
        Debug.Log("Expanding " + name);
        // TODO: Expand Card.
        BoardManager.Instance.CardExpand(this);
        _isExpanded = true;
    }

    private void OnMouseDrag()
    {
        if (_isOnBoard && Vector3.Distance(_pointerDownPosition, Input.mousePosition) >
            gameObject.GetComponent<RectTransform>().rect.width / 2 && !_isExpanded)
        {
            _isDragging = true;
            _isTimerRunning = false;
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (_isDragging)
        {
            Vector3 actualMouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(actualMouseLocation.x, actualMouseLocation.y, transform.position.z);
        }
    }

    public void AddProperty(string propName, string propVal, string pointVal = "0")
    {
        CardProperty newProp = new CardProperty(propName, propVal, pointVal);
        if (PropertyList.Any(prop => prop.Equals(newProp)))
        {
            return;
        }
        PropertyList.Add(newProp);
    }

    public string GetProperty(string searchName)
    {
        return
            PropertyList.Where(prop => prop.PropertyName == searchName)
                .Select(prop => prop.PropertyValue)
                .FirstOrDefault();
    }

    private bool SetSprite()
    {
        try
        {
            string collectionName = PropertyList.Find(prop => prop.PropertyName == "Collection").PropertyValue;
            if (string.IsNullOrEmpty(collectionName))
                return false;
            //string spriteName = name + ".png";
            _renderer.sprite = Resources.Load<Sprite>("Sprites/" + collectionName + "/" + name);
            return true;
        }
        catch (ArgumentNullException e)
        {
            Debug.Log(e);
            return false;
        }
    }

    public void SetExpInfo(string info)
    {
        if (string.IsNullOrEmpty(_expandedInfo))
            _expandedInfo = info;
    }

    public string GetExpInfo()
    {
        return _expandedInfo;
    }

    public bool DoesPropertyExist(string propertyValue, string propertyName = "")
    {
        return string.IsNullOrEmpty(propertyName) ? PropertyList.Any(prop => prop.PropertyValue == propertyValue) : PropertyList.Any(prop => prop.PropertyName == propertyName && prop.PropertyValue == propertyValue);
    }

    public void MoveToBoard()
    {
        Vector3 rotation;
        Player p = BoardManager.Instance.FindOwningPlayer(this);
        switch (p.name.ToLower())
        {
            case "player1":
                //rotation = Vector3.zero;
                rotation = new Vector3(0.0f, 0.0f, 180.0f);
                break;
            case "player2":
                //rotation = new Vector3(0.0f, 0.0f, 90.0f);
                rotation = new Vector3(0.0f, 0.0f, 180.0f);
                break;
            case "player3":
                rotation = new Vector3(0.0f, 0.0f, 180.0f);
                break;
            case "player4":
                // rotation = new Vector3(0.0f, 0.0f, -90.0f);
                rotation = new Vector3(0.0f, 0.0f, 180.0f);
                break;
            default:
                rotation = Vector3.zero;
                break;
        }
        p.PlaceCard(this, rotation);
        _renderer.enabled = true;
    }

    public bool IsSelected()
    {
        return _isSelected;
    }

    public void SetIsSelected(bool selected)
    {
        _isSelected = selected;
        if (_isOnBoard)
            return;
        Player p = BoardManager.Instance.FindOwningPlayer(this);
        if (selected)
        {
            char changeAxis;
            float changeMagnitude;
            switch (p.name.ToLower())
            {
                case "player1":
                    changeAxis = 'y';
                    changeMagnitude = -1.0f;
                    break;
                case "player2":
                    //changeAxis = 'x';
                    changeAxis = 'y';
                    changeMagnitude = 1.0f;
                    break;
                case "player3":
                    changeAxis = 'y';
                    changeMagnitude = 1.0f;
                    break;
                case "player4":
                    //changeAxis = 'x';
                    //changeMagnitude = -1.0f;
                    changeAxis = 'y';
                    changeMagnitude = 1.0f;
                    break;
                default:
                    return;
            }
            _originalPosition = transform.position;
            if (changeAxis == 'x')
                transform.position += new Vector3(changeMagnitude, 0.0f);
            else
                transform.position += new Vector3(0.0f, changeMagnitude);
        }
        else
        {
            transform.position = _originalPosition;
        }


    }

    public bool IsOnBoard()
    {
        return _isOnBoard;
    }

    public void SetIsOnBoard(bool onBoard)
    {
        _isOnBoard = onBoard;
    }

    public List<CardProperty> FindCommonProperties(Card other)
    {
        return other.PropertyList.Where(prop => DoesPropertyExist(prop.PropertyValue, prop.PropertyName)).Distinct().ToList();
    }
}
