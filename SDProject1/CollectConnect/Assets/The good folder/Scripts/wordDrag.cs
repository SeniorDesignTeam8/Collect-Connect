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


    //when the word is selected it 
    public void OnBeginDrag(PointerEventData eventData)
    {
     
        lastLocation = transform.parent;
        transform.SetParent(moveArea.transform);
        cgroup.blocksRaycasts = false;
    }

    // events that can occur while moving the word around the screen 
    //
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        checkDrop area = bank.GetComponent<checkDrop>();
    }


    // When the player lets go of the word it will either stay where they dropped it if valid
    //or snap back to the last valid location 
    public void OnEndDrag(PointerEventData eventData)
    {
        if(transform.parent.gameObject!= bank && lastLocation.childCount>0)
        {
            transform.SetParent(bank);
        }
        else
            transform.SetParent(lastLocation);
        cgroup.blocksRaycasts = true;
    
        GameObject.Find("mainCanvas").GetComponent<Board>().addToListEnd(transform);

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
