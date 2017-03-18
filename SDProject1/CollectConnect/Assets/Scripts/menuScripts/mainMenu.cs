using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class mainMenu : MonoBehaviour
{
    public Button PlayBtn;
    public Button AboutBtn;

    private void Start()
    {
        PlayBtn.GetComponent<Button>().onClick.AddListener(PlayGameTransition);
        AboutBtn.GetComponent<Button>().onClick.AddListener(AboutGameTransition);
        PlayBtn.gameObject.SetActive(true);
        AboutBtn.gameObject.SetActive(true);
    }

    private void PlayGameTransition()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void AboutGameTransition()
    {
        SceneManager.LoadScene("About");
    }
}
