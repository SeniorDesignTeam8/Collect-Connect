using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplBoard : MonoBehaviour
{

    [SerializeField]
    int numOptions;

    GameObject holderPF;
    GameObject [] spots;
	// Use this for initialization
	void Start ()
    {
        spots = new GameObject[numOptions];
	}

    public void createBoard()
    {
        // make board and place holders equal distance aprt 

    }
    public void dealCards()
    {
        // deal to parent slot
        //deal to children slot
            // acrtivate children slot so they can picked up 
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
