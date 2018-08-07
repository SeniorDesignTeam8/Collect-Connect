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
    bool begin = false;
    public GameObject[,] board;
   
    List<GameObject> available;

  
    void Start()
    {
        available = new List<GameObject>();
        setUpBoard();
        Invoke("pickStartCard", 1);
        Invoke("checkCardsOnBoard", 1);
    }

    //initalizes board with spaces for cards and word connections
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
    //sets the starting card in the middle of the board
    public void pickStartCard()
    {
         int middle = (boardDimensions - 1);
        CardManager startingCard = GameObject.Find("mainCanvas").GetComponent<CardManager>();
        start = startingCard.createCardObject();
        start.transform.SetParent(board[middle, middle].transform);
    }

    //checks the board for cards
    //once a card is found it highlights the neihboring spots
    //indicating to the player that that is an available spot to play a card
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
                    validConnection(i, j);

                }

            }
        }
        

    }

    //returns a card or word if the player tries to place more than one
    public void limitActiveObjects()
    {
        int cards = 0, connections = 0, cardref=-1, connRef=-1;
        
        for(int i=0; i< available.Count;i++)
        {
            if(available[i].transform.childCount>0 && available[i].tag=="tile")
            {
                if (cards == 0)
                {
                    cards++;
                    cardref = i;
                }
                else
                {
                    Transform t = available[cardref].GetComponentInChildren<Dragable>().hand;
                  //  GameObject wtf = available[cardref].transform.GetChild(0).gameObject;
                    available[cardref].transform.GetChild(0).SetParent(t);
                    cardref = i;
                }
            }
            else if (available[i].transform.childCount > 0 && available[i].tag == "connection")
            {
                if (connections == 0)
                {
                    connections++;
                    connRef = i;
                }
                else
                {
                    Transform t = available[connRef].GetComponentInChildren<wordDrag>().bank;
                    available[connRef].transform.GetChild(0).transform.SetParent(t);
                    connRef = i;
                }
            }
        }
    }

    //arranges the list "available" so that the most recently placed object 
    //is at the back of the list. important for determining which card to 
    //send back to the players hand when they try to place more than one 
    //on the board at a time
    public void addToListEnd(Transform recent)
    {
        // available.Add(recent.parent.gameObject);
        if (available.Contains(recent.parent.gameObject))
        {
            int current = available.IndexOf(recent.parent.gameObject);
            available.Add(available[current]);
            available.RemoveAt(current);
        }
        Invoke("limitActiveObjects", .1f);
    }

    //the function that checks whether the tiles neighboring where a 
    //card has been placed are available to hold a card or are already full
    //and cannot hold a card
    public void checkNeighbor(int xCord, int yCord)
    {
        if (xCord - 2 >= 0)
        {
            if (board[xCord - 2, yCord] != null && board[xCord-2,yCord].tag=="tile")
            {
                if (board[xCord - 2, yCord].transform.childCount > 0)
                {
                    available.Remove(board[xCord - 2, yCord]);
                    board[xCord - 2, yCord].GetComponent<tile>().notAvailable();
                }
                else
                {
                    if(!available.Contains(board[xCord - 2, yCord]))
                         available.Add(board[xCord - 2, yCord]);
                    board[xCord - 2, yCord].GetComponent<tile>().isAvailable();
                }
            }
        }
     
           if (xCord + 2 < size)
            {
                 if (board[xCord + 2, yCord] != null && board[xCord + 2, yCord].tag == "tile")
            {
                      if (board[xCord + 2, yCord].transform.childCount > 0)
                      {
                         available.Remove(board[xCord + 2, yCord]);
                         board[xCord + 2, yCord].GetComponent<tile>().notAvailable();
                      }
                        else
                      {
                        if (!available.Contains(board[xCord +2, yCord]))
                            available.Add(board[xCord +2, yCord]);   
                          board[xCord + 2, yCord].GetComponent<tile>().isAvailable();
                      }
                 }
            }
            if (yCord - 2 >= 0)
            {
                if (board[xCord, yCord - 2] != null && board[xCord , yCord- 2].tag == "tile")
            {
                     if (board[xCord, yCord - 2].transform.childCount > 0)
                      {
                           available.Remove(board[xCord , yCord- 2]);
                           board[xCord, yCord - 2].GetComponent<tile>().notAvailable();
                      }
                     else
                     {
                           if(!available.Contains(board[xCord, yCord - 2]))
                                 available.Add(board[xCord , yCord-2]);
                           board[xCord, yCord - 2].GetComponent<tile>().isAvailable();
                     }

                 }
            }

            if (yCord + 2 < size)
            {
                if (board[xCord, yCord + 2] != null && board[xCord, yCord + 2].tag == "tile")
            {
                     if (board[xCord, yCord + 2].transform.childCount > 0)
                     {
                          available.Remove(board[xCord , yCord +2]);
                          board[xCord, yCord + 2].GetComponent<tile>().notAvailable();
                     }
                     else
                     {
                        if(!available.Contains(board[xCord , yCord+ 2]))
                              available.Add(board[xCord , yCord+2]);
                        board[xCord, yCord + 2].GetComponent<tile>().isAvailable();
                     }
                 
                }
            }
    }

    //highlights the connections available for a player to make
    public void validConnection(int xCord, int yCord)
    {
        if (xCord - 1 >= 0)
        {
            if (board[xCord - 1, yCord] != null)
            {
                if (board[xCord - 1, yCord].transform.childCount > 0)
                {
                    available.Remove(board[xCord - 1, yCord]);
                    board[xCord - 1, yCord].GetComponent<tile>().notAvailable();
                }
                else
                {
                    if (!available.Contains(board[xCord - 1, yCord]))
                        available.Add(board[xCord - 1, yCord]);
                    board[xCord - 1, yCord].GetComponent<tile>().isAvailable();
                }
            }
        }

        if (xCord + 1 < size)
        {
            if (board[xCord + 1, yCord] != null)
            {
                if (board[xCord + 1, yCord].transform.childCount > 0)
                {
                    available.Remove(board[xCord + 1, yCord]);
                    board[xCord + 1, yCord].GetComponent<tile>().notAvailable();
                }
                else
                {
                    if (!available.Contains(board[xCord +1, yCord]))
                        available.Add(board[xCord + 1, yCord]);
                    board[xCord + 1, yCord].GetComponent<tile>().isAvailable();
                }
            }
        }
        if (yCord - 1 >= 0)
        {
            if (board[xCord, yCord - 1] != null)
            {
                if (board[xCord, yCord - 1].transform.childCount > 0)
                {
                    available.Remove(board[xCord , yCord- 1]);
                    board[xCord, yCord - 1].GetComponent<tile>().notAvailable();
                }
                else
                {
                    if (!available.Contains(board[xCord , yCord-1]))
                        available.Add(board[xCord , yCord- 1]);
                    board[xCord, yCord - 1].GetComponent<tile>().isAvailable();
                }

            }
        }

        if (yCord + 1 < size)
        {
            if (board[xCord, yCord + 1] != null)
            {
                if (board[xCord, yCord + 1].transform.childCount > 0)
                {
                    available.Remove(board[xCord, yCord + 1]);
                    board[xCord, yCord + 1].GetComponent<tile>().notAvailable();
                }
                else
                {
                    if (!available.Contains(board[xCord, yCord+1]))
                        available.Add(board[xCord, yCord + 1]);
                    board[xCord, yCord + 1].GetComponent<tile>().isAvailable();
                }

            }
        }
    }


    //checks where the player played a card and a connection
    //if they are not next to each other then the connection 
    //is not validated and their turn is not up
    public void isValidMove()
    {

        int card = -1, connect = -1, cardi=-1, cardj=-1, connj=-1, conni=-1;
        for (int i = 0; i < available.Count; i++)
        {
            if (available[i].transform.childCount > 0 && available[i].tag == "tile")
            {
                card = i;
            }
            else if (available[i].transform.childCount > 0 && available[i].tag == "connection")
            {
                connect = i;
            }
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (card!=-1&& board[i, j] == available[card])
                {
                    cardi = i;
                    cardj = j;
                }
                if (connect!=-1&&board[i, j] == available[connect])
                {
                    connj = j;
                    conni = i;
                }
            }
        }
        if (cardi!=-1&&cardj!=-1&&conni!=-1&&connj!=-1)
        { if (cardi == conni && (cardj + 1 == connj || cardj - 1 == connj))
            {
                checkCardsOnBoard();
            }
            else if (cardj == connj && (cardi + 1 == conni || cardi - 1 == conni))
            {
                checkCardsOnBoard();
            }
            else
            {
                Debug.Log(cardi + "   " + conni);
                Debug.Log(cardj + "   " + connj);
                Debug.Log("Invalid Move!");
            }
        }
    }
}