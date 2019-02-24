using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragItems : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CanvasGroup cgroup;
    public Transform lastLocation;
    public Transform spawn;
    public GameObject moveArea;
    public bool canBeMoved = false;


    //when the card is selected it creates a placeholder in its spot
    //this allwos the player to be able to arrange their hand
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canBeMoved)
        {
         //   lastLocation = transform.parent;
            transform.SetParent(moveArea.transform);
            cgroup.blocksRaycasts = false;
        }
    }

    // events that can occur while moving the card around the screen 
    //
    public void OnDrag(PointerEventData eventData)
    {
        if (canBeMoved)
        {
            transform.position = eventData.position;
         }
    }


    // When the player lets go of the card it will either stay where they dropped it if valid
    //or snap back to the last valid location 
    public void OnEndDrag(PointerEventData eventData)
    {
        if (canBeMoved)
        {
            if (lastLocation.tag != "hand"|| lastLocation.tag== "return" || (lastLocation.transform.childCount > 0)) //tag == "card" &&
            {
                transform.SetParent(spawn);
            }
            else
                transform.SetParent(lastLocation);
 
            cgroup.blocksRaycasts = true;
        }

    }






    // Use this for initialization
    void Start()
    {
        // canBeMoved = true;
        cgroup = GetComponent<CanvasGroup>();
       // holdAmount = GetComponent<HoldAmount>();
        moveArea = GameObject.Find("Canvas");
        spawn = lastLocation = transform.parent;

    }



}

