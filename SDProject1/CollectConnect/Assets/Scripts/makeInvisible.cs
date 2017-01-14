using UnityEngine;
using System.Collections;
using System;

public class makeInvisible : MonoBehaviour
{
	// Use this for initialization
    private void Start()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
    }
}
