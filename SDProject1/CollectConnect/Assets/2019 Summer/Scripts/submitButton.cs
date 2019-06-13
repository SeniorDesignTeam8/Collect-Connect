using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class submitButton : MonoBehaviour
{
    Button submit;
    [SerializeField]
    Sprite active;
    [SerializeField]
    Sprite inactive;

    bool userChoice = false;

    public GameObject keywordHolder;
    private void Start()
    {
        submit = GetComponent<Button>();
    }
    void Update()
    {
        if (userChoice)
        {
            submit.interactable = true;
            submit.image.sprite = active;
        }
        else
        {
            submit.interactable = false;
            submit.image.sprite = inactive;
            
        }
    }
    public void setUserChoice(bool ready)
    {
        userChoice = ready;
    }
}
