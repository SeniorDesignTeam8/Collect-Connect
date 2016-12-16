using UnityEngine;
using System.Collections;
using System;

public class makeInvisible : MonoBehaviour
{
	// Use this for initialization
    void Start()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
    }
}
