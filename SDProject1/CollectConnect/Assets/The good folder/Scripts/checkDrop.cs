using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class checkDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool ableArrange;
    bool isValidSpot;

    public void OnDrop(PointerEventData eventData)
    {
        Dragable d = eventData.pointerDrag.GetComponent<Dragable>();
       
            d.lastLocation = transform;
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (transform.tag == "hand")
        {
            ableArrange = true;
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(transform.tag=="hand")
        {
            ableArrange = false;
        }
    }



    // Use this for initialization
    void Start ()
    {
        ableArrange = true;

	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
