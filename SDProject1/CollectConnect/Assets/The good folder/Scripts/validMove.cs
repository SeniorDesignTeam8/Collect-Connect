using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class validMove : MonoBehaviour
{
    Color bColor = Color.white;
    int xCord, yCord;
    Image glow;
    public GameObject board;
    GameObject neighborLoc;
    initBoardplacement tile;
    validMove neighborTile;
    public bool isAvailable;
    public void checkNearestCard()
    {
        if(xCord-1 >=0)
        {
            neighborLoc= tile.board[xCord - 1, yCord];
            neighborTile= neighborLoc.GetComponent<validMove>();
            {
                if(neighborTile.occupid())
                {
                    glow.color = bColor;
                    isAvailable = true;
                }
            }
        }
        if (xCord + 1 < tile.boardDimensions)
        {
            neighborLoc= tile.board[xCord + 1, yCord];
            neighborTile = neighborLoc.GetComponent<validMove>();
            {
                if (neighborTile.occupid())
                {
                    glow.color = bColor;
                    isAvailable = true;
                }
            }
        }
        if (yCord - 1 >= 0)
        {
            neighborLoc= tile.board[xCord, yCord-1];
            neighborTile = neighborLoc.GetComponent<validMove>();
            {
                if (neighborTile.occupid())
                {
                    glow.color = bColor;
                    isAvailable = true;
                }
            }
        }
        if (yCord + 1 < tile.boardDimensions)
        {
            neighborLoc= tile.board[xCord , yCord+1];
            neighborTile = neighborLoc.GetComponent<validMove>();
            {
                if (neighborTile.occupid())
                {
                    glow.color = bColor;
                    isAvailable = true;
                }
            }
        }
    }
    public bool occupid()
    {
        if (transform.childCount > 0)
            return true;
        else return false;
    }
    public void validateSpaces()
    {
        for (int i = 0; i < tile.boardDimensions; i++)
        {
            for(int j=0;j<tile.boardDimensions;j++)
            {
                    glow = tile.board[i, j].GetComponent<Image>();
                    xCord = i;
                    yCord = j;
                    checkNearestCard();
            }
        }

    }
    void Start()
    {
        board = GameObject.Find("mainCanvas");
        tile = board.GetComponent<initBoardplacement>();
        validateSpaces();
        
        
    }
}
