using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandleDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{

    bool valid;

    public void OnDrop(PointerEventData eventData)
    {
        DragItems item = eventData.pointerDrag.GetComponent<DragItems>();

            if (transform.tag == "hand" && valid )
            {
                Debug.Log("Dropping on Hand");
                item.lastLocation = transform;

            }
            else 
            {
                item.lastLocation = item.spawn;
            }

       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (transform.tag == "hand")
        {
            Debug.Log("On hand");
            valid = true;
        }
        else if (transform.tag=="return")
            Debug.Log("Spawn");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        valid = false;
        
    }



    // Use this for initialization
    void Start()
    {
        valid = false;

    }


}
