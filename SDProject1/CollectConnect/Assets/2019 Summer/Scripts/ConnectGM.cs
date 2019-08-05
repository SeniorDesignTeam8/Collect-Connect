using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ConnectGM : MonoBehaviour
{
    [SerializeField] connectPlayer player;
    public int votingPoints = 50;
    int MaxRound=4,currentRound=0;
    Dealer dealKeyWords;
    [SerializeField]  TextMeshProUGUI round;
    [SerializeField]  TextMeshProUGUI scoretext;
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

    public  enum names { Hana, Loki, Player};
    int aiChoices = 0;
    public AImanager AI;
    [SerializeField]GameEvent resetSubmit;
    [SerializeField] GameEvent startSecondMode;

    [SerializeField] GameObject guessers;
    [SerializeField] GameObject EnterKeywordPanel;
    [SerializeField] GameObject GuessHold;
    [SerializeField] GameObject waitingPanel;


    void Start()
    {
        dealKeyWords = GetComponent<Dealer>();
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

    public void setKeyWordsFromSyn()
    {
        for(int i=0; i<3; i++)
        {
            keywords[i] = Instantiate(keywordPF[0]);
            keywords[i].GetComponentInChildren<TextMeshProUGUI>().text = Dealer.keywords[i];
            keywords[i].transform.SetParent(keywordHolder.transform);
        }
    }
    public void startRound()
    {
        collectVotes();
        //StopAllCoroutines();
        EnterKeywordPanel.SetActive(true);
        GuessHold.SetActive(false);
        aiChoices = 0;
        if (currentRound < MaxRound)
        {
            //clear cards from the previous round done here and not in round over because
            // the cards need to be displayed for both the first and second half of the round 
            deleteCards();
            dealCards();

            readCardTags.getDealtCardsName(cards);

            //should start loading screen with and when the 
            //events shoots off that all cards have been loaded remove lodaing screen
            dealKeyWords.stepsGetKeywords();

            AI.startAIGuess();
            //deal keywords from database
            //           dealKeywords();

        }
    }
    //called when the user submits their keyword
    public void firstHalfRoundOver()
    {
        string choice = currentSelection.choice;
        updateScore(currentSelection.points);
        deleteWords();
        currentRound++;
        round.text = "Round " + (currentRound + 1) + "/" + MaxRound.ToString();
        //make the selected words panel inactive so they cannot spam the submit  button
        EnterKeywordPanel.SetActive(false);
        secondHalfofRound();       
    }
 

    void updateScore(int points)
    {
        player.points += points;
        scoretext.text = "Score: " + player.points;
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

    public void secondHalfofRound()
    {
        BasicGuesser bg = guessers.GetComponent<BasicGuesser>();
        TricksterGuesser tg = guessers.GetComponent<TricksterGuesser>();
        while(!bg.done && !tg.done)
        { }
        //while the ai are not ready put up a loading screen 
        GuessHold.SetActive(true);
        player.readyToCastVote(GuessHold);
        AI.showChoices();
        AI.castVotes();
    }

    public void collectVotes()
    {
        GameObject[] wordsForJudging = GameObject.FindGameObjectsWithTag("VotingKeyword");
        for (int i = wordsForJudging.Length - 1; i >= 0; i--)
        {
            castVote Temp = wordsForJudging[i].GetComponent<castVote>();
            switch ((names)Temp.ownedBy)
            {
                case names.Hana:
                    AI.addPoints(names.Hana, votingPoints*Temp.getPoints());
                    break;
                case names.Loki:
                    AI.addPoints(names.Loki, votingPoints * Temp.getPoints());
                    break;
                case names.Player:
                    updateScore(votingPoints * Temp.getPoints());
                    break;
            }

             Destroy(wordsForJudging[i]);
        }
    }

    public void enableWaitingPanel(bool enable)
    {
        waitingPanel.SetActive(enable);
    }
}
