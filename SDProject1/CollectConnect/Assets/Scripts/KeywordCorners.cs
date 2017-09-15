using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeywordCorners : MonoBehaviour {


	public  bool[] cornerFilled = {
		false, false, false, false // Top Left, Top Right, Bottom Right, Bottom Left (Clockwise)
	};

	public Vector3[] cornerPos = new Vector3[4];

	// Use this for initialization
	void Start () 
	{
		BoxCollider2D rend = this.gameObject.GetComponent<BoxCollider2D> ();

		cornerPos [0] = new Vector3 (rend.bounds.min.x, rend.bounds.max.y, 1); //top left
		cornerPos[1] = new Vector3	(rend.bounds.max.x, rend.bounds.max.y, 1); //top right
		cornerPos[2] = new Vector3	(rend.bounds.max.x, rend.bounds.min.y, 1); //Bottom right
		cornerPos[3] = new Vector3	(rend.bounds.min.x, rend.bounds.min.y, 1); //Bottom Left
	}
	
	public void SetCorners()
	{
		BoxCollider2D rend = this.gameObject.GetComponent<BoxCollider2D> ();

		cornerPos [0] = new Vector3 (rend.bounds.min.x, rend.bounds.max.y, 1); //top left
		cornerPos[1] = new Vector3	(rend.bounds.max.x, rend.bounds.max.y, 1); //top right
		cornerPos[2] = new Vector3	(rend.bounds.max.x, rend.bounds.min.y, 1); //Bottom right
		cornerPos[3] = new Vector3	(rend.bounds.min.x, rend.bounds.min.y, 1); //Bottom Left

		Debug.Log (cornerPos [0].ToString () + "     " + cornerPos [1].ToString ());
	}

	public float GetCorner(int i, char variable)
	{

		if (variable == 'x')
			return cornerPos [i].x;
		else if (variable == 'y')
			return cornerPos [i].y;
		else
			return cornerPos [i].z;
	}


}
