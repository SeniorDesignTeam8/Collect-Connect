using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

public class keywordInput : MonoBehaviour
{
    bool validInput=false;
    TMP_InputField word;
    currentSelection selectable;
    public int points = 0;
    [SerializeField] GameEvent keywordClicked;
    // Start is called before the first frame update
    void Start()
    {
        word = GetComponentInChildren<TMP_InputField>();
        selectable = GetComponent<currentSelection>();
    }
    [SerializeField]GameEvent resetBoard;

    public void setWord()
    {
        validInput = false;
        
        //if it has any white spaces dont even check dictionary.com
        string pattern = @"\s+";
        if (Regex.IsMatch(word.text, pattern)||string.IsNullOrEmpty(word.text))
            return;

        //create a link to see if searching the word on dictionary.com will be a valid request
        string check = "https://www.dictionary.com/browse/"+word.text;

       

        Debug.Log(check);
        //start a coroutine to pull the web page 
        StartCoroutine(checkIfWord(check));


    }
    public void allowSelection()
    {
        //non words do not trigger the submit button 
        if (!validInput)
        {
            selectable.enabled = true;
            //if the last selected item was the inputfield it needs to be cleard from being the selection
            if (currentSelection.selected = gameObject)
                resetBoard.Raise();
        }
        //valid input raises the event that the submit button can be clicked
        else
            keywordClicked.Raise();
    }

    IEnumerator checkIfWord(string uri)
    {
        //https://www.dictionary.com/browse/
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            //  string[] separatingStrings = { @"{""word"":""", "......." ," "};

            string pages = webRequest.downloadHandler.text;
            //StringComparison compare = StringComparison.OrdinalIgnoreCase;
            validInput = pages.Contains("description");
            allowSelection();
                if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }

        }
    }

    private void setPts()
    {
        //get all other keywords on screen
        //if they choose the same word that was already prs
    }

}
