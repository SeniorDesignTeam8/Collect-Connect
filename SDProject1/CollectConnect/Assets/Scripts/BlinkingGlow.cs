using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingGlow : MonoBehaviour {

	//this script takes care of the blinking glow that appears behind the keyword lsits in the keyword phase
	// TO DO: Set the blinking.setactive to false when it is no longer that players turn
	public GameObject p2KeywordGlow;
	public GameObject p3KeywordGlow;
	public GameObject p4KeywordGlow;
	private float blinkTime;

	// Use this for initialization
	void Start ()
	{
		//arrow = this.GetComponent<SpriteRenderer>();
		//StartBlinking ();
		//InvokeRepeating("Blink", 0, 0.4f);
		blinkTime = 0.5f; //How often it should blink
	}


	void LateUpdate()
	{
		if (BoardManager.CurrentPhase == GamePhase.Research) //if it is Keyword phase
		{
			if (blinkTime <= 0f) //if .5 seconds has passed
			{
				switch (BoardManager.Instance.CurrentPlayer)
				{
				case 1: //player 2
					Blink (p2KeywordGlow);
					break;
				case 2: //player 3
					Blink (p3KeywordGlow);
					break;
				case 3: //player 4
					Blink (p4KeywordGlow);
					break;
				}

				blinkTime = 0.5f; //reset blink time
			} 
			else 
			{
				blinkTime -= Time.deltaTime; //if .5 seconds hasnt passed subtract the delta time
			}
		}
		else //if it is not the keyword phase then set all the blinking to false so it doesnt show up
		{
			p2KeywordGlow.SetActive (false);
			p3KeywordGlow.SetActive (false);
			p4KeywordGlow.SetActive (false);
		}
	}
		
	private static void Blink(GameObject listGlow)
	{

		switch (listGlow.activeSelf) {
		case true:
			listGlow.SetActive (false);
			break;

		case false:
			listGlow.SetActive (true);
			break;
		}
	}
}
