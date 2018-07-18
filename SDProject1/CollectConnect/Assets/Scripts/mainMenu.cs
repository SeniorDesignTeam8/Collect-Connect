using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenu : MonoBehaviour
{
    public Button PlayBtn;
    public Button PlayBtn2;
    public Button AboutBtn;

    private void Start()
    {
        PlayBtn.GetComponent<Button>().onClick.AddListener(PlayGameTransition);
        PlayBtn2.GetComponent<Button>().onClick.AddListener(PlayGameTransition);
        AboutBtn.GetComponent<Button>().onClick.AddListener(AboutGameTransition);
        PlayBtn.gameObject.SetActive(true);
        AboutBtn.gameObject.SetActive(true);
    }

    private static void PlayGameTransition()
    {
        SceneManager.LoadScene("PlayerSelection");
    }

    private static void AboutGameTransition()
    {
        SceneManager.LoadScene("About");
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
