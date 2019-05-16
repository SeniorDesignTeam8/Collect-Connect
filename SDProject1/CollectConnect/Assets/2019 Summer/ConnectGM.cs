using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ConnectGM : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI round;
    [SerializeField]
    TextMeshProUGUI scoretext;
    int MaxRound=25;
    int currentRound=0;
    int score = 0;


    [SerializeField]
    GameObject keywordSlot;
    [SerializeField]
    GameObject cardHolder;
    [SerializeField]
    GameObject keywordHolder;

    [SerializeField]
    GameObject [] keywordPF;
    [SerializeField]
    GameObject cardPF;
    public TMP_InputField inputWord;
    [SerializeField]
    GameObject inputWordObj;
    System.Random rnd;

    GameObject[] cards;
    GameObject[] keywords;
    List<List<int>> availableCards;
    int[] currentCardColl;

    int inputVal=0;

    // Start is called before the first frame update
    void Start()
    {
        currentCardColl = new int[2];
        rnd = new System.Random();
        cards = new GameObject[2];
        keywords = new GameObject[5];
        availableCards = DataBaseHandler.getAllCards();
        startRound();
    }

    //creates 2 cards from different collections 
    //sets them on the board
    void dealCards()
    {
        GameObject cardObj = Instantiate(cardPF);
        GameObject cardObj2 = Instantiate(cardPF);
        // Pick 2 cards from different collections 
        currentCardColl[0] = rnd.Next(0, GM.collNames.Count);

        //Make sure the collection chosen has crds left in it
        while(availableCards[currentCardColl[0]].Count == 0)
        {
            currentCardColl[0] = rnd.Next(0, GM.collNames.Count);
        }

        //Make sure it is a different collection than the first card
        //make sure that collection has card in it to choose from
        currentCardColl[1] = rnd.Next(0, GM.collNames.Count);
        while(currentCardColl[0] == currentCardColl[1] || availableCards[currentCardColl[1]].Count==0)
        {
            currentCardColl[1] = rnd.Next(0, GM.collNames.Count);
        }


        // pick a specific card from the first collection chosen
        int card1 = rnd.Next(0, availableCards[currentCardColl[0]].Count);
        int card2 = rnd.Next(0, availableCards[currentCardColl[1]].Count);

        //the card will save its particular id and set its image 
        cardObj.GetComponent<cardID>().coll_id = currentCardColl[0] + 1;
        cardObj.GetComponent<cardID>().setImageName(GM.collNames[currentCardColl[0]], availableCards[currentCardColl[0]][card1]);
        cardObj.GetComponent<cardID>().setImage();

        //the card will save its particular id and set its image 
        cardObj2.GetComponent<cardID>().coll_id = currentCardColl[1] + 1;
        cardObj2.GetComponent<cardID>().setImageName(GM.collNames[currentCardColl[1]], availableCards[currentCardColl[1]][card2]);
        cardObj2.GetComponent<cardID>().setImage();

        availableCards[currentCardColl[0]].RemoveAt(card1);
        availableCards[currentCardColl[1]].RemoveAt(card2);

        cards[0] = cardObj;
        cards[1] = cardObj2;
        foreach(var x in cards)
        {
            x.transform.SetParent(cardHolder.transform);
        }
 
    }
    void dealKeywords()
    {
        //list of all the collections
        List<int> collections = Enumerable.Range(0, 7).ToList();
        for(int i =0; i<2; i++)
        {
            keywords[i] = Instantiate(keywordPF[0]);
            keywords[i].GetComponentInChildren<TextMeshProUGUI>().text = DataBaseHandler.getKeywordByColl(currentCardColl[i]+1, 0);
            keywords[i].GetComponent<keywordPts>().rare = 0;
            //remove the collections that cards were dealt from
            //so that we can pick a keyword from a non dealt collection 
            collections.Remove(currentCardColl[i]);
        }
        for (int i = 2; i < 4; i++)
        {
            keywords[i] = Instantiate(keywordPF[1]);
            keywords[i].GetComponentInChildren<TextMeshProUGUI>().text = DataBaseHandler.getKeywordByColl(currentCardColl[i-2]+1, 1);
            keywords[i].GetComponent<keywordPts>().rare = 0;
        }

        //pick a random are keywrod from a collection not chosen 
        int coll = rnd.Next(0, collections.Count);
        coll = collections[coll];
        keywords[4] = Instantiate(keywordPF[2]);
        keywords[4].GetComponentInChildren<TextMeshProUGUI>().text = DataBaseHandler.getKeywordByColl(coll+1, 1);
        keywords[4].GetComponent<keywordPts>().rare = 1;
        //pick 1 keyword that is not in the

        //deal 5 keywords
        foreach (var x in keywords)
        {
            x.transform.SetParent(keywordHolder.transform);
            x.GetComponent<DragItems>().canBeMoved = true;
        }
        inputWordObj.transform.SetAsLastSibling();
        inputWordObj.GetComponent<DragItems>().canBeMoved = false;

    }
    public void startRound()
    {
        if (currentRound < MaxRound)
        {
            dealCards();
            dealKeywords();
        }
    }
    public void roundOver()
    {
        saveRound();
        updateScore();
        deleteCards();
        deleteWords();
        currentRound++;
        round.text = "Round " + (currentRound + 1) + "/" + MaxRound.ToString();
    }

    void updateScore()
    {
        if (keywordSlot.transform.GetChild(0).gameObject.tag == "playerWord")
            score += inputVal;
        else score += keywordSlot.transform.GetChild(0).GetComponent<keywordPts>().pts;
        scoretext.text = "Score: " + score;
    }
    public void checkInput()
    {
        //if the word they input was on screen they recieve -5 pts
        inputVal = -5;
        //else if check to see if its in the database at all
        inputVal = 25;
        //else 
        inputVal = 40;

    }
    void saveRound()
    {
        //call database things here
    }
    void deleteWords()
    {
        inputWord.text = "";
        inputWordObj.transform.SetParent(keywordHolder.transform);
        foreach( var x in keywords)
        {
            Destroy(x);
        }
    }

    void deleteCards()
    {
        foreach (var x in cards)
        {
            Destroy(x);
        }
    }


}
