using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridClass : MonoBehaviour {

	public List<GameObject> PlayedKeywords ;
	public int columnCount;
	public int rowCount;
	public int listCount;
	public GameObject tile;

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

		for(int i = 0;i<4;i++) //columns
		{
			for(int j = 0;j<5;j++) //rows
			{
				PlayedKeywords.Add(Instantiate(tile,new Vector3(-8+(i*5), 5-j*2, 0),Quaternion.identity));
			}
		}
	}

	public void SnapToGrid(GameObject Key)
	{
		Key.transform.position = PlayedKeywords [listCount].transform.position;
		PlayedKeywords [listCount] = Key.gameObject;
		listCount++;
	}

	public void SwitchKeywords()
	{		
//		GameObject temp = selected1;
//		selected1 = selected2;
//		selected2 = temp;
	}

}
