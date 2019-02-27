using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerLogic : MonoBehaviour
{
    public int holdAmount = 1;
    public int score = 0;
    public bool turn;
    [SerializeField]
    GameEvent invalid;
    [SerializeField]
    GameEvent deactivate;
    [SerializeField]
    GameEvent endTurn;
    [SerializeField]
    GameObject confirmBtn;

    [SerializeField]
    GameObject outlineHolder;
    [SerializeField]
    GameObject keyOutline;
    [SerializeField]
    GameObject cardOutline;


   public GameObject panel;

    public TextMeshProUGUI scoreText;

    void Start()
    {

    }
    public void setTurn(bool isturn)
    {
       
        if (!isturn)
        {
            panel.SetActive(true);
            turn = false;
            confirmBtn.SetActive(false);
            holdAmount = 0; //can only hold 1 card as a guess 

        }
        else
        {
            panel.SetActive(false);
            confirmBtn.SetActive(true);
            turn = true;
            holdAmount = 1; // can hold multiple keywords 
        }
    }

    //call this when there is a new round, since this only changes by round not turn 
    public void changeOutline()
    {
        for(int i =0; i< outlineHolder.transform.childCount;i++)
        {
           Destroy(outlineHolder.transform.GetChild(i).gameObject);
        }
        outlineHolder.transform.DetachChildren();
        if(turn)
        {
            GameObject temp =Instantiate(keyOutline, outlineHolder.transform);
        }
        else
        {
            Instantiate(cardOutline, outlineHolder.transform);
        }
    }

    public void toggleConfirm(bool active)
    {
        confirmBtn.SetActive(active);
    }

    public void confirmChoice()
    {
        if(transform.childCount<1)
        {
            invalid.Raise();
        }
        else
        {
            deactivate.Raise();
            endTurn.Raise();
            
        }
    }

}
