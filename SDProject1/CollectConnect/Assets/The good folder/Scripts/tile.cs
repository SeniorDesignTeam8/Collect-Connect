using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class tile : MonoBehaviour
{
    public bool confirmed, available;
    Color glowColor = Color.white, initColor;

    Image imageHighlight;


    public void isAvailable()
    {
         available = true;
         imageHighlight.color = glowColor;

    }
    public void notAvailable()
    {
        Dragable card = transform.GetChild(0).GetComponent<Dragable>();
        if (card != null)
        {
            card.canBeMoved = false;
        }
        else
        {
            wordDrag connect = transform.GetChild(0).GetComponent<wordDrag>();
            connect.cgroup.blocksRaycasts = false;
        }
        available = false;
        imageHighlight.color = initColor;
    }
    void Start()
    {
        confirmed = false;  
        imageHighlight = GetComponent<Image>();
        initColor = imageHighlight.color;

    }
}
