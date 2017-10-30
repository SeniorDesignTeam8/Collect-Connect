using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenuButton : MonoBehaviour {

	public Button mainButton; 
	// Use this for initialization
	void Start () {

		mainButton.GetComponent<Button>().onClick.AddListener (LoadMainMenu);
	}

	public static void LoadMainMenu()
	{
        BoardManager.Instance.loadingScreen.gameObject.SetActive(true);
        BoardManager.ResetPassArray();
        BoardManager.CurrentPhase = GamePhase.PreGame;
     
        SceneManager.LoadScene ("mainMenu");
	}

	// Update is called once per frame
	void Update () {
		
	}
}
