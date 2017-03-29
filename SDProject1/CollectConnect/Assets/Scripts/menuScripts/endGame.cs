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

    void Start()
    {
        MainMenuBtn.GetComponent<Button>().onClick.AddListener(ReturntoMain);
        MainMenuBtn.gameObject.SetActive(true);

        first.GetComponent<Text>();
        second.GetComponent<Text>();
        third.GetComponent<Text>();
        fourth.GetComponent<Text>();

        int[] placementScores = new int[4];
        placementScores[0] = GlobalVar.instance.score1;
        placementScores[1] = GlobalVar.instance.score2;
        placementScores[2] = GlobalVar.instance.score3;
        placementScores[3] = GlobalVar.instance.score4;

        Array.Sort(placementScores);
        Array.Reverse(placementScores);

        first.text = placementScores[0].ToString();
        second.text = placementScores[1].ToString();
        third.text = placementScores[2].ToString();
        fourth.text = placementScores[3].ToString();

        if (GlobalVar.instance.score1 == placementScores[0])
        {
            piece1.transform.position = new Vector3(-1, 2, 0);
        }
        else if (GlobalVar.instance.score1 == placementScores[1])
        {
            piece1.transform.position = new Vector3(-1, 0, 0);
        }
        else if (GlobalVar.instance.score1 == placementScores[2])
        {
            piece1.transform.position = new Vector3(-1, -1, 0);
        }
        else if (GlobalVar.instance.score1 == placementScores[3])
        {
            piece1.transform.position = new Vector3(-1, -3, 0);
        }
        if (GlobalVar.instance.score2 == placementScores[0])
        {
            piece2.transform.position = new Vector3(-1, 2, 0);
        }
        else if (GlobalVar.instance.score2 == placementScores[1])
        {
            piece2.transform.position = new Vector3(-1, 0, 0);
        }
        else if (GlobalVar.instance.score2 == placementScores[2])
        {
            piece2.transform.position = new Vector3(-1, -1, 0);
        }
        else if (GlobalVar.instance.score2 == placementScores[3])
        {
            piece2.transform.position = new Vector3(-1, -3, 0);
        }
        if (GlobalVar.instance.score3 == placementScores[0])
        {
            piece3.transform.position = new Vector3(-1, 2, 0);
        }
        else if (GlobalVar.instance.score3 == placementScores[1])
        {
            piece3.transform.position = new Vector3(-1, 0, 0);
        }
        else if (GlobalVar.instance.score3 == placementScores[2])
        {
            piece3.transform.position = new Vector3(-1, -1, 0);
        }
        else if (GlobalVar.instance.score3 == placementScores[3])
        {
            piece3.transform.position = new Vector3(-1, -3, 0);
        }
        if (GlobalVar.instance.score4 == placementScores[0])
        {
            piece4.transform.position = new Vector3(-1, 2, 0);
        }
        else if (GlobalVar.instance.score4 == placementScores[1])
        {
            piece4.transform.position = new Vector3(-1, 0, 0);
        }
        else if (GlobalVar.instance.score1 == placementScores[2])
        {
            piece4.transform.position = new Vector3(-1, -1, 0);
        }
        else if (GlobalVar.instance.score4 == placementScores[3])
        {
            piece4.transform.position = new Vector3(-1, -3, 0);
        }
    }

    private void ReturntoMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
