using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridClass : MonoBehaviour {

	public List<GameObject>[] PlayedKeywords = new List<GameObject>[20];
	public int columnCount;
	public int rowCount;
	public int listCount;

	// Use this for initialization
	void Start () 
	{
		columnCount = 1;
		rowCount = 1;
		listCount = 0;

		SetUpGrid ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Testing a Grid system below this comment
	//WILL MOVE THE CODE TO SEPERATE SCRIPT LATER


	//	transform.position = SnapToGrid(transform,GridSpacing);
	private void SetUpGrid()
	{
		if (columnCount == 5)
		{
			columnCount = 1;
			rowCount++;
		}

	}

	public Vector3 SnapToGrid(GameObject Key)
	{
		PlayedKeywords [listCount] = Key;
		listCount++;
	}

	public void SwitchKeywords()
	{		
//		GameObject temp = selected1;
//		selected1 = selected2;
//		selected2 = temp;
	}

}
