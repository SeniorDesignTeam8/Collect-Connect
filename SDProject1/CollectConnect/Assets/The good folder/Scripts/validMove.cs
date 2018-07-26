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
    public bool checkNearestCard()
    {
        if(xCord-1 >=0)
        {
            neighborLoc= tile.board[xCord - 1, yCord];
            neighborTile= neighborLoc.GetComponent<validMove>();
            {
                if(neighborTile.occupid())
                {
                    glow.color = bColor;
                    return true;
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
                    return true;
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
                    return true;
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
                    return true;
                }
            }
        }
        return false;
    }
    public bool occupid()
    {
        if (transform.childCount > 0)
            return true;
        else return false;
    }
    void Start()
    {
        board = GameObject.Find("mainCanvas");
        tile = board.GetComponent<initBoardplacement>();
        for (int i = 0; i < tile.boardDimensions; i++)
        {
            for(int j=0;j<tile.boardDimensions;j++)
            {
                if(transform==tile.board[i,j].transform)
                {
                    xCord = i;
                    yCord = j;
                    break;
                }
            }
        }
        glow = GetComponent<Image>();
    }
}
