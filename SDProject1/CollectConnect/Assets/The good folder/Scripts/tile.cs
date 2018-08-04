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
         available = true;
 //        imageHighlight.color = glowColor;

    }
    public void notAvailable()
    {
        transform.GetChild(0).GetComponent<Dragable>().cgroup.blocksRaycasts = false;
        available = false;
        imageHighlight.color = initColor;
    }
    void Start()
    {
        imageHighlight = GetComponent<Image>();
        initColor = imageHighlight.color;

    }
}
