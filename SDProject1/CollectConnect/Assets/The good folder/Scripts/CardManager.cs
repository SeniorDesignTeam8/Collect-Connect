using Mono.Data.Sqlite;
using System;

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour{

    public static IDbConnection dbConnect;
    public static List<GameObject> Deck;
    public List<GameObject> returned;
    public static List<string> wordbank;
    System.Random rnd;
    //  public GameObject cards;


    //pulls every object from the database and puts them in a deck
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


    //deals 4 crads to the players
    public void dealCards()
    {
        GameObject card;
        //for (int i = 0; i < 4;i++)
        //{
            card = createCardObject();
            card.transform.SetParent(GameObject.Find("Player1").transform);
       // }
    }

    //creates the actual card gameobject
    public GameObject createCardObject()
    {
        
        int pick = rnd.Next(Deck.Count-1);
        int i = pick;
        Deck[pick].SetActive(true);
        CardInfo reset = Deck[pick].GetComponent<CardInfo>();
        Text des= Deck[pick].GetComponentInChildren<Text>();
        des.text=reset.finalDescription();
        Component[] images;
        images=Deck[pick].GetComponentsInChildren<Image>();
        Image art= Deck[pick].GetComponentInChildren<Image>();
        Image wordBack= Deck[pick].GetComponentInChildren<Image>();
        foreach (Image x in images)
        {
            if(x.tag=="art")
            {
                art = x;
            }
            if(x.tag=="wordBacking")
            {
                wordBack = x;
            }
           
        }
        wordBack.enabled = false;
        des.enabled = false;
        art.sprite= reset.cardPic;
        GameObject newCard = Deck[pick];//Deck[pick].transform.SetParent(GameObject.Find("Player1").transform);
      
        Deck.RemoveAt(pick);
        return newCard;
        
    }

    public void returnCard()
    {
        returned.Add(GameObject.FindGameObjectWithTag("return").transform.GetChild(0).gameObject);
    }
        
    void Start ()
    {
        returned = new List<GameObject>();
        wordbank = new List<string>();
        string conn = "URI=file:" + Application.dataPath + "/CollectConnectDB.db";
        dbConnect = (IDbConnection)new SqliteConnection(conn);
        dbConnect.Open();
        Deck = new List<GameObject>();
        rnd = new System.Random();
        BuildDeck ();
        Invoke("dealCards", .5f);
        Invoke("dealCards", .5f);
        Invoke("dealCards", .5f);
        Invoke("dealCards", .5f);
    }
	
	
}
