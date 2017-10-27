using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour {

	//This script handles the Character Selection screen
	//TO DO: Find a way to make it so players cant be the same character

	public string Player1 = "Default1"; //Default Key values for the PlayerPrefs
	public string Player2 = "Default2"; //
	public string Player3 = "Default3"; //
	public string Player4 = "Default4"; //
	public int pickingPlayer;
	public Button finalizePick;
	public Button playGame;
	public Button backButton;
	public string characterName;
	public GameObject ButtonList;
	public Button[] CharacterButtons;
	public Button charButtons;
	public Text charName;
	public Image charImage;
	public Text charDescr;
	public int numbPlayers;
	public string selectedCharacter;
	public GameObject Player1Image;
	public GameObject Player2Image;
	public GameObject Player3Image;
	public GameObject AIImage;
	public Button selCharImage;
	public Text Title; //Top text that tells the player what to do
    public int textPlayer;
    public AudioSource soundEffect;
    public AudioClip selectSound;

    private void Awake()
	{
		DontDestroyOnLoad(this);
	}

	// Use this for initialization
	void Start ()
	{
		pickingPlayer = 2;
		PlayerPrefs.GetString ("Player1Key", Player1); //Get the keys already if none then set the default keys listed above
		PlayerPrefs.GetString ("Player2Key", Player2);
		PlayerPrefs.GetString ("Player3Key", Player3);
		PlayerPrefs.GetString ("Player4Key", Player4);
		numbPlayers = PlayerPrefs.GetInt ("PlayerNumber"); //Number of players that was chosen in the previous scene
		playGame.GetComponent<Button>().onClick.AddListener(StartGame); //Create the button with the listener
		backButton.GetComponent<Button>().onClick.AddListener(BackButton); //Create the back button with the correct listener
		CharacterButtons = ButtonList.GetComponentsInChildren<Button> (); //Put the buttons in the list
		SetUpPlayerTitles (); //Function Call
		setUpCharacterButtons (); //Functrion Call
		SetUpFinalize (); //Function Call
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
		
	//This will set up the available characters as buttons
	public void setUpCharacterButtons()
	{
		foreach (Button button in CharacterButtons) //for all the characters in the list
		{

			charButtons = button.GetComponent<Button> ();

			//OnClick Listener
			charButtons.onClick.AddListener(() => {

                PlaySelect();
				charName.text = button.gameObject.name; //Name
				charImage.GetComponent<Image>().enabled = true; //Enabled = yes
				charImage.GetComponent<Image>().sprite = button.image.sprite; //button image
				charImage.GetComponent<Image>().preserveAspect = true; 
				charDescr.GetComponent<Text>().text =  button.GetComponentInParent<TextMesh>().text; //Description of the character
				selectedCharacter = button.GetComponentInParent<TextMesh>().text.ToString(); //This is the currently selected character
				selCharImage = button; //Sets the image
			});
		}
	}

	//This is what happens when the finalize pick button is pressed
	public void SetUpFinalize()
	{
		finalizePick.onClick.AddListener (() => {

            PlaySelect();
			//if the player has actually selected someone
			if(selCharImage != null)
			{
				if(numbPlayers == 1 || (numbPlayers == 2 && pickingPlayer == 3))
				{
					Title.text = "Select the character for the AI #" + pickingPlayer;
				}
				else
				{
					Title.text = "Select the character for Player " + pickingPlayer;
				}

                charName.text = ""; //Name
                charImage.GetComponent<Image>().enabled = false; //Enabled = yes
                //charImage.GetComponent<Image>().sprite = button.image.sprite; //button image
                //charImage.GetComponent<Image>().preserveAspect = true;
                charDescr.GetComponent<Text>().text = ""; //Description of the character
                textPlayer = pickingPlayer - 1;
                selCharImage.GetComponentInChildren<Text>().text = "Player " + textPlayer.ToString();
                selCharImage.GetComponentInChildren<Text>().color = Color.red;

                //Cases are which player is picking
                switch (pickingPlayer)
				{
				//Order of the Cases: 2,3,4,1)
				case 1: //Since Player 1 is always an AI this is done LAST compared to the other cases
					    AIImage.GetComponent<Image>().sprite = selCharImage.image.sprite; //Sets the image to the selected character
					    PlayerPrefs.SetString ("Player1Key", selCharImage.image.sprite.name);  //Sets the name of the key used in the MainSceneCharacter script
					    Title.text = "Press 'Play Game' to continue to the game"; //New title
					    playGame.interactable = true; //Lets the player move on to play the game
					    finalizePick.interactable = false; //Doesnt allow anyone to pick a character
                        selCharImage.GetComponentInChildren<Text>().text = "AI";
                        break;
				case 2: //Player 2 
					    Player1Image.GetComponent<Image>().sprite = selCharImage.image.sprite;
					    PlayerPrefs.SetString ("Player2Key", selCharImage.image.sprite.name);
					    pickingPlayer++;
					    break;
				case 3: //Player 3
					    Player2Image.GetComponent<Image>().sprite = selCharImage.image.sprite;
					    PlayerPrefs.SetString ("Player3Key", selCharImage.image.sprite.name);
					    pickingPlayer++;
					    break;
				case 4: //Player 3
					    Player3Image.GetComponent<Image>().sprite = selCharImage.image.sprite;
					    PlayerPrefs.SetString ("Player4Key", selCharImage.image.sprite.name);
					    Title.text = "Select the character for the AI #1";
					    pickingPlayer = 1;
					    break;

				};
				selCharImage.interactable = false;
				selCharImage = null; //Sets it as null if after the switch to make sure someone picks a new character
			}
			});
		


	}

	//Sets up which title should be used for the banner text
	private void SetUpPlayerTitles ()
	{
		if (numbPlayers == 1) 
		{
			Player2Image.GetComponentInChildren<Text> ().text = "AI";
			Player3Image.GetComponentInChildren<Text> ().text = "AI";
		}
		else if (numbPlayers == 2)
		{
			Player3Image.GetComponentInChildren<Text> ().text = "AI";
		}
	}


	//Starts the MainScene/Game
	private static void StartGame()
	{
		SceneManager.LoadScene("MainScene");
	}

	//Loads the previous scene if the back button is pressed
	private static void BackButton()
	{
		SceneManager.LoadScene("PlayerSelection");
	}


    public void PlaySelect()
    {
        if (soundEffect.isPlaying)
            soundEffect.Stop();
        soundEffect.clip = selectSound;
        soundEffect.Play();
    }

}
