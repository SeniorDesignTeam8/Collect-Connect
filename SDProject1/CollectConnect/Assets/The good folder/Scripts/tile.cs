using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class tile : MonoBehaviour
{
   
    public bool isOccupid, available;
    Color glowColor = Color.white, initColor;
 
    Image imageHighlight;


    public void isAvailable()
    {
        if (transform.childCount == 0)
        {
            available = true;
            imageHighlight.color = glowColor;
        }
        else
        {
            available = false;

            imageHighlight.color = initColor;
        }
    }
    public void notAvailable()
    {
        available = false;
        imageHighlight.color = initColor;
    }
    void Start()
    {
       // canBeMoved = true;
        imageHighlight = GetComponent<Image>();
        initColor = imageHighlight.color;

    }
}
