using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetImage : MonoBehaviour {

	public SpriteRenderer NewImage;

	// Use this for initialization
	void Start ()
	{
		this.GetComponent<SpriteRenderer> ().sprite = NewImage.sprite;
	}

}
