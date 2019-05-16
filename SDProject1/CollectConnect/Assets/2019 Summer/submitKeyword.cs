using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class submitKeyword : MonoBehaviour
{
    Button submit;
    [SerializeField]
    Sprite active;
    [SerializeField]
    Sprite inactive;

    public GameObject keywordHolder;
    private void Start()
    {
        submit = GetComponent<Button>();
    }
    void Update()
    {
        if (keywordHolder.transform.childCount > 0)
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

}
