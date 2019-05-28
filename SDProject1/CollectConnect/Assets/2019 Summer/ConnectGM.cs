using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ConnectGM : MonoBehaviour
{
    [SerializeField]  TextMeshProUGUI round;
    [SerializeField]  TextMeshProUGUI scoretext;
    int MaxRound=25;
    int currentRound=0;
    int score = 0;


   // [SerializeField]  GameObject keywordSlot;
    [SerializeField]  GameObject cardHolder;
    [SerializeField]  GameObject keywordHolder;

    [SerializeField]  GameObject [] keywordPF;
    [SerializeField]  GameObject cardPF;
    public TMP_InputField inputWord;
    [SerializeField]  GameObject inputWordObj;
    System.Random rnd;

    public GameObject[] cards;
    GameObject[] keywords;
    List<List<int>> availableCards;
    int[] currentCardColl;

    [SerializeField]GameEvent clearBoard;
    // Start is called before the first frame update
    void Start()
    {
        currentCardColl = new int[2];
        rnd = new System.Random();
        cards = new GameObject[2];
        keywords = new GameObject[3];
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
        
        //Remove the collections of the cards that have been dealt
        collections.Remove(currentCardColl[0]);
        collections.Remove(currentCardColl[1]);

        //pick 2 keyword from the collections of the cards
        //one keyword is rare one keyword is common
        helperDealKeywords(0, currentCardColl[0], 0);
        helperDealKeywords(1, currentCardColl[1], 1);



        //pick a random rare keywrod from a collection in the refined collection list
        int coll = rnd.Next(0, collections.Count);
        coll = collections[coll];
        helperDealKeywords(2, coll, 1);

        //place the keywords in the proper place
        foreach (var x in keywords)
        {
            x.transform.SetParent(keywordHolder.transform);
        }

        //for consistency move the input option to be the last
        inputWordObj.transform.SetAsLastSibling();

    }

    void helperDealKeywords(int index, int collection, int rare)
    {
        keywords[index] = Instantiate(keywordPF[0]);
        keywords[index].GetComponentInChildren<TextMeshProUGUI>().text = DataBaseHandler.getKeywordByColl(collection + 1, 0);
        keywords[index].GetComponent<keywordPts>().rare = rare;
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
        clearBoard.Raise();
    }

    void updateScore()
    {
        score += currentSelection.points;
        string choice = currentSelection.choice;
        scoretext.text = "Score: " + score;
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

    public List<string> getListOfKeywords()
    {
        List<string> words = new List<string>();
        foreach(var x in keywords)
        {
            words.Add(x.GetComponentInChildren<TextMeshProUGUI>().text);
        }
        return words;
    }


}
