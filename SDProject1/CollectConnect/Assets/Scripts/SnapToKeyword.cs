using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToKeyword : BoardManager {

	public static void CornerSnap(Card cardA, Card boardCard, GameObject keyword)
	{
		BoxCollider2D cardARend = cardA.gameObject.GetComponent<BoxCollider2D> ();
		BoxCollider2D boardRend = boardCard.gameObject.GetComponent<BoxCollider2D> ();
		KeywordCorners keyCorn = keyword.gameObject.GetComponent<KeywordCorners> ();

		keyword.GetComponent<KeywordCorners> ().SetCorners ();  //Set the new corners to the board locations

		//if (boardCard.isSnapped != true) 
		if (boardCard.isSnapped == true && boardCard.GetAttachedTo() != keyword) 
		{
			ClearAttachedCorner (boardCard);
		}

		for (int i = 0; i < keyword.GetComponent<KeywordCorners>().cornerFilled.Length; i++)
		{
			if (keyword.GetComponent<KeywordCorners>().cornerFilled [i] == false) 
			{
				cardA.transform.position = Coordinates (i, cardARend, keyCorn); //Set the new corners to the board locations
				cardA.SetAttachedKey(keyword.gameObject);
				cardA.SetOnCorner (i);
				keyword.GetComponent<KeywordCorners>().cornerFilled [i] = true;
				break;
			} else
			{
				Debug.Log ("Corner " + i.ToString() + " Filled");
			}
		}

//	
		if(boardCard.GetAttachedTo() != keyword)
		{
			for (int i = 0; i < keyword.GetComponent<KeywordCorners>().cornerFilled.Length; i++)
			{
				if (keyword.GetComponent<KeywordCorners>().cornerFilled [i] == false) 
				{
				
					boardCard.transform.position = Coordinates (i, boardRend, keyCorn);  //Set the new corners to the board locations
					boardCard.SetAttachedKey(keyword.gameObject);
					boardCard.SetOnCorner (i);
					keyword.GetComponent<KeywordCorners>().cornerFilled [i] = true;
					boardCard.SetSnapped (true);
					break;
				} 
				else 
				{
					Debug.Log ("Corner " + i.ToString() + " Filled");
				}
			}
		}
	}

	public static Vector3 Coordinates(int i, BoxCollider2D cardRend, KeywordCorners keyCorn)
	{
		switch (i) 
		{
		case 0: //top left
			return	new Vector3 (keyCorn.GetCorner (i, 'x') - cardRend.bounds.extents.x, keyCorn.GetCorner (i, 'y') + cardRend.bounds.extents.y, 1); 
		case 1: //topright
			return	new Vector3 (keyCorn.GetCorner (i, 'x') + cardRend.bounds.extents.x, keyCorn.GetCorner (i, 'y') + cardRend.bounds.extents.y, 1); 
		case 2: //bottom right
			return	new Vector3 (keyCorn.GetCorner (i, 'x') + cardRend.bounds.extents.x, keyCorn.GetCorner (i, 'y') - cardRend.bounds.extents.y, 1); 
		case 3: //bottom left
			return	new Vector3 (keyCorn.GetCorner (i, 'x') - cardRend.bounds.extents.x, keyCorn.GetCorner (i, 'y') - cardRend.bounds.extents.y, 1); 
		default:
			Debug.Log ("Coordinates Broke");
			return new Vector3 (1, 1, 1);
		}
	}

	public static void ClearAttachedCorner(Card boardCard)
	{
		GameObject keyWord = boardCard.GetAttachedTo ();
		int cornerNum = boardCard.GetOnCorner ();
		keyWord.GetComponent<KeywordCorners> ().cornerFilled [cornerNum] = false;
	}
}