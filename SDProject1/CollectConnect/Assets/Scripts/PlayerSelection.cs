using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
    public Button BackBtn;
    public Button OnePlayerBtn;
    public Button TwoPlayersBtn;
    public Button ThreePlayersBtn;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

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

    private static void BackBtnTransition()
    {
        SceneManager.LoadScene("About");
    }

    private static void OnePlayerFunction()
    {
        PlayerPrefs.SetInt("PlayerNumber", 1);
        SceneManager.LoadScene("MainScene");
    }

    private static void TwoPlayersFunction()
    {
        PlayerPrefs.SetInt("PlayerNumber", 2);
        SceneManager.LoadScene("MainScene");
    }

    private static void ThreePlayersFunction()
    {
        PlayerPrefs.SetInt("PlayerNumber", 3);
        SceneManager.LoadScene("MainScene");
    }
}


