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
        wordDrag connect = eventData.pointerDrag.GetComponent<wordDrag>();
        Dragable card = eventData.pointerDrag.GetComponent<Dragable>();
        if (card != null)
        {
            if ((transform.tag == "tile" && valid) || transform.tag == "hand")
            {

                card.lastLocation = transform;

            }
            else
            {
                card.lastLocation = GameObject.Find("Player1").transform;
            }
        }
        else
        {
            if ((transform.tag == "connection" && valid) || transform.tag == "wordBank")
            {
                wordDrag d = eventData.pointerDrag.GetComponent<wordDrag>();
                d.lastLocation = transform;

            }
            else
            {
                wordDrag d = eventData.pointerDrag.GetComponent<wordDrag>();
                d.lastLocation = GameObject.Find("word_bank").transform;
            }

        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (transform.tag == "hand")
        {
            ableArrange = true;
        }
        if (transform.tag == "tile"||transform.tag=="connection")
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
