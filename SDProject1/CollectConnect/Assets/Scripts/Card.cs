using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;


public class Card : MonoBehaviour
{
    public struct CardProperty
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
            //Debug.Log("Setting point value to " + newValue);
            PointValue = newValue;
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
    private Vector3 _offset;
    private Vector3 _screenPoint;
    private bool _isSpriteLoaded;
    private bool _isSelected; // Specifies if this card is selected.
    private bool _isTimerRunning; // If true, mouse is currently held down on card.
    private string _expandedInfo; // Information to display in expanded view.
    public List<CardProperty> PropertyList = new List<CardProperty>();
    private string _imageLocation;
	public bool isHorizontal;
	public int cardID;
	public bool isSnapped;
	public GameObject attachedKeyNode;
	public int atCorner;

    public string cardLang;
    public string cardLoc;
    public string cardMed;
    public string cardPeople;
    public string cardYear;
    public string cardSourceLoc;
    public Sprite cardFullPic;
    public Sprite cardSmallPic;
    public GameObject CardEnhancePlace;

    // Use this for initialization
    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _renderer = GetComponent<SpriteRenderer>();
        LineList = new List<LineRenderer>();
		isHorizontal = false;
		isSnapped = false;
        CardEnhancePlace = GameObject.FindGameObjectWithTag("EnhanceCardGameObject");
        //CardGlow.GetComponent<Renderer>().enabled = false;
    }

    private void OnMouseDown()
    {
        //Debug.Log("Mouse Down.");
        _mouseDownTime = Time.time; // Mark the current time as 0. After 2 seconds, expand card.
        _pointerDownPosition = Input.mousePosition;
        _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(_pointerDownPosition.x, _pointerDownPosition.y, _screenPoint.z));
        
        _isTimerRunning = true;
    }

    private void OnMouseUp()
    {
        //Debug.Log("Mouse Up.");
        _isTimerRunning = false;
        if (_isDragging)
        {
            _isDragging = false;
            return;
        }
        if (_isExpanded) // Is the card expanded?
        {
            //Debug.Log("Shrinking " + name);
            BoardManager.Instance.CardUnexpand(this);

            _isExpanded = false;
        }
		else if (IsOnBoard() && BoardManager.Instance.GetIsStarted()) // Is the card on the board? If not, then select from player's hand.
        {
            BoardManager.Instance.SelectCardOnBoard(this);
        }
		else if(BoardManager.Instance.GetIsStarted()) // It's in a player's hand. If it's the owning player's turn, then select it.
        {
            BoardManager.Instance.SelectCardInHand(this);
        }
        // Otherwise, do nothing (might change to deselect other card in future).
    }

    private void Update()
    {
        if (!_isSpriteLoaded && _imageLocation != null) // Wait for sprite to load before anything else.
        {
            _isSpriteLoaded = SetSprite();
            return;
        }
        if (!_isTimerRunning || !(Time.time - _mouseDownTime >= ExpandedInfoDelay))
            return;
        _isTimerRunning = false;
        //Debug.Log("Expanding " + name);

        BoardManager.Instance.CardExpand(this);
        _isExpanded = true;

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

    public string GetPropertyValue(string searchName)
    {
        return (from prop in PropertyList where prop.PropertyName == searchName select prop.PropertyValue).FirstOrDefault();
    }

    public int GetPts(CardProperty searchProperty)
    {
        //Debug.Log("Getting points for " + searchProperty.PropertyValue);
        //Debug.Log(searchProperty.PropertyName);
        foreach (CardProperty property in PropertyList)
        {
            //Debug.Log(property.PropertyName);
            if (property.PropertyValue == searchProperty.PropertyValue)
                return property.PointValue;
        }
        return 1;
    }

    public CardProperty GetPropertyFromKeyword(string keyword)
    {
        //Debug.Log("Getting property for " + keyword);
        foreach (CardProperty prop in PropertyList)
        {
            if (prop.PropertyValue.Equals(keyword))
                return prop;
        }
        return new CardProperty("WRONG", "WRONG");
    }

    public bool SetSprite()
    {
        try
        {
			//StartCoroutine(ApplySprite());
            byte[] fileData = File.ReadAllBytes(Application.dataPath + "/pics/" + _imageLocation);
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(fileData);

            tex = ScaleTexture(tex, 725, 900);

            cardFullPic = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));


            if (tex.width >127 && tex.height >160)
			{
				if(tex.width > tex.height)
				{
					isHorizontal = true;
					tex = ScaleTexture(tex, 128, 90);
				}
				else
				{
					tex = ScaleTexture(tex, 125, 148);
				}
			}
			else if(tex.width > tex.height)
			{
				isHorizontal = true;
			}
            //Debug.Log("Exiting ScaleTexture" + tex.width.ToString() + "  " + tex.height.ToString());

         
			Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height ), new Vector2(0.5f, 0.5f), 75.0f);

            //BoundsSetup(mySprite);

            cardSmallPic = mySprite;
            _renderer.sprite = mySprite;

            return true;
        }
        catch (DirectoryNotFoundException e)
        {
            Debug.Log(e);
            return false;
        }
    }

	public Texture2D ScaleTexture(Texture2D source,int targetWidth,int targetHeight)
	{
		Texture2D result = new Texture2D(targetWidth,targetHeight,source.format,true);
		Color[] rpixels = result.GetPixels(0);
		float incX = ((float)1/source.width)*((float)source.width/targetWidth);
		float incY = ((float)1/source.height)*((float)source.height/targetHeight);

		for(int px = 0; px < rpixels.Length; px++)
		{
			rpixels[px] = source.GetPixelBilinear(incX*((float)px%targetWidth),
				incY*((float)Mathf.Floor(px/targetWidth)));
		}

		result.SetPixels(rpixels,0);
		result.Apply();
        return result;
        
    }

    public void SetImageLocation(string loc)
    {
        _imageLocation = loc;
    }

    public string GetImageLocation()
    {
        return _imageLocation;
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
                rotation = new Vector3(0.0f, 0.0f, 180.0f);
                break;
            case "player2":
                rotation = new Vector3(0.0f, 0.0f, 180.0f);
                break;
            case "player3":
                rotation = new Vector3(0.0f, 0.0f, 180.0f);
                break;
            case "player4":
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
            Card c = this;
            _originalPosition = transform.position;

            if (c == p.PlayerHand.At(0))   //move cards to designated location on board
            {
                c.gameObject.transform.position = p.LocationOnBoard1.transform.position;
            }
            else if (c == p.PlayerHand.At(1))
            {
                c.gameObject.transform.position = p.LocationOnBoard2.transform.position;
            }
            else if (c == p.PlayerHand.At(2))
            {
                c.gameObject.transform.position = p.LocationOnBoard3.transform.position;
            }
            else if (c == p.PlayerHand.At(3))
            {
                c.gameObject.transform.position = p.LocationOnBoard4.transform.position;
            }
            else if (c == p.PlayerHand.At(4))
            {
                c.gameObject.transform.position = p.LocationOnBoard5.transform.position;
            }
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

	//Un-comment and use if loading images from the web
//	private IEnumerator ApplySprite()
//	{
//		string url = _imageLocation;
//		Texture2D tex = new Texture2D(2, 2);
//		WWW webImage = new WWW (url);
//		yield return webImage;
//		webImage.LoadImageIntoTexture(tex);
//		Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 75.0f);
//
//	
//		if(tex.width > tex.height || tex.width == tex.height)
//		{
//			isHorizontal = true;
//		}
//		_renderer.sprite = mySprite;
//
//	}

	public void SetID(int ID)
	{
		cardID = ID;
	}

	public int GetID()
	{
		return cardID;
	}

	public void SetSnapped(bool snapped) //set whether the card is already snapped to a keyword
	{
		isSnapped = snapped;
	}

	public bool GetSnapped() //is the card snapped to a keyword
	{
		return isSnapped;
	}

	public void SetAttachedKey(GameObject keyNode)
	{
		//Which Keynode is the card attached too
		attachedKeyNode = keyNode;
	}

	public GameObject GetAttachedTo()
	{
		//return which keynode the card is attached too
		return attachedKeyNode;
	}

	public void SetOnCorner(int cornerNum)
	{
		atCorner = cornerNum;
	}

	public int GetOnCorner()
	{
		//which corner is the card attached too
		return atCorner;
	}
 
    public string GetLocation()
    {
        return cardLoc;
    }

    public void SetLocation(string loc)
    {
        cardLoc += loc;
    }

    public string GetSourceLoc()
    {
        return cardSourceLoc;
    }

    public void SetSourceLoc(string loc)
    {
        cardSourceLoc = loc;
    }

    public string GetContributor()
    {
        return cardPeople; 
    }

    public void SetContributor(string peeps)
    {
        cardPeople += peeps;
    }

    public string GetMedium()
    {
        return cardMed;
    }

    public void SetMedium(string medium)
    {
        cardMed += medium;
    }

    public string GetYear()
    {
        return cardYear;
    }

    public void SetYear(string year)
    {
        cardYear += year;
    }

    public string GetLanguage()
    {
        return cardLang;
    }

    public void SetLanguage(string lang)
    {
        cardLang += lang;
    }
}
