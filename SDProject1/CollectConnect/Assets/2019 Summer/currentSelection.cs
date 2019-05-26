using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
public class currentSelection : MonoBehaviour, IPointerClickHandler
{
    public static int points;
    public static GameObject selected;
    public TextMeshProUGUI textPro;
    public TMP_InputField textInput;
    public static string choice;
    float time;
    [SerializeField]GameEvent keywordClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        onDoubleClick();
    }
    void onDoubleClick()
    {
        
        selected = gameObject;
        if (textPro != null)
            choice = textPro.text;
        else if (textInput != null)
            choice = textInput.text;
        points= selected.GetComponent<keywordPts>().pts;

        keywordClicked.Raise();
    }
    public void reset()
    {
        selected = null;
        choice = null;
        points = 0;
    }

}
