using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour {

    public Texture backgroundTexture;
    public Texture titleText;
    public GUIStyle btn;

    void OnGUI()
    {
        //display background texture
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTexture);

        GUI.DrawTexture(new Rect(Screen.width / 4, Screen.height - Screen.height, Screen.width * 0.5f, Screen.height * 0.5f), titleText);

        //display buttons (without GUI Outline)
        if (GUI.Button(new Rect(Screen.width * 0.35f, Screen.height * 0.65f, Screen.width * 0.3f, Screen.height * 0.2f), "", btn))
        {
            OnMouseDown();
        }
    }

    void OnMouseDown()
    {
        SceneManager.LoadScene("MainScene");
    }
}
