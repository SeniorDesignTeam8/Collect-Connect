using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;

public class Card : MonoBehaviour
{
    private struct CardProperty
    {
        public readonly string PropertyName;
        public readonly string PropertyValue;
        private int PointValue;

        public CardProperty(string name, string value, string pointString = "0")
        {
            PropertyName = name;
            PropertyValue = value;
            if (!int.TryParse(pointString, out PointValue))
                PointValue = 0;
        }

        public bool Equals(CardProperty other)
        {
            return string.Equals(PropertyName, other.PropertyName) && string.Equals(PropertyValue, other.PropertyValue) && PointValue == other.PointValue;
        }

        public void SetPointValue(int newValue)
        {
            PointValue = newValue;
        }
    }

    public AudioClip SelectSound;

    private SpriteRenderer _renderer;
    private const float ExpandedInfoDelay = 2.0f; // Time to wait before expanding.
    private float _mouseDownTime; // Time when last clicked, in seconds since game start.
    private bool _isOnBoard; // Specifies if the card is in play or in the deck.
    private bool _isExpanded; // Specifies if the card is currently in expanded view.


    private static bool _isAnySelected; // Specifies if a card is selected.
    private bool _isSpriteLoaded;
    private bool _isThisSelected; // Specifies if this card is selected.
    private bool _isTimerRunning; // If true, mouse is currently held down on card.
    private string _expandedInfo; // Information to display in expanded view.
    private readonly List<CardProperty> _propertyList = new List<CardProperty>();

    // Use this for initialization
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        _mouseDownTime = Time.time; // Mark the current time as 0. After 2 seconds, expand card.
        _isTimerRunning = true;
    }

    private void OnMouseUp()
    {
        _isTimerRunning = false;
        if (_isExpanded) // Is the card expanded?
        {
            // TODO: Shrink the card to regular size.
            _isExpanded = false;
        }
        else if (!_isAnySelected) // Select this card.
        {
            _isAnySelected = true;
            // TODO: Mark card as selected.
            _isThisSelected = true;
        }
        else if (_isThisSelected) // A card is selected, but is it this one?
        {
            _isThisSelected = false; // If so, then deselect this card.
            // TODO: Deselect card.
            _isAnySelected = false;
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
        // TODO: Expand Card.
        _isExpanded = true;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        //if (_isOnBoard && !_renderer.enabled)
        //{
        //    _renderer.enabled = true;
        //}
        //else if (!_isOnBoard && _renderer.enabled)
        //{
        //    _renderer.enabled = false;
        //}
    }

    public void AddProperty(string propName, string propVal, string pointVal = "0")
    {
        CardProperty newProp = new CardProperty(propName, propVal, pointVal);
        if (_propertyList.Any(prop => prop.Equals(newProp)))
        {
            return;
        }
        _propertyList.Add(newProp);
    }

    public string GetProperty(string searchName)
    {
        return
            _propertyList.Where(prop => prop.PropertyName == searchName)
                .Select(prop => prop.PropertyValue)
                .FirstOrDefault();
    }

    private bool SetSprite()
    {
        try
        {
            string collectionName = _propertyList.Find(prop => prop.PropertyName == "Collection").PropertyValue;
            if (string.IsNullOrEmpty(collectionName))
                return false;
            string spriteName = name + ".png";
            _renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(@"Assets/Sprites/" + collectionName + "/" + spriteName);
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
        return string.IsNullOrEmpty(propertyName) ? _propertyList.Any(prop => prop.PropertyValue == propertyValue) : _propertyList.Any(prop => prop.PropertyName == propertyName && prop.PropertyValue == propertyValue);
    }

    public void MoveToBoard(Player p)
    {
        Vector3 rotation;

        switch (p.name.ToLower())
        {
            case "player1":
                rotation = Vector3.left;
                break;
            case "player2":
                rotation = Vector3.zero;
                break;
            case "player3":
                rotation = Vector3.right;
                break;
            case "player4":
                rotation = Vector3.up;
                break;
            default:
                rotation = Vector3.zero;
                break;
        }
        transform.Rotate(rotation);
        p.PlaceCard(this);
        _renderer.enabled = true;
        _isOnBoard = true;
    }
}
