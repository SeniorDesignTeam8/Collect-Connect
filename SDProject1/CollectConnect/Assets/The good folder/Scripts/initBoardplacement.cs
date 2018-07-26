using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class initBoardplacement : MonoBehaviour {

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
    void Start ()
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

        
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
