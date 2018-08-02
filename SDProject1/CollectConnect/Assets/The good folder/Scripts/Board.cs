using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{

    public GameObject panel;
    public GameObject card;
    GameObject start;
    public float offsetX;
    public float offsetY;
    public int boardDimensions;
    public float distancePanelX;
    public float distancePanelY;
    public GameObject mainCanvas;
  

    public GameObject[,] board;


    // Use this for initialization
    void Start()
    {
        int middle = (boardDimensions - 1) / 2;
        board = new GameObject[boardDimensions, boardDimensions];
        for (int i = 0; i < boardDimensions; i++)
        {
            for (int j = 0; j < boardDimensions; j++)
            {

                board[i, j] = Instantiate(panel, new Vector3(i * distancePanelX + offsetX, j * distancePanelY + offsetY, 0), Quaternion.identity);
                board[i, j].transform.SetParent(mainCanvas.transform);
                if (i == middle && j == middle)
                {
                    start = Instantiate(card);
                    start.transform.SetParent(board[i, j].transform);
                }
            }
        }
        Invoke("resetBoard", 1);
    }
    public void resetBoard()
    {
        
        for (int i = 0; i < boardDimensions; i++)
        {
            for (int j = 0; j < boardDimensions; j++)
            {
                if (board[i, j].transform.childCount > 0)
                {
                    board[i, j].GetComponent<tile>().isAvailable();
                }
                else
                {
                    if (checkNeighbor(i, j))
                    {
                        board[i, j].GetComponent<tile>().isAvailable();
                    }
                    else
                    {
                        board[i, j].GetComponent<tile>().notAvailable();
                    }
                }

            }
        }
        

    }
    public bool checkNeighbor(int xCord, int yCord)
    {
        if (xCord - 1 >= 0)
        {
            if (board[xCord - 1, yCord].transform.childCount > 0)
                return true;
        }

        if (xCord + 1 < boardDimensions)
        {
            if (board[xCord + 1, yCord].transform.childCount > 0)
                return true;
        }
        if (yCord - 1 >= 0)
        {
            if (board[xCord, yCord - 1].transform.childCount > 0)
                return true;
        }
        if (yCord + 1 < boardDimensions)
        {
            if (board[xCord, yCord + 1].transform.childCount > 0)
                return true;
        }
        return false;

    }

}