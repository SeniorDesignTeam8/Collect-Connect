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
    GameObject outlineHolder;
    [SerializeField]
    GameObject keyOutline;
    [SerializeField]
    GameObject cardOutline;

    [SerializeField]
    GameObject buttPanel;
    [SerializeField]
    GameObject confirmBtn; //button to confirm their selected keyword/ card
    [SerializeField]
    GameObject yesButt; // button to confirm if the other player guessed the correct connection 
    [SerializeField]
    GameObject noButt; //button to confirm if the other player did not guess the correct connection 

    bool pickingWord;
    public TextMeshProUGUI prompt;

    public TextMeshProUGUI scoreText;

    void Start()
    {

    }
    public void setTurn(bool isturn)
    {
       
        if (!isturn)
        {
            turn = false;
            confirmBtn.SetActive(false);
            holdAmount = 0; //can only hold 1 card as a guess 

        }
        else
        {
            confirmBtn.SetActive(true);
            turn = true;
            holdAmount = 1; // can hold multiple keywords 
        }
        
    }
    public void setPromptText()
    {
        if (!turn)
        {
            prompt.text = "Wait for your turn";
        }
        else if (turn && tutorial.active)
        {
            if (pickingWord)
            {
                prompt.text = "Drag a word to connect on of the smaller cards to the large card. Keep the selected card a secret!";
            }
            else
            {
                prompt.text = "Guess the small card your opponent is thinking of!";
            }
        }
        else prompt.text = "";

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
            pickingWord = true;
            GameObject temp =Instantiate(keyOutline, outlineHolder.transform);
        }
        else
        {
            pickingWord = false;
            Instantiate(cardOutline, outlineHolder.transform);
        }
    }

    public void updateButtonPanel()
    {
        if(!turn)
        {
            prompt.text= "Is this the connection you had in mind?";
            yesButt.transform.SetParent(buttPanel.transform);
            yesButt.SetActive(true);
            noButt.transform.SetParent(buttPanel.transform);
            noButt.SetActive(true);
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
           // invalid.Raise();
        }
        else
        {
            deactivate.Raise();
            endTurn.Raise();
            
        }
    }

}
