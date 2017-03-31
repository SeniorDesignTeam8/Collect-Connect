using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EndGame2 : MonoBehaviour
{
    public Button MainMenuBtn;
    public GameObject P1Piece;
    public GameObject P2Piece;
    public GameObject P3Piece;
    public GameObject P4Piece;
    public GameObject FirstPlaceHolder;
    public GameObject SecondPlaceHolder;
    public GameObject ThirdPlaceHolder;
    public GameObject FourthPlaceHolder;
    public string[,] PlayerScores;
    public List<GameObject> PlayerPieces;
    public GameObject FirstPlaceScoreTxt;
    public GameObject SecondPlaceScoreTxt;
    public GameObject ThirdPlaceScoreTxt;
    public GameObject FourthPlaceScoreTxt;

    private void Start()
    {
        MainMenuBtn.GetComponent<Button>().onClick.AddListener(MainMenuTransition);
        MainMenuBtn.gameObject.SetActive(true);

        PlayerScores = new string[4, 2];
        PlayerPieces = new List<GameObject>();

        for (int i = 0; i < 4; i++) //prefill
        {
            PlayerScores[i, 0] = "";   //prefill blanks for player
            PlayerScores[i, 1] = "0";  //prefill 0 for scores

            PlayerPieces.Add(P1Piece); //prefill player pieces
        }

        Scoring();  //calculate scoring
    }

    private void MainMenuTransition()    //go back to main menu
    {
        SceneManager.LoadScene("mainMenu");
    }

    private void Scoring()
    {
        int score1 = 0;
        int score2 = 0;
        string tempName = "";

        //get players and scores
        PlayerScores[0, 0] = "PLAYER1";
        PlayerScores[1, 0] = "PLAYER2";
        PlayerScores[2, 0] = "PLAYER3";
        PlayerScores[3, 0] = "PLAYER4";
        PlayerScores[0, 1] = PlayerPrefs.GetInt("Player1Score").ToString();
        PlayerScores[1, 1] = PlayerPrefs.GetInt("Player2Score").ToString();
        PlayerScores[2, 1] = PlayerPrefs.GetInt("Player3Score").ToString();
        PlayerScores[3, 1] = PlayerPrefs.GetInt("Player4Score").ToString();


        for (int b = 0; b < 4; b++)
        {
            Debug.Log(PlayerScores[b, 0]);
            Debug.Log(PlayerScores[b, 1]);
        }


        for (int i = 0; i < 4; i++) //find top score
        {
            for (int j = 0; j < 4; j++)
            {
                if (j < 3)  //not end of list
                {
                    Int32.TryParse(PlayerScores[j, 1], out score1); //convert string to int
                    Int32.TryParse(PlayerScores[j + 1, 1], out score2);

                    Debug.Log("Score 1: " + score1);
                    Debug.Log("Score2: " + score2);


                    if (score1 < score2) //if the second score is larger than the first
                    {
                        PlayerScores[j, 1] = score2.ToString(); //flip scores
                        PlayerScores[j + 1, 1] = score1.ToString();

                        tempName = PlayerScores[j, 0]; //flip player names
                        PlayerScores[j, 0] = PlayerScores[j + 1, 0];
                        PlayerScores[j + 1, 0] = tempName;
                    }
                }
            }
        }

        for (int k = 0; k < 4; k++)    //making a list of player pieces to align with scores
        {
            if (PlayerScores[k, 0] == "PLAYER1")
            {
                PlayerPieces[k] = P1Piece;
            }
            else if (PlayerScores[k, 0] == "PLAYER2")
            {
                PlayerPieces[k] = P2Piece;
            }
            else if (PlayerScores[k, 0] == "PLAYER3")
            {
                PlayerPieces[k] = P3Piece;
            }
            else if (PlayerScores[k, 0] == "PLAYER4")
            {
                PlayerPieces[k] = P4Piece;
            }
        }

        //card.gameObject.transform.position = ExpCardImage.transform.position;
        //card.gameObject.transform.localScale = ExpCardImage.gameObject.GetComponent<Renderer>().bounds.extents;

        //set leaderboard
        PlayerPieces[0].transform.position = FirstPlaceHolder.transform.position;
        FirstPlaceScoreTxt.gameObject.GetComponent<Text>().text = PlayerScores[0, 1];
        FirstPlaceHolder.gameObject.GetComponent<Renderer>().enabled = false;

        PlayerPieces[1].transform.position = SecondPlaceHolder.transform.position;
        SecondPlaceScoreTxt.gameObject.GetComponent<Text>().text = PlayerScores[1, 1];
        SecondPlaceHolder.gameObject.GetComponent<Renderer>().enabled = false;

        PlayerPieces[2].transform.position = ThirdPlaceHolder.transform.position;
        ThirdPlaceScoreTxt.gameObject.GetComponent<Text>().text = PlayerScores[2, 1];
        ThirdPlaceHolder.gameObject.GetComponent<Renderer>().enabled = false;

        PlayerPieces[3].transform.position = FourthPlaceHolder.transform.position;
        FourthPlaceScoreTxt.gameObject.GetComponent<Text>().text = PlayerScores[3, 1];
        FourthPlaceHolder.gameObject.GetComponent<Renderer>().enabled = false;
    }
}
 