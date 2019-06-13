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

    [SerializeField]GameEvent keywordClicked;
    [SerializeField] GameEvent userWordClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.pointerPress.GetComponent<currentSelection>()!=null)
            onDoubleClick();
    }
    void onDoubleClick()
    {
      
        selected = gameObject;
        points= selected.GetComponent<keywordPts>().pts;
        //if it is a regular keyword
        if (textPro != null)
        {
            choice = textPro.text;
            keywordClicked.Raise();
        }

        //else the user input option was selected 
        else if (textInput != null)
        {
            choice = textInput.text;
            userWordClicked.Raise();
        }
         
    }
    public void setUserInput()
    {
        selected = gameObject;
        points = selected.GetComponent<keywordPts>().pts;
        choice = textInput.text;
    }
    public void reset()
    {
        selected = null;
        choice = null;
        points = 0;
    }

}
