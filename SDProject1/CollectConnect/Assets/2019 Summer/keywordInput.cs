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
    ConnectGM GM;
    bool validInput=false;
    TMP_InputField word;
    currentSelection selectable;
    [SerializeField] GameEvent keywordClicked;
    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("InfoPanel").GetComponent<ConnectGM>();
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
        {
            setPts();
            selectable.setUserInput();
            keywordClicked.Raise(); }
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
        List<string> words = GM.getListOfKeywords();
        keywordPts keyInfo = GetComponent<keywordPts>();
        //user input keyword worth 40 points
        keyInfo.pts = 40;
        //if it was in the database it is worth less
        if (DataBaseHandler.isKeywordInDataBase(word.text))
            keyInfo.pts = 20;

        //if it ison screen and they try to cheese it, they get a deduction in points
        foreach(string x in words)
        {
            if (x.Equals(word.text, StringComparison.InvariantCultureIgnoreCase))
                keyInfo.pts = -5;
        }
        Debug.Log(keyInfo.pts.ToString());
    }

}
