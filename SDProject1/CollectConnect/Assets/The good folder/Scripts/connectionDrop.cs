using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class connectionDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    tile connectSpace;
    Board setTile;
    public bool ableArrange;
    bool valid;
    public void OnDrop(PointerEventData eventData)
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (transform.tag == "connection")
        {
            connectSpace = transform.GetComponent<tile>();
            valid = connectSpace.available;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        valid = false;
    }



    // Use this for initialization
    void Start()
    {
        valid = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
