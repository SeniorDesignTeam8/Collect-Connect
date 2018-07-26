using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class validMove : MonoBehaviour
{
    int xCord, yCord;
    GameObject board;
    GameObject neighbor;
    initBoardplacement tile;
    public void checkNearestCard()
    {
        if(xCord-1 >=0)
        {
            if(yCord+1 !=tile.boardDimensions)
            {

            }
            if(yCord-1>=0)
            {

            }
        }
    }
    public bool occupid()
    {
        if (transform.childCount > 0)
            return true;
        else return false;
    }
    void Start()
    {
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
    }
}
