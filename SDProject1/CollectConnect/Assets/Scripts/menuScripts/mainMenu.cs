using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour {

    public Texture BackgroundTexture;
    public Texture TitleText;
    public GUIStyle btn;
    public AudioClip ButtonClickSound;
    public AudioSource MenuSoundSource;

    private void Start()
    {
        MenuSoundSource.clip = ButtonClickSound;
    }

    private void OnGUI()
    {
        //display background texture
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BackgroundTexture);

        GUI.DrawTexture(new Rect(Screen.width / 4, Screen.height - Screen.height, Screen.width * 0.5f, Screen.height * 0.5f), TitleText);

        //display buttons (without GUI Outline)
        if (GUI.Button(new Rect(Screen.width * 0.35f, Screen.height * 0.65f, Screen.width * 0.3f, Screen.height * 0.2f), "", btn))
        {
            OnMouseDown();
        }
    }

    private void OnMouseDown()
    {
        MenuSoundSource.Play();
        SceneManager.LoadScene("MainScene");
    }
}
