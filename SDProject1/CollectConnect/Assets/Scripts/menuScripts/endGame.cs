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
    public Text first, second, third, fourth;
    public GameObject piece1, piece2, piece3, piece4;
    private BoardManager bM;
    private List<Player> Scores;

    void Start()
    {
        bM = FindObjectOfType<BoardManager>();
        Scores = bM._playerScriptRefs;
        MainMenuBtn.GetComponent<Button>().onClick.AddListener(ReturntoMain);
        MainMenuBtn.gameObject.SetActive(true);

        first.GetComponent<Text>();
        second.GetComponent<Text>();
        third.GetComponent<Text>();
        fourth.GetComponent<Text>();

        int[] placementScores = new int[4];
        placementScores[0] = Scores[0].Score;
        placementScores[1] = Scores[1].Score;
        placementScores[2] = Scores[2].Score;
        placementScores[3] = Scores[3].Score;

        Array.Sort(placementScores);

        first.text = placementScores[0].ToString();
        second.text = placementScores[1].ToString();
        third.text = placementScores[2].ToString();
        fourth.text = placementScores[3].ToString();

        foreach (Player p in Scores)
        {
            int j = 1;
            for (int i = 0; i < 4; i++)
            {
                if (p.Score == placementScores[i] && i == 0 && j == 1)
                {
                    piece1.transform.position = new Vector3(-1, 2, 0);
                }
                else if (p.Score == placementScores[i] && i == 1 && j == 1)
                {
                    piece1.transform.position = new Vector3(-1, 0, 0);
                }
                else if (p.Score == placementScores[i] && i == 2 && j == 1)
                {
                    piece1.transform.position = new Vector3(-1, -1, 0);
                }
                else if (p.Score == placementScores[i] && i == 3 && j == 1)
                {
                    piece1.transform.position = new Vector3(-1, -3, 0);
                }
                else if (p.Score == placementScores[i] && i == 0 && j == 2)
                {
                    piece2.transform.position = new Vector3(-1, 2, 0);
                }
                else if (p.Score == placementScores[i] && i == 1 && j == 2)
                {
                    piece2.transform.position = new Vector3(-1, 0, 0);
                }
                else if (p.Score == placementScores[i] && i == 2 && j == 2)
                {
                    piece2.transform.position = new Vector3(-1, -1, 0);
                }
                else if (p.Score == placementScores[i] && i == 3 && j == 2)
                {
                    piece2.transform.position = new Vector3(-1, -3, 0);
                }
                else if (p.Score == placementScores[i] && i == 0 && j == 3)
                {
                    piece3.transform.position = new Vector3(-1, 2, 0);
                }
                else if (p.Score == placementScores[i] && i == 1 && j == 3)
                {
                    piece3.transform.position = new Vector3(-1, 0, 0);
                }
                else if (p.Score == placementScores[i] && i == 2 && j == 3)
                {
                    piece3.transform.position = new Vector3(-1, -1, 0);
                }
                else if (p.Score == placementScores[i] && i == 3 && j == 3)
                {
                    piece3.transform.position = new Vector3(-1, -3, 0);
                }
                else if (p.Score == placementScores[i] && i == 0 && j == 4)
                {
                    piece4.transform.position = new Vector3(-1, 2, 0);
                }
                else if (p.Score == placementScores[i] && i == 1 && j == 4)
                {
                    piece4.transform.position = new Vector3(-1, 0, 0);
                }
                else if (p.Score == placementScores[i] && i == 2 && j == 4)
                {
                    piece4.transform.position = new Vector3(-1, -1, 0);
                }
                else if (p.Score == placementScores[i] && i == 3 && j == 4)
                {
                    piece4.transform.position = new Vector3(-1, -3, 0);
                }
            }
            j++;
        }
    }

    private void ReturntoMain()
    {
        SceneManager.LoadScene("mainMenu");
    }
}
