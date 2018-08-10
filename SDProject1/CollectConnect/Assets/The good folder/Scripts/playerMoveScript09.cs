using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMoveScript09 : MonoBehaviour {

    public turnSystemScript09 turnSystem;
    public TurnClass09 turnClass;
    public bool isTurn = false;
    public KeyCode moveKey;

	// Use this for initialization
	void Start () {
        turnSystem = GameObject.Find("Turn-basedSystem").GetComponent<turnSystemScript09>();

        foreach (TurnClass09 tc in turnSystem.playersGroup)
        {
            if (tc.playerGameObject.name == gameObject.name) turnClass = tc;
        }
	}
	
	// Update is called once per frame
	void Update () {
        isTurn = turnClass.isTurn;

        if (isTurn)
        {
            // quick way to switch turns (testing)
            if (Input.GetKeyDown(moveKey))
            {
             
                
                isTurn = false;
                turnClass.isTurn = isTurn;
                turnClass.wasTurnPrev = true;

            }
        }
	}
}
