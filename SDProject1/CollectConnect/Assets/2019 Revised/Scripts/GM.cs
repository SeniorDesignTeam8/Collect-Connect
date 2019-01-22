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
    PlayerLogic [] players;
    System.Random rnd;
    PlayerLogic currentPlayer;
    static IDbConnection dbConnect;
    static List<GameObject> Deck;
    static List<string> wordbank;
    List<string> activeWords;
    [SerializeField]
    int MaxRound;
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
    GameObject approveSelect;

    void Awake ()
    {
        currentRound = new Vector2(0,0); // each round will consist of the players changing turn once 
        rnd = new System.Random();
        startGame();
	}
	void startGame()
    {
        // sets up the players, and assigns who goes first randomly 
        int x = rnd.Next(0,players.Length-1);
        currentPlayer = players[x];
        currentPlayer.setTurn(true);
        foreach ( var player in players)
        {
            if(player!= players[x])
            {
                player.setTurn(false);
            }
        }
        // collects the cards and keywords from the database
        activeWords = new List<string>();
        wordbank = new List<string>();
        Deck = new List<GameObject>();
        string conn = "URI=file:" + Application.dataPath + "/CollectConnectDB.db";
        dbConnect = (IDbConnection)new SqliteConnection(conn);
        dbConnect.Open();
        BuildDeck();

    }
    public void changeTurn()
    {
        if(currentRound.y==0)
        {
            //change player turn
            foreach (var x in players)
            {
                x.setTurn(!x.turn);
            }

            activateCards.Raise();
            currentRound.y++;
        }
        else // dont change player turn 
        {
            //ask player1 to check choice pop up
                 //give points 
            approveSelect.SetActive(true);


        }
    }

    public void finishRound()
    {
            roundOver.Raise(); //clear the board
            currentRound.y = 0;
            currentRound.x++;          
            newRound(); //call new round
    }

    public void givePoints()
    {
        foreach (var x in players)
        {
            x.score+= x.transform.childCount;
            x.scoreText.text = x.score.ToString();
        }
        Invoke("finishRound", .15f);
    }
    public void newRound()
    {
        if(currentRound.x<MaxRound)
        {
            newRoundStart.Raise();
        }
        else
        {
            //game over 
            //announce winner 
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
                GameObject cards = Instantiate(GameObject.Find("card"));
                //cards.AddComponent<CardInfo>();
                //CardInfo cardComponent = new CardInfo();    //cards.GetComponent<CardBase>();
                CardInfo cardComponent = cards.GetComponent<CardInfo>();
                cardComponent.name = (string)rd["cardDisplayTitle"];

                string raw = (string)rd["cardDescription"];
                string s = raw;
                cardComponent.setCardDes(s);
                int cardId = (int)(long)rd["cardID"];
                s = (string)rd["imageFileName"];
                cardComponent.setCardID(cardId);
                cardComponent.setImageLocation(s);
                raw = (string)rd["Location"];
                string loc = raw;
                cardComponent.setCardSourceLoc(loc);

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



                    string attr;

                    if (cat == "Concept")
                    {
                        cardComponent.setCatParam(cat, param);
                        wordbank.Add(param);
                    }
                    else if (cat == "Location")
                    {
                        rawString = (string)kwReader["attribute"];
                        attr = rawString;

                        cardComponent.setCardLocation(attr + ": " + param + "\n");
                    }
                    else if (cat == "Person")
                    {
                        rawString = (string)kwReader["attribute"];
                        attr = rawString;
                        cardComponent.setCardPeople(attr + ": " + param + "\n");
                    }
                    else if (cat == "Medium")
                    {

                        cardComponent.setCardMedium(cat + ": " + param + "\n");
                    }
                    else if (cat == "Year")
                    {
                        if (kwReader["attribute"] != DBNull.Value)
                        {
                            rawString = (string)kwReader["attribute"];
                            attr = rawString;
                            cardComponent.setCardYear(attr + ": " + param + "\n");
                        }
                        else
                        {
                            cardComponent.setCardYear(cat + ": " + param + "\n");
                        }
                    }
                    else if (cat == "Language")
                    {
                        cardComponent.setCardLang(cat + ": " + param + "\n");
                    }

                }
                kwReader.Close();
                kwReader = null;
                cards.SetActive(false);
                Deck.Add(cards);
            }
            rd.Close();
            rd = null;
        }
    }

    public GameObject createCardObject()
    {

        int pick = rnd.Next(Deck.Count - 1);
        int i = pick;
        Deck[pick].SetActive(true);
        CardInfo reset = Deck[pick].GetComponent<CardInfo>();
        TextMeshProUGUI des = Deck[pick].GetComponentInChildren<TextMeshProUGUI>();
        des.text = reset.finalDescription();
        Component[] images;
        images = Deck[pick].GetComponentsInChildren<Image>();
        Image art = Deck[pick].GetComponentInChildren<Image>();
        foreach (Image x in images)
        {
            if (x.tag == "art")
            {
                art = x;
            }

        }
        art.sprite = reset.cardPic;
        GameObject newCard = Deck[pick];
        //stores the keyword associated with the card that was chosen 
        activeWords.Add(wordbank[pick]);
        wordbank.RemoveAt(pick);

        Deck.RemoveAt(pick);
        return newCard;

    }

    public GameObject getKeywords()
    {
        GameObject keyword = Instantiate(keywordPF);
        if (activeWords.Count != 0)
        {
            int x = rnd.Next(0, activeWords.Count - 1);
            keyword.GetComponentInChildren<TextMeshProUGUI>().text= activeWords[x];
            activeWords.RemoveAt(x);
        }
        return keyword;

    }
    // function to make keyword objects 
    //return those keyword 
    
    
    //use events ???
    // changing turn 
    //delete all cards and keywords
    // update scores
    // 

}
