using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerSelection : MonoBehaviour
{
    public Button BackBtn;
    public Button OnePlayerBtn;
    public Button TwoPlayersBtn;
    public Button ThreePlayersBtn;

    private void Start()
    {
        BackBtn.GetComponent<Button>().onClick.AddListener(BackBtnTransition);
        BackBtn.gameObject.SetActive(true);

        OnePlayerBtn.GetComponent<Button>().onClick.AddListener(OnePlayerFunction);
        OnePlayerBtn.gameObject.SetActive(true);

        TwoPlayersBtn.GetComponent<Button>().onClick.AddListener(TwoPlayersFunction);
        TwoPlayersBtn.gameObject.SetActive(true);

        ThreePlayersBtn.GetComponent<Button>().onClick.AddListener(ThreePlayersFunction);
        ThreePlayersBtn.gameObject.SetActive(true);
    }

    private void BackBtnTransition()
    {
        SceneManager.LoadScene("About");
    }

    //using Globalvar for out Playernumbers as well
    private void OnePlayerFunction()
    {
        GlobalVar.instance.PlayerNumber = 1;
        SceneManager.LoadScene("MainScene");
    }

    private void TwoPlayersFunction()
    {
        GlobalVar.instance.PlayerNumber = 2;
        SceneManager.LoadScene("MainScene");
    }

    private void ThreePlayersFunction()
    {
        GlobalVar.instance.PlayerNumber = 3;
        SceneManager.LoadScene("MainScene");
    }
}


