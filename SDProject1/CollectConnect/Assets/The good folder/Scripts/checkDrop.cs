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
    bool valid;
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.tag == "tile")
        {
            checkTile = transform.GetComponent<validMove>();
            //checkTile.validateSpaces();
            valid = checkTile.isAvailable;
        }
        
        if ((transform.tag == "tile"&&valid)|| transform.tag=="hand")
        {
            Dragable d = eventData.pointerDrag.GetComponent<Dragable>();
            updateBoard.validateSpaces();
            d.lastLocation = transform;
        }
        else 
        {
            Dragable d = eventData.pointerDrag.GetComponent<Dragable>();
            updateBoard.validateSpaces();
            d.lastLocation = GameObject.Find("Hand").transform;
        }
        updateBoard.validateSpaces();
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
            valid = checkTile.isAvailable;            
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        updateBoard.validateSpaces();
        if (transform.tag=="hand")
        {
            ableArrange = false;
        }
        valid = false;
    }



    // Use this for initialization
    void Start ()
    {
        setTile = GameObject.Find("mainCanvas").GetComponent<initBoardplacement>();
        updateBoard = setTile.board[0, 0].GetComponent<validMove>();
        ableArrange = true;
        valid = false;
        //checkTile.validateSpaces();


    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
