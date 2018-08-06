using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InflateCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject cardDisplay;
    GameObject copy;
    public void OnPointerEnter(PointerEventData eventData)
    {
       // Debug.Log("Cursor Entering " + name + " GameObject");
        if (transform.tag=="card")
        {
            // inflate();
            display();

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       // Debug.Log("Cursor Entering " + name + " GameObject");
        if (transform.tag == "card")
        {
            // deflate();
           // removeDisplay();
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
    public void infalteCopy()
    {
        Component[] images = copy.transform.GetComponentsInChildren<Image>();
        Image wordBack = copy.transform.GetComponentInChildren<Image>();
        foreach (Image x in images)
        {
            if (x.tag == "wordBacking")
            {
                wordBack = x;
            }

        }
        Text des = copy.transform.GetComponentInChildren<Text>();
        des.enabled = true;
        wordBack.enabled = true;
        copy.transform.localScale += new Vector3(.5f, .5f, 0);
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
    public void display()
    {
        if (cardDisplay.transform.childCount > 0)
        {
            Transform[] children = cardDisplay.GetComponentsInChildren<Transform>();
            for (int i = 1; i < children.Length; i++)
            {
                Destroy(children[i].gameObject);
            }

        }
        copy = Instantiate(gameObject);
        Destroy(copy.GetComponent<InflateCard>());
        Destroy(copy.GetComponent<Dragable>());
        copy.transform.SetParent(cardDisplay.transform);
        infalteCopy();
    }


    // Use this for initialization
    void Start ()
    {
        cardDisplay = GameObject.Find("DisplayCard");
	}
	
}
