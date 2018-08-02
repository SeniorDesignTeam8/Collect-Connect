using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InflateCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Cursor Entering " + name + " GameObject");
        if (transform.tag=="card")
        {
            inflate();
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
        Component [] images = transform.GetComponentsInChildren<Image>();
        Image wordBack =transform.GetComponentInChildren<Image>();
        foreach (Image x in images)
        {
            if (x.tag == "wordBacking")
            {
                wordBack = x;
            }

        }
        Text des= transform.GetComponentInChildren<Text>();
        des.enabled = true;
        wordBack.enabled = true;
       transform.localScale+=new Vector3(2f,2f,0);
        

    }
    public void deflate()
    {
        Component[] images = transform.GetComponentsInChildren<Image>();
        Image wordBack = transform.GetComponentInChildren<Image>();
        foreach (Image x in images)
        {
            if (x.tag == "wordBacking")
            {
                wordBack = x;
            }

        }
        Text des = transform.GetComponentInChildren<Text>();
        des.enabled = false;
        wordBack.enabled = false;
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
