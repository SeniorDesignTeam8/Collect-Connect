using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class connectionPanel : MonoBehaviour
{
    GameObject [] players;
    public float offset;
    RectTransform rect;
	// Use this for initialization
	void Awake ()
    {
        rect = gameObject.GetComponent<RectTransform>();
        players = GameObject.FindGameObjectsWithTag("Player");
	}

    // Update is called once per frame
    private void OnEnable()
    {
       if(!players[0].GetComponentInChildren<PlayerLogic>().turn)
        {
            transform.SetParent(players[0].transform);
            
           // transform.position = Vector3.zero;
        }
       else
        {
            transform.SetParent(players[1].transform);
           // transform.position = Vector3.zero;
        }
        transform.position = new Vector3(transform.parent.position.x , transform.position.y, 0f);
        if (rect.position.x<300)
            transform.position = new Vector3(transform.parent.position.x + offset, transform.position.y, 0f);
       else
            transform.position = new Vector3(transform.parent.position.x - offset, transform.position.y, 0f);
    }
}
