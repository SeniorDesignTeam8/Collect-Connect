using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandleDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerLogic player;
    public GameObject dropArea;
    returnFirstChild backToSpawn;
    bool valid;

    public void OnDrop(PointerEventData eventData)
    {
        DragItems item = eventData.pointerDrag.GetComponent<DragItems>();

        if (item != null)
        { 
            if (transform.tag == "hand" && valid )
            {
                if (dropArea.transform.childCount >= 1 && backToSpawn != null)//player.holdAmount)
                {
                    backToSpawn.sendBack();
                }
                item.lastLocation = dropArea.transform;
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
            valid = true;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        valid = false;        
    }

    void Start()
    {
        backToSpawn = GetComponent<returnFirstChild>();
        valid = false;

    }


}
