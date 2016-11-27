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
        public int PointValue;

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
    private string _name;
    private List<CardProperty> _propertyList = new List<CardProperty>();

    // Use this for initialization
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {

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

    public bool SetSprite()
    {
        try
        {
            string collectionName = _propertyList.Find(prop => prop.PropertyName == "Collection").PropertyValue;
            string spriteName = _propertyList.Find(prop => prop.PropertyName == "SpriteName").PropertyValue;
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
}
