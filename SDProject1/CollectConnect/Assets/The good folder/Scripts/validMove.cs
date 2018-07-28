using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class validMove : MonoBehaviour
{
    public bool isOccupid, availableSpace;
    Color bColor = Color.white, initColor;

    Image glow;
    public GameObject board;
    GameObject neighborLoc;
    initBoardplacement tile;
    validMove neighborTile;

    public void setNeighbor(int xCord, int yCord)
    {


        if (xCord - 1 >= 0)
        {
            neighborLoc = tile.board[xCord - 1, yCord];
            neighborTile = neighborLoc.GetComponent<validMove>();
            setColoravailable();
        }

        if (xCord + 1 < tile.boardDimensions)
        {
            neighborLoc = tile.board[xCord + 1, yCord];
            neighborTile = neighborLoc.GetComponent<validMove>();
            setColoravailable();
        }
        if (yCord - 1 >= 0)
        {
            neighborLoc = tile.board[xCord, yCord - 1];
            neighborTile = neighborLoc.GetComponent<validMove>();
            setColoravailable();
        }
        if (yCord + 1 < tile.boardDimensions)
        {
            neighborLoc = tile.board[xCord, yCord + 1];
            neighborTile = neighborLoc.GetComponent<validMove>();
            setColoravailable();
        }

    }
    void setColoravailable()
    {
        if (!neighborTile.occupid())
        {
            neighborTile.glow.color = bColor;
            neighborTile.availableSpace = true;
        }
        else
        {
            neighborTile.glow.color = initColor;
            neighborTile.availableSpace = false;
        }

    }
    public bool occupid()
    {
        if (transform.childCount > 0)
        {
            isOccupid = true;
            glow.color = initColor;
            return true;
        }

        else
        {
            isOccupid = false;
            return false;
        }
    }

    public void setAvailable()
    {

        for (int i = 0; i < tile.boardDimensions; i++)
        {
            for (int j = 0; j < tile.boardDimensions; j++)
            {
                validMove x = tile.board[i, j].GetComponent<validMove>();
                if (x.occupid())
                {
                    setNeighbor(i, j);
                }
            }
        }

    }
    void Start()
    {
        board = GameObject.Find("mainCanvas");
        tile = board.GetComponent<initBoardplacement>();
        glow = GetComponent<Image>();
        initColor = glow.color;

    }
}
