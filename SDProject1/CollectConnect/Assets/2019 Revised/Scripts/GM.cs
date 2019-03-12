using Mono.Data.Sqlite;
using System;

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GM : MonoBehaviour
{
    [SerializeField]
    public PlayerLogic [] players;
    System.Random rnd;
    PlayerLogic currentPlayer;
    static IDbConnection dbConnect;
    static IDbConnection wordConnect;

    [SerializeField]
    GameObject regCard;
    [SerializeField]
    GameObject parentCrd;

 //   List<List<int>> deck;
    List<int> HPR; //id 1
    List<int> IRC; //id 2
    List<int> HIC; //id 3
    List<int> FUAB; //id 4
    int[] collectionOnScreen;

    static List<string> wordbank;
    [SerializeField]
    public int MaxRound;
    Vector2 currentRound;
    [SerializeField]
    GameObject keywordPF;

    [SerializeField]
    GameEvent roundOver; //deletes current cards and keywrods

    [SerializeField]
    GameEvent newRoundStart; //has to deal all new cards and keywords

    [SerializeField]
    GameEvent activateCards; //allows cards to be moved on second players turn

    [SerializeField]
    GameEvent gameOver;

    [SerializeField]
    endGameStats stats;
    bool wasCorrect = false;
    void Awake ()
    {
        collectionOnScreen = new int[4];
        currentRound = new Vector2(0,0); // each round will consist of the players changing turn once 
        rnd = new System.Random();
        startGame();
	}
	void startGame()
    {
        // sets up the players, and assigns who goes first randomly 
        int x = rnd.Next(0,players.Length);
        currentPlayer = players[x];
        currentPlayer.setTurn(true);
        foreach ( var player in players)
        {
            if(player!= players[x])
            {
                player.setTurn(false);
            }
            player.changeOutline(); // gives the outline of what object they should be holding 
            player.setPromptText();
        }
        // collects the cards and keywords from the database
        wordbank = new List<string>();
       
        string conn = "URI=file:" + Application.dataPath + "/CollectConnectDB.db";
        string conn2= "URI=file:" + Application.dataPath + "/testDB.db";
        dbConnect = (IDbConnection)new SqliteConnection(conn);
        wordConnect = new SqliteConnection(conn2);

        dbConnect.Open();
        wordConnect.Open();
        BuildDeck();
        initCards();
        keyWordData();

    }


    void initCards()
    {
        HPR = Enumerable.Range(1,8).ToList();//8
        IRC = Enumerable.Range(1, 4).ToList();//4
        HIC = Enumerable.Range(1, 6).ToList();//6
        FUAB = Enumerable.Range(1, 10).ToList();//10
    }
    public GameObject dealCard(int numCard, bool parent)
    {
        GameObject dealtCard;
        int collection = rnd.Next(1, 5);   //pick a collection 
        if (parent)
        {
            dealtCard = Instantiate(parentCrd);
        }
        else dealtCard = Instantiate(regCard);

        

        cardChosen(dealtCard, collection);  //from that collection see if you can pull a card
        collectionOnScreen[numCard] = dealtCard.GetComponent<cardID>().coll_id;  //once a card is chosen save which collection it came from so you can pick the appropiate keywords

        dealtCard.GetComponent<cardID>().setImage();
        return dealtCard;
    }
    void cardChosen(GameObject cardObj, int  col)
    {
        if (col == 1)
        {
            if (HPR.Count == 1)
            {
                 cardChosen(cardObj,col + 1);
            }
            else
            {
                int pick = rnd.Next(0, HPR.Count);
                cardObj.GetComponent<cardID>().coll_id = col;
                cardObj.GetComponent<cardID>().setImageName("HPR_", HPR[pick]);

            }
        }
        else if (col == 2)
        {
            if (IRC.Count == 0)
            {
                cardChosen(cardObj,col + 1);
            }
            else
            {
                int pick = rnd.Next(0, IRC.Count);
                cardObj.GetComponent<cardID>().coll_id = col;
                cardObj.GetComponent<cardID>().setImageName("IRC_", IRC[pick]);

            }
        }
        else if (col == 3)
        {
            if (HIC.Count == 0)
            {
                cardChosen(cardObj,col + 1);
            }
            else
            {
                int pick = rnd.Next(0, HIC.Count);
                cardObj.GetComponent<cardID>().coll_id = col;
                cardObj.GetComponent<cardID>().setImageName("HIC_", HIC[pick]);
            }
        }
        else if (col == 4)
        {
            if (FUAB.Count == 0)
            {
                cardChosen(cardObj,1);
            }
            else
            {
                int pick = rnd.Next(0, FUAB.Count);
                cardObj.GetComponent<cardID>().coll_id = col;
                cardObj.GetComponent<cardID>().setImageName("FUAB_", FUAB[pick]);

            }
        }
    }
    public void setUp()
    {

        foreach (var player in players)
        {
            player.setTurn(player.turn);
            player.changeOutline(); // gives the outline of what object they should be holding 
        }
        clearObjects();
        initCards();
        BuildDeck();
        newRound();
        currentRound = new Vector2(0, 0);
    }
    void clearObjects()
    {
        HPR.Clear();
        IRC.Clear();
        HIC.Clear();
        FUAB.Clear();
        wordbank.Clear();
        foreach (var x in players)
        {
            x.score =0;
        }
    }
    public void changeTurn()
    {
        if(currentRound.y==0)
        {
            //change player turn
            foreach (var x in players)
            {
                x.setTurn(!x.turn);
                x.setPromptText();
            }

            activateCards.Raise();
            currentRound.y++;
        }
        else // dont change player turn 
        {

           // approveSelect.SetActive(true);
            foreach(var x in players)
            {
                x.toggleConfirm(false);
                x.updateButtonPanel();
            }
        }
    }

    public void finishRound()
    {
        foreach (var x in players)
        {
            if (x.turn)
            {
                //set card in stats
                stats.setCard(x.transform.GetChild(0).gameObject);
            }
            else
            {
                stats.setKeyWord(x.transform.GetChild(0).gameObject);
            }
        }
        stats.setCorrect(wasCorrect);
        roundOver.Raise(); //clear the board
        currentRound.y = 0;
        currentRound.x++;          
        newRound(); //calls a new round without changing turns 
        wasCorrect = false;
    }


    public void givePoints()
    {
        wasCorrect = true;
        foreach (var x in players)
        {
            //will need to change based on the value of the keyword
            if(!x.turn)
                x.score += 1;
        }
        // Save the parent Card
        // The Choosen Key word
        // The Guessed Card


        //Clears the board and increments round 
        Invoke("finishRound", .15f);
    }
    public void newRound()
    {
        if(currentRound.x<MaxRound)
        {
            newRoundStart.Raise();
            foreach (var x in players)
            {
                x.toggleConfirm(x.turn);
                x.changeOutline();
                x.setPromptText();
            }
        }
        else
        {
            gameOver.Raise();
        }
        
    }

    public GameObject getKeywords()
    {
        GameObject keyword = Instantiate(keywordPF);
        if (wordbank.Count != 0)
        {
            int x = rnd.Next(0, wordbank.Count);
            keyword.GetComponentInChildren<TextMeshProUGUI>().text= wordbank[x];
            wordbank.RemoveAt(x);
        }
        return keyword;

    }
    void keyWordData()
    {
        List<string> words = new List<string>();
        IDbCommand dbcmd = wordConnect.CreateCommand();
        const string query = "SELECT NAME FROM keywords_current WHERE coll_id=2";
        dbcmd.CommandText = query;

        try
        {
            int i = 0;
            IDataReader rd = dbcmd.ExecuteReader();
            while (rd.Read())
            {
                words.Add(rd.GetString(0));
                Debug.Log(words[i]);
                i++;
            }
            rd.Close();
            rd = null;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }
    }

    private static void BuildDeck()
    {
     
        IDbCommand dbcmd = dbConnect.CreateCommand();

        // Load the collections.
        List<string> collectionList = new List<string>();
        List<int> collectionIdList = new List<int>();
        try
        {
            const string sqlQuery = "SELECT * FROM sets"; // get id of last card inserted into cards table
            dbcmd.CommandText = sqlQuery;
            IDataReader rd = dbcmd.ExecuteReader();
            while (rd.Read())
            {
                collectionIdList.Add(rd.GetInt32(0));
                collectionList.Add(rd.GetString(1));
            }
            rd.Close();
            rd = null;

            collectionList = collectionList.Distinct().ToList(); // Remove any duplicates.
            collectionIdList = collectionIdList.Distinct().ToList(); // Remove any duplicates.
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }
        // Load the artifacts from each collection to make cards from them. Then add them to their respective lists.
        foreach (string col in collectionList)
        {
            int index = collectionList.IndexOf(col);
            int setId = collectionIdList[index];

            string sqlQuery = "SELECT * FROM cards INNER JOIN sets ON cards.setID = sets.setID WHERE cards.setID = " + setId + " AND cards.Location<>''";// get id of last card inserted into cards table
                                                                                                                                                         //Debug.Log(sqlQuery);
            dbcmd.CommandText = sqlQuery;
            IDataReader rd = dbcmd.ExecuteReader();
            while (rd.Read())
            {



                string raw = (string)rd["cardDescription"];
                string s = raw;

                int cardId = (int)(long)rd["cardID"];
                s = (string)rd["imageFileName"];

                raw = (string)rd["Location"];
                string loc = raw;


                string keywordQuery = "SELECT cat.category, param.parameter, attr.attribute FROM cards c NATURAL JOIN categories_parameters_attributes cpa LEFT OUTER JOIN categories cat ON cpa.categoryID = cat.categoryID LEFT OUTER JOIN parameters param ON cpa.parameterID = param.parameterID LEFT OUTER JOIN attributes attr ON cpa.attributeID = attr.attributeID WHERE cardID = " + cardId + " ORDER BY CATEGORY, ATTRIBUTE, PARAMETER";
                //Debug.Log (keywordQuery);
                IDbCommand kwCmd = dbConnect.CreateCommand();
                kwCmd.CommandText = keywordQuery;
                IDataReader kwReader = kwCmd.ExecuteReader();
                //Debug.Log(kwCmd.ToString() + "    " + kwReader.ToString() + "    " + (int)(long)kwReader["cpa.pointValue"]);
                while (kwReader.Read())
                {
                    //string attr;
                    string rawString = (string)kwReader["category"];
                    string cat = rawString;

                    rawString = (string)kwReader["parameter"];
                    string param = rawString;


                    if (cat == "Concept")
                    {
                        wordbank.Add(param);
                    }


                }
                kwReader.Close();
                kwReader = null;

            }
            rd.Close();
            rd = null;
        }
    }



}
