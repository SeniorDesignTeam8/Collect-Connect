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
    int size;

    public GameObject[,] board;


    // Use this for initialization
    void Start()
    {
        setUpBoard();
        Invoke("pickStartCard", 1);
        Invoke("checkCardsOnBoard", 1);
    }


    public void setUpBoard()
    {
        RectTransform rtCanvas = mainCanvas.GetComponent<RectTransform>();
        Vector2 sizing = rtCanvas.sizeDelta;
        offsetX = sizing.x * .35f;
        offsetY = sizing.y * .3f;

        size = (boardDimensions * 2) - 1;
        board = new GameObject[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i % 2 == 0 && j % 2 == 0)
                {
                    board[i, j] = Instantiate(panel, new Vector3(i * distancePanelX + offsetX, j * distancePanelY + offsetY, 0), Quaternion.identity);
                    board[i, j].transform.SetParent(mainCanvas.transform);
                }
                else if ((i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0))
                {
                    board[i, j] = Instantiate(wordPanel, new Vector3(i * distancePanelX + offsetX, j * distancePanelY + offsetY-20, 0), Quaternion.identity);
                    board[i, j].transform.SetParent(mainCanvas.transform);
                }

                else board[i, j] = null;

            }
        }

    }
    public void pickStartCard()
    {
         int middle = (boardDimensions - 1);
        CardManager startingCard = GameObject.Find("mainCanvas").GetComponent<CardManager>();
        start = startingCard.createCardObject();
        start.transform.SetParent(board[middle, middle].transform);
    }
    public void checkCardsOnBoard()
    {
        
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (board[i, j] != null&& board[i, j].transform.childCount > 0)
                {
                   board[i, j].GetComponent<tile>().notAvailable();
                   checkNeighbor(i, j);
 
                }

            }
        }
        

    }
    public void checkNeighbor(int xCord, int yCord)
    {
        if (xCord - 2 >= 0)
        {
            if (board[xCord - 2, yCord] != null)
            {
                if (board[xCord - 2, yCord].transform.childCount > 0)
                {
                    board[xCord - 2, yCord].GetComponent<tile>().notAvailable();
                }
                else
                {
                    board[xCord - 2, yCord].GetComponent<tile>().isAvailable();
                }
            }
        }
     
           if (xCord + 2 < size)
            {
                 if (board[xCord + 2, yCord] != null)
                  {
                      if (board[xCord + 2, yCord].transform.childCount > 0)
                      {
                         board[xCord + 2, yCord].GetComponent<tile>().notAvailable();
                      }
                        else
                     {
                          board[xCord + 2, yCord].GetComponent<tile>().isAvailable();
                      }
                 }
            }
            if (yCord - 2 >= 0)
            {
                if (board[xCord, yCord - 2] != null)
                {
                     if (board[xCord, yCord - 2].transform.childCount > 0)
                      {
                           board[xCord, yCord - 2].GetComponent<tile>().notAvailable();
                      }
                     else
                     {
                           board[xCord, yCord - 2].GetComponent<tile>().isAvailable();
                        }

                 }
            }

            if (yCord + 2 < size)
            {
                if (board[xCord, yCord + 2] != null)
                {
                       if (board[xCord, yCord + 2].transform.childCount > 0)
                      {
                          board[xCord, yCord + 2].GetComponent<tile>().notAvailable();
                     }
                      else
                     {
                          board[xCord, yCord + 2].GetComponent<tile>().isAvailable();
                     }
                 
                }
            }
    }

    public void validConnection()
    {


    }

}