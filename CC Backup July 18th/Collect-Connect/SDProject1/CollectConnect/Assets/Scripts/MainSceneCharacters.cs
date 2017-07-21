using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MainSceneCharacters : MonoBehaviour {

	public SpriteRenderer player1Image; //Player Images (pulled spriterenderers directly)
	public SpriteRenderer player2Image;
	public SpriteRenderer player3Image;
	public SpriteRenderer player4Image;
	public List<SpriteRenderer> Images; //Made a list for the player images


	// Use this for initialization
	void Awake () 
	{
		DontDestroyOnLoad(this);
		//Debug.Log (PlayerPrefs.GetString ("Player1Key"));
		//player1Image.sprite = Resources.Load<Sprite>(PlayerPrefs.GetString ("Player1Key"));
		//SetSprites ();
		Images.Add (player1Image); //Adding the images to the list
		Images.Add (player2Image);
		Images.Add (player3Image);
		Images.Add (player4Image);

		//For loop to go throught he list and set all the images
		for (int i = 1; i <= Images.Count; i++) 
		{
			SetSprites (Images [i - 1], i); //i-1 so there is null exception
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	//Sets the sprites images
	void SetSprites(SpriteRenderer sprites, int count)
	{
		string playerKey = "Player" + count.ToString() + "Key"; //Gets the correct image name based on count

		//Debug.Log(PlayerPrefs.GetString(playerKey));

		//Try catch to set the images
		try
		{
			//StartCoroutine(ApplySprite());
			byte[] fileData = File.ReadAllBytes(Application.dataPath + "/AvatarFolder/" + PlayerPrefs.GetString (playerKey) + ".jpg"); //Pulls the application data path and checks the correct folder
			Texture2D tex = new Texture2D(2, 2); 
			tex.LoadImage(fileData); //Loads the image
			Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 75.0f); //Sets to a temp variable
			sprites.sprite = mySprite; //Sets the image(that was passed in) to the temp variable image
		}
		catch (DirectoryNotFoundException e)
		{
			Debug.Log(e); //Displays error if it occurs
		}
	}
}