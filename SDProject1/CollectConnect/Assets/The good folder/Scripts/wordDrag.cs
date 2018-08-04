using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class wordDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CanvasGroup cgroup;
    public Transform lastLocation;
    public Transform bank;
    public GameObject moveArea;
    RectTransform rectTrans;
    GameObject placeholder;

    public bool canBeMoved;


    //when the card is selected it 
    public void OnBeginDrag(PointerEventData eventData)
    {
     
        lastLocation = transform.parent;
        transform.SetParent(moveArea.transform);
        cgroup.blocksRaycasts = false;
    }

    // events that can occur while moving the card around the screen 
    //
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        checkDrop area = bank.GetComponent<checkDrop>();
    }


    // When the player lets go of the card it will either stay where they dropped it if valid
    //or snap back to the last valid location 
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(lastLocation);
        cgroup.blocksRaycasts = true;

    }

    // Use this for initialization
    void Start()
    {

        rectTrans = GetComponent<RectTransform>();
        cgroup = GetComponent<CanvasGroup>();
        moveArea = GameObject.Find("mainCanvas");

        bank = lastLocation = transform.parent;

    }

}
