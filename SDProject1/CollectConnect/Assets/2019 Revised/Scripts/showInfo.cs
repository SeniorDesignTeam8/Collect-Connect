using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class showInfo : MonoBehaviour, IPointerClickHandler
{
    

    Image art;
    Image info;
    public Image artImg;
    public Image backImg;
    float time;
    

    public void OnPointerClick(PointerEventData eventData)
    {
        int clickCount = eventData.clickCount;
        if(clickCount==1)
        {
            time = eventData.clickTime;
        }
        if (eventData.clickTime - time < 1)
        {
            time = 0;
            clickCount = 0;
            onDoubleClick();
        }

    }
    void onDoubleClick()
    {
        GameObject.FindGameObjectWithTag("InfoPanel").GetComponent<GM>().activateCardPopup();

        GameObject panel = GameObject.FindGameObjectWithTag("InfoPanel").GetComponent<GM>().popupPanel;
        art = panel.GetComponent<refToInactiveOb>().art;

        info= panel.GetComponent<refToInactiveOb>().back;

        art.sprite = artImg.sprite;
        info.sprite = backImg.sprite;
        
    }

}
