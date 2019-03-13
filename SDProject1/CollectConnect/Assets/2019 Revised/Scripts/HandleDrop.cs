using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandleDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    PlayerLogic player;
    bool valid;

    public void OnDrop(PointerEventData eventData)
    {
        DragItems item = eventData.pointerDrag.GetComponent<DragItems>();

        if (item != null)
        { 
            if (transform.tag == "hand" && valid )
            {
                item.lastLocation = transform;
            }
            else 
            {
                item.lastLocation = item.spawn;
            }
        }     
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (transform.tag == "hand")
        {
            if (transform.childCount >= player.holdAmount)
                valid = false;
            else
                valid = true;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        valid = false;        
    }

    void Start()
    {
        player = GetComponent<PlayerLogic>();
        valid = false;

    }


}
