using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class keywordInput : MonoBehaviour
{
    TMP_InputField word;
    DragItems movable;
    // Start is called before the first frame update
    void Start()
    {
        word = GetComponentInChildren<TMP_InputField>();
        movable = GetComponent<DragItems>();
        movable.canBeMoved = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if the word is invalid return it to spawn area
        if (word.text == "")
            transform.SetParent(movable.spawn);
    }
    public void setWord()
    {
        //later need to turn this into actual checks 
        //whether or not it is a word 
        //and other pt value checks 
        if (word.text!="")
            movable.canBeMoved = true;

    }

}
