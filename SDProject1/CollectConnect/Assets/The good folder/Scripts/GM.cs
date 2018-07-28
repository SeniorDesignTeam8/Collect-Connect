using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM : MonoBehaviour
{
    public Button confirm;
    public GameObject player;

	public void confirmButtonPress()
    {
        if(player.transform.childCount==3)
        {

        }
        //else invalid turn

    }
    void Start()
    {
        player = GameObject.Find("Player1");
    }
	
}
