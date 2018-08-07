using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CanvasGroup cgroup;
    public Transform lastLocation;
    public Transform hand;
    public GameObject moveArea;
    RectTransform rectTrans;
    GameObject placeholder;
    
    public bool canBeMoved=true;


    //when the card is selected it creates a placeholder in its spot
    //this allwos the player to be able to arrange their hand
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canBeMoved)
        {
            placeholder = new GameObject();
            placeholder.transform.SetParent(transform.parent);
            RectTransform rt = placeholder.AddComponent<RectTransform>();
            rt = rectTrans;
            placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());

            lastLocation = transform.parent;
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
            checkDrop area = hand.GetComponent<checkDrop>();
            if (area.ableArrange)
                arrangeHand();
        }
    }


    // When the player lets go of the card it will either stay where they dropped it if valid
    //or snap back to the last valid location 
    //updates the list of items on the board
    //does not allow the player to place more than one card on the board at a time
    public void OnEndDrag(PointerEventData eventData)
    {
        if (canBeMoved)
        {
            if (transform.parent.gameObject != hand && lastLocation.childCount > 0)
            {
                transform.SetParent(hand);
            }
            else
                 transform.SetParent(lastLocation);
            transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            Destroy(placeholder);

            cgroup.blocksRaycasts = true;
            GameObject.Find("mainCanvas").GetComponent<Board>().addToListEnd(transform);
            //GameObject.Find("mainCanvas").GetComponent<Board>().limitActiveObjects();
        }
 
    }

    void arrangeHand()
    {
        for (int i = 0; i < lastLocation.childCount; i++)
        {
            if (transform.position.x < lastLocation.GetChild(i).position.x)
            {
                placeholder.transform.SetSiblingIndex(i);
                break;
            }
        }
    }





    // Use this for initialization
    void Start()
    {
       // canBeMoved = true;
        rectTrans = GetComponent<RectTransform>();
        cgroup = GetComponent<CanvasGroup>();
        moveArea = GameObject.Find("mainCanvas");
        
       hand = lastLocation = transform.parent;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
