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

    public void PlayGameTransition()
    {
        SceneManager.LoadScene("Game");
    }

    public void AboutGameTransition()
    {
        SceneManager.LoadScene("About");
    }
}
