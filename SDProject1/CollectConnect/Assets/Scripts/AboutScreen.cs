using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AboutScreen : MonoBehaviour
{
    public Button BackBtn;

    private void Start()
    {
        BackBtn.GetComponent<Button>().onClick.AddListener(BackBtnTransition);
        BackBtn.gameObject.SetActive(true);
    }

    private void BackBtnTransition()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Update()
    {
        //quit application 
        if (Input.GetKeyDown((KeyCode.Escape)))
        {
            Application.Quit();
        }
    }
}

