using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class checkDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    tile boardSpace;
    Board setTile;
    public bool ableArrange;
    bool valid;
    public void OnDrop(PointerEventData eventData)
    {

        if ((transform.tag == "tile" && valid) || transform.tag == "hand")
        {
            Dragable d = eventData.pointerDrag.GetComponent<Dragable>();
            d.lastLocation = transform;

        }
        else
        {
            Dragable d = eventData.pointerDrag.GetComponent<Dragable>();
            d.lastLocation = GameObject.Find("Player1").transform;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (transform.tag == "hand")
        {
            ableArrange = true;
        }
        if (transform.tag == "tile")
        {
            boardSpace = transform.GetComponent<tile>();
            valid = boardSpace.available;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {

        if (transform.tag == "hand")
        {
            ableArrange = false;
        }
        valid = false;
    }



    // Use this for initialization
    void Start()
    {

        ableArrange = true;
        valid = false;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
