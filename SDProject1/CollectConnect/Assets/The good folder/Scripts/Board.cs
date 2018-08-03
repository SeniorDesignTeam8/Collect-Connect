using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{

    public GameObject panel;
    public GameObject wordPanel;
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
        setUpBoard();
        Invoke("pickStartCard", 1);
        Invoke("resetBoard", 1);
    }

    public void setUpBoard()
    {
        RectTransform rtCanvas = mainCanvas.GetComponent<RectTransform>();
        Vector2 sizing = rtCanvas.sizeDelta;
        offsetX = sizing.x * .35f;
        offsetY = sizing.y * .3f;


        board = new GameObject[boardDimensions, boardDimensions];

        for (int i = 0; i < boardDimensions; i++)
        {
            for (int j = 0; j < boardDimensions; j++)
            {
                board[i, j] = Instantiate(panel, new Vector3(i * distancePanelX + offsetX, j * distancePanelY + offsetY, 0), Quaternion.identity);
                board[i, j].transform.SetParent(mainCanvas.transform);

            }
        }

    }
    //public void setUpBoard()
    //{
    //    RectTransform rtCanvas = mainCanvas.GetComponent<RectTransform>();
    //    Vector2 sizing = rtCanvas.sizeDelta;
    //    offsetX = sizing.x * .35f;
    //    offsetY = sizing.y * .3f;

    //    int size = (boardDimensions * 2) - 1;
    //    board = new GameObject[size, size];

    //    for (int i = 0; i < size; i++)
    //    {
    //        for (int j = 0; j < size; j++)
    //        {
    //            if (i % 2 == 0 && j % 2 == 0)
    //            {
    //                board[i, j] = Instantiate(panel, new Vector3(i * distancePanelX + offsetX, j * distancePanelY + offsetY, 0), Quaternion.identity);
    //                board[i, j].transform.SetParent(mainCanvas.transform);
    //            }
    //            else if ((i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0))
    //            {
    //                board[i, j] = Instantiate(wordPanel, new Vector3(i * distancePanelX + offsetX, j * distancePanelY + offsetY, 0), Quaternion.identity);
    //                board[i, j].transform.SetParent(mainCanvas.transform);
    //            }

    //            else board[i, j] = null;

    //        }
    //    }

    //}
    public void pickStartCard()
    {
        int middle = (boardDimensions - 1)/2;
        // int middle = (boardDimensions - 1);
        CardManager startingCard = GameObject.Find("mainCanvas").GetComponent<CardManager>();
        start = startingCard.createCardObject();
        start.transform.SetParent(board[middle, middle].transform);
    }
    public void resetBoard()
    {
        
        for (int i = 0; i < boardDimensions; i++)
        {
            for (int j = 0; j < boardDimensions; j++)
            {
                if (board[i, j] != null)
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
        

    }
    public bool checkNeighbor(int xCord, int yCord)
    {
            if (xCord - 1 >= 0)
            {
                if (board[xCord - 1, yCord] != null)
                {
                    if (board[xCord - 1, yCord].transform.childCount > 0)
                        return true;
                }
            }

            if (xCord + 1 < boardDimensions)
            {
                if (board[xCord + 1, yCord] != null)
                {
                    if (board[xCord + 1, yCord].transform.childCount > 0)
                        return true;
                }
            }
            if (yCord - 1 >= 0)
            {
                if (board[xCord, yCord - 1] != null)
                {
                    if (board[xCord, yCord - 1].transform.childCount > 0)
                        return true;
                }
            }
            if (yCord + 1 < boardDimensions)
            {
                if (board[xCord, yCord + 1] != null)
                {
                    if (board[xCord, yCord + 1].transform.childCount > 0)
                        return true;
                }
            }
            return false;

    }

}