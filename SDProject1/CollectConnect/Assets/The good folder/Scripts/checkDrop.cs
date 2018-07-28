using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class checkDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    validMove checkTile;
    validMove updateBoard;
    initBoardplacement setTile;
    public bool ableArrange;
    bool valid, occupid;
    public void OnDrop(PointerEventData eventData)
    {

        if ((transform.tag == "tile" && valid && !occupid) || transform.tag == "hand")
        {
            Dragable d = eventData.pointerDrag.GetComponent<Dragable>();

            d.lastLocation = transform;
            checkTile.setAvailable();
        }
        else
        {
            Dragable d = eventData.pointerDrag.GetComponent<Dragable>();

            d.lastLocation = GameObject.Find("Player1").transform;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //updateBoard.validateSpaces();
        if (transform.tag == "hand")
        {
            ableArrange = true;
        }
        if (transform.tag == "tile")
        {
            checkTile = transform.GetComponent<validMove>();
            valid = checkTile.availableSpace;
            occupid = checkTile.occupid();
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
        //  setTile = GameObject.Find("mainCanvas").GetComponent<initBoardplacement>();

        // updateBoard = setTile.board[0, 0].GetComponent<validMove>();
        ableArrange = true;
        valid = false;
        //checkTile.validateSpaces();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
