using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}

