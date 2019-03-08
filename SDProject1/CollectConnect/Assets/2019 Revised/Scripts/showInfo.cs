using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class showInfo : MonoBehaviour, IPointerClickHandler
{
    bool active;
    GameObject art;
    GameObject info;
    float time;
    DragItems movable;
	// Use this for initialization
	void Start ()
    {
        active = false;
        movable = GetComponent<DragItems>();
        info = this.transform.Find("Panel").gameObject;
        art = this.transform.Find("artImage").gameObject;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int clickCount = eventData.clickCount;
        if(clickCount==1)
        {
            time = eventData.clickTime;
        }
        if (eventData.clickTime - time < 1 && clickCount == 2)
        {
            time = 0;
            clickCount = 0;
            onDoubleClick();
        }

    }
    void onDoubleClick()
    {
        art.SetActive(!art.activeSelf);
        info.SetActive(!info.activeSelf);
        if(active && info.activeSelf)
        {
            movable.canBeMoved = false;
        }
        else if(active)
        {
            movable.canBeMoved = true;
        }
        
    }

}
