using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InflateCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Cursor Entering " + name + " GameObject");
        if (transform.tag=="card")
        {
            Invoke("inflate", .5f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Cursor Entering " + name + " GameObject");
        if (transform.tag == "card")
        {
          deflate();
        }
    }

    public void inflate()
    {
       transform.localScale+=new Vector3(2f,2f,0);

    }
    public void deflate()
    {
        transform.localScale -= new Vector3(2f, 2f, 0);
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
