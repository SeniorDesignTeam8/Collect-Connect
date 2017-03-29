using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class endGame : MonoBehaviour
{
    public Button MainMenuBtn;
    public GameObject first, second, third, fourth;
    public GameObject piece1, piece2, piece3, piece4;
    public GameObject holder1, holder2, holder3, holder4;
    public string[,] playerscores;
    public List<GameObject> PlayerPieces;

    void Start()
    {
        MainMenuBtn.GetComponent<Button>().onClick.AddListener(ReturntoMain);
        MainMenuBtn.gameObject.SetActive(true);

        playerscores = new string[4, 2];
        PlayerPieces = new List<GameObject>();

        for(int i = 0; i < 4; i++)
        {
            playerscores[i, 0] = "";
            playerscores[i, 0] = "0";
        }
        PlayerPieces.Add(piece1);
        PlayerPieces.Add(piece2);
        PlayerPieces.Add(piece3);
        PlayerPieces.Add(piece4);

        getScores();
    }

    private void getScores()
    {
        int count = 0;
        int one = 0;
        int two = 0;
        string temp = "";

        playerscores[0, 0] = "Player 1";
        playerscores[1, 0] = "Player 2";
        playerscores[2, 0] = "Player 3";
        playerscores[3, 0] = "Player 4";
        playerscores[0, 1] = GlobalVar.instance.score1.ToString();
        playerscores[1, 1] = GlobalVar.instance.score2.ToString();
        playerscores[2, 1] = GlobalVar.instance.score3.ToString();
        playerscores[3, 1] = GlobalVar.instance.score4.ToString();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (j < 3)
                {
                    Int32.TryParse(playerscores[j, 1], out one); //convert string to int
                    Int32.TryParse(playerscores[j + 1, 1], out two);

                    Debug.Log("Score 1: " + one);
                    Debug.Log("Score 2: " + two);


                    if (one < two) //if the second score is larger than the first
                    {
                        playerscores[j, 1] = two.ToString(); //flip scores
                        playerscores[j + 1, 1] = one.ToString();

                        temp = playerscores[j, 0]; //flip player names
                        playerscores[j, 0] = playerscores[j + 1, 0];
                        playerscores[j + 1, 0] = temp;
                    }
                }
            }
        }

        for (int k = 0; k < 4; k++)    //making a list of player pieces to align with scores
        {
            if (playerscores[k, 0] == "Player 1")
            {
                PlayerPieces[k] = piece1;
            }
            else if (playerscores[k, 0] == "Player 2")
            {
                PlayerPieces[k] = piece2;
            }
            else if (playerscores[k, 0] == "Player 3")
            {
                PlayerPieces[k] = piece3;
            }
            else if (playerscores[k, 0] == "Player 4")
            {
                PlayerPieces[k] = piece4;
            }
        }

        PlayerPieces[0].transform.position = holder1.transform.position;
        first.gameObject.GetComponent<Text>().text = playerscores[0, 1];
        holder1.gameObject.GetComponent<Renderer>().enabled = false;

        PlayerPieces[1].transform.position = holder2.transform.position;
        second.gameObject.GetComponent<Text>().text = playerscores[1, 1];
        holder2.gameObject.GetComponent<Renderer>().enabled = false;

        PlayerPieces[2].transform.position = holder3.transform.position;
        third.gameObject.GetComponent<Text>().text = playerscores[2, 1];
        holder3.gameObject.GetComponent<Renderer>().enabled = false;

        PlayerPieces[3].transform.position = holder4.transform.position;
        fourth.gameObject.GetComponent<Text>().text = playerscores[3, 1];
        holder4.gameObject.GetComponent<Renderer>().enabled = false;
    }

    private void ReturntoMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
