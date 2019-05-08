using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    Color maketransparent;
    Color startColor;
    [SerializeField]
    GameObject largeDropArea;

    public GameObject scorePopUp;

    public Sprite inactiveConfirm;
    public Sprite activeConfirm;
    bool pickingWord;
    public TextMeshProUGUI prompt;

    public Sprite[] ptVals;
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
            largeDropArea.SetActive(false);
        }
        else
        {
            largeDropArea.SetActive(true);
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
            {                 //                                                                                        //
                prompt.text = "Select a keyword that relates 1 of the 3 cards on the right to the card on the left";
            }
            else
            {
                prompt.text = "Guess the card your opponent is thinking of!";
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
            if(transform.GetChild(0).tag=="keyWord")
            {
                GM.maxRoundPts = transform.GetChild(0).GetComponent<keywordPts>().pts;
            }
            deactivate.Raise();
            endTurn.Raise();
            
        }
    }
    private void Update()
    {
        if(confirmBtn.activeSelf)
        {
            if (transform.childCount == 0)
            {
                confirmBtn.GetComponent<Image>().sprite = inactiveConfirm;
                largeDropArea.SetActive(true);
            }
            else
            {
                confirmBtn.GetComponent<Image>().sprite = activeConfirm;
                largeDropArea.SetActive(false);
            }

        }
        if(scorePopUp.activeSelf)
        {
            maketransparent.a -= .008f;
            Color lerpedColor = Color.Lerp(maketransparent, startColor, Time.deltaTime);
            scorePopUp.GetComponent<Image>().color = maketransparent;
            
            if (maketransparent.a <= 0)
                turnoffPopup();
        }
    }

    void turnoffPopup()
    {
        scorePopUp.SetActive(false);
        scorePopUp.GetComponent<Image>().color = startColor;



    }
    public void addScore(int pts)
    {
        score += pts;
        if(pts==-5)
        {
            scorePopUp.GetComponent<Image>().sprite = ptVals[5];
        }
        else if (pts == 40)
            scorePopUp.GetComponent<Image>().sprite = ptVals[4];
        else if (pts == 20)
            scorePopUp.GetComponent<Image>().sprite = ptVals[3];
        else if (pts == 10)
            scorePopUp.GetComponent<Image>().sprite = ptVals[2];
        else if (pts == 5)
            scorePopUp.GetComponent<Image>().sprite = ptVals[1];
        else 
            scorePopUp.GetComponent<Image>().sprite = ptVals[0];

        scorePopUp.SetActive(true);
        startColor = scorePopUp.GetComponent<Image>().color;
        maketransparent = startColor;
    }
    public void updatePts()
    {
        scoreText.text = "Score: "+score.ToString();
    }

}
