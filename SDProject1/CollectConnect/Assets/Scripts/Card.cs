using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = PropertyName != null ? PropertyName.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (PropertyValue != null ? PropertyValue.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ PointValue;
                return hashCode;
            }
        }
    }

    private SpriteRenderer _renderer;
    private bool _isOnBoard;
    private string _name;
    private readonly List<CardProperty> _propertyList = new List<CardProperty>();

    // Use this for initialization
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.enabled = false;
        if (SetSprite())
            _isOnBoard = false;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (_isOnBoard && !_renderer.enabled)
        {
            _renderer.enabled = true;
        }
        else if (!_isOnBoard && _renderer.enabled)
        {
            _renderer.enabled = false;
        }
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
            string spriteName = _name + ".png";
            _renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(@"Assets\Sprites\" + collectionName + @"\" + spriteName);

        }
        catch (ArgumentNullException)
        {
            return false;
        }
        return true;
    }

    public void SetName(string cardName)
    {
        _name = cardName;
    }

    public bool DoesPropertyExist(string propertyValue, string propertyName = "")
    {
        return string.IsNullOrEmpty(propertyName) ? _propertyList.Any(prop => prop.PropertyValue == propertyValue) : _propertyList.Any(prop => prop.PropertyName == propertyName && prop.PropertyValue == propertyValue);
    }
}
