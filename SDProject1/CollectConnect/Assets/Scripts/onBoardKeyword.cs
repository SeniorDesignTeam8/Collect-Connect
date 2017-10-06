using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class onBoardKeyword : MonoBehaviour {
	
	//public GameObject KeywordPrefab;

	// Use this for initialization
	public void onBoardKey (GameObject curr)
	{
		Debug.Log (curr.ToString ());
		GameObject go = curr;
		Debug.Log (go.ToString ());
		go.GetComponentInChildren<Text> ().resizeTextForBestFit = true;
		go.GetComponentInChildren<Text> ().resizeTextMaxSize = 100;
		go.GetComponentInChildren<Text> ().resizeTextMinSize = 1;
		go.GetComponentInChildren<Text> ().fontStyle = FontStyle.Bold;
		go.GetComponentInChildren<Text> ().alignByGeometry = true;

		Button btn = go.GetComponent<Button> ();

		ColorBlock btnColors = go.GetComponent<Button> ().colors;
		btnColors.normalColor = Color.white;
		btnColors.highlightedColor = Color.yellow;
		btnColors.pressedColor = Color.grey;

		btn.colors = btnColors;

		btn.onClick.AddListener (() => 
			{
				Debug.Log(go.GetComponentInChildren<Text>().text + " Clicked!");
                BoardManager.Instance.PlaySelect ();
				BoardManager.Instance._currentKeyword = go.GetComponentInChildren<Text> ().text;
				//Debug.Log(BoardManager.Instance._currentKeyword);
			});
		
	
		//go.transform.Rotate(0.0f, 0.0f, -90.0f);
		go.SetActive (true);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
		
}
