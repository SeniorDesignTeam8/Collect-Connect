using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuitButton : MonoBehaviour
{ Button button;
    // Use this for initializationvoid 
   void Awake ()
    { button = GetComponent<Button>();
        button.onClick.AddListener(() => { whenClicked(); } );
    }
    void whenClicked()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}