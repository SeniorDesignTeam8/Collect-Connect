using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inactiveObject : MonoBehaviour {

    PlayerLogic player;

	void Start ()
    {
        player = GetComponentInChildren<PlayerLogic>();
	}

    public void makeInactive()
    {

    }

}
