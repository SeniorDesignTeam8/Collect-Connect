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
    public PlayerLogic[] players;
    System.Random rnd;
    PlayerLogic currentPlayer;
    static IDbConnection dbConnect;

    static List<string> collNames = new List<string>() { "HPR_", "IRC_", "HIC_", "FUAB_", "BCG_", "AAG_", "AW_", "KAG_" };  //order of names here matters!
    List<List<int>> availableCards; // every new game will hold all cards from database

    [SerializeField]
    GameObject regCard;
    [SerializeField]
    GameObject parentCrd;


    public GameObject popupPanel;

    List<int> inColls;
    List<int> outColls;

    public int MaxRound;
    Vector2 currentRound;
    [SerializeField]
    GameObject keywordPF10;
    [SerializeField]
    GameObject keywordPF20;
    [SerializeField]
    GameObject keywordPF40;

    [SerializeField]
    GameEvent roundOver; //deletes current cards and keywrods
    [SerializeField]
    GameEvent newRoundStart; //has to deal all new cards and keyword
    [SerializeField]
    GameEvent activateCards; //allows cards to be moved on second players turn
    [SerializeField]
    GameEvent gameOver;

    [SerializeField]
    endGameStats stats;
    bool wasCorrect = false;


    void Awake()
    {
        outColls = Enumerable.Range(1, 8).ToList(); //8
        inColls = new List<int>();
        currentRound = new Vector2(0, 0); // each round will consist of the players changing turn once 
        rnd = new System.Random();
        startGame();
    }
    void startGame()
    {
        // sets up the players, and assigns who goes first randomly 
        int x = rnd.Next(0, players.Length);
        currentPlayer = players[x];
        currentPlayer.setTurn(true);
        foreach (var player in players)
        {
            if (player != players[x])
            {
                player.setTurn(false);
            }
            player.changeOutline(); // gives the outline of what object they should be holding 
            player.setPromptText();
        }
        // collects the cards and keywords from the database
        

        string conn = "URI=file:" + Application.dataPath + "/testDB.db";
        dbConnect = (IDbConnection)new SqliteConnection(conn);
        dbConnect.Open();
        availableCards = new List<List<int>>();
        initCards();


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
        newRound();
        currentRound = new Vector2(0, 0);
    }
    void clearObjects()
    {
        availableCards.Clear();

        foreach (var x in players)
        {
            x.score = 0;
        }
    }



    //this function is called from the simplboard class
    //creates card objects as the game progress
    public GameObject dealCard(bool parent)
    {
        int card;
        GameObject dealtCard;
        if (parent)
        {
            dealtCard = Instantiate(parentCrd);
        }
        else dealtCard = Instantiate(regCard);


        int coll = rnd.Next(0, availableCards.Count); //pick a collection at random            
        // get the actual id of that collection 
        if(outColls.Contains(coll + 1) && availableCards[coll].Count > 0)
            card = rnd.Next(0, availableCards[coll].Count);  //pick a card at  random from that collection  
        else
        {
            while (!outColls.Contains(coll + 1)|| availableCards[coll].Count == 0)
            {
                coll = rnd.Next(0, availableCards.Count);
            }
            card = rnd.Next(0, availableCards[coll].Count);
        }

       
        outColls.Remove(coll+1);                  //coll has been used this round and is no longer in the out list for keywords
        inColls.Add(coll+1);                      // coll has been used this round so add to in list for keywords 

        dealtCard.GetComponent<cardID>().coll_id = coll+1;
        dealtCard.GetComponent<cardID>().setImageName(collNames[coll], availableCards[coll][card]);

        dealtCard.GetComponent<cardID>().setImage();


        availableCards[coll].RemoveAt(card);      //card is no longer available from that collection
        return dealtCard;
    }

    public void changeTurn()
    {
        if (currentRound.y == 0)
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
            foreach (var x in players)
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
            if (!x.turn)
                x.score += 1;
        }


        //Clears the board and increments round 
        Invoke("finishRound", .15f);
    }
    public void newRound()
    {
        outColls = Enumerable.Range(1, 8).ToList();//8
        inColls.Clear();
        if (currentRound.x < MaxRound)
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

    /*
         SELECT * FROM keywords_current WHERE coll_id=2;
    SELECT NAME FROM keywords_current WHERE coll_id=2;
    SELECT NAME FROM keywords_current WHERE coll_id=2 AND rare=0; 
    SELECT name FROM "cards" WHERE coll_id=8;
SELECT name FROM "cards" WHERE coll_id=8 AND NOT id=58;      
         */
    //change to return choosen a string
    public GameObject getKeywords(int row, bool col)
    {
        List<string> words = new List<string>();
        string word;
        IDbCommand dbcmd = dbConnect.CreateCommand();
        string query;
        int rare = 0;
        int coll = -1;
        GameObject keyword;

        //SAVE WHETHER OR NOT IT IS RARE !!!!!!!!!!!!!!!!!!!!!!!!!
        //rare not a collection on board 40 pts 
        if (row==0)
        {
            coll = rnd.Next(0, outColls.Count);//picks a keyword from a card collection not on the board need
            coll = outColls[coll];
            outColls.Remove(coll);
            rare = 1;
            keyword= Instantiate(keywordPF40);
        }

        
        else if (row==1)
        {
            
            if(col)//rare a collecrtion on the board 
            {
                coll = rnd.Next(0, inColls.Count);//picks a keyword from a card collection  on the board need
                coll = inColls[coll];
                inColls.Remove(coll);
                rare = 1;
            }
            else //obvious other collection 
            {
                coll = rnd.Next(0, outColls.Count);//picks a keyword from a card collection not on the board need
                coll = outColls[coll];
                outColls.Remove(coll);
                rare = 0;
            }
            keyword = Instantiate(keywordPF20);
        }
        else 
        {
            coll = rnd.Next(0, inColls.Count);//picks a keyword from a card collection  on the board need
            coll = inColls[coll];
            inColls.Remove(coll);
            rare = 0;
            keyword = Instantiate(keywordPF10);
        }
        

        query= "SELECT NAME FROM keywords_current WHERE coll_id="+coll.ToString()+" AND rare="+rare.ToString();
        dbcmd.CommandText = query;
        try
        {
            IDataReader rd = dbcmd.ExecuteReader();
            while (rd.Read())
            {
                words.Add(rd.GetString(0));
            }
            rd.Close();
            rd = null;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }
        int x = rnd.Next(0, words.Count);
        word = words[x];

        keyword.GetComponentInChildren<TextMeshProUGUI>().text = word;
        keyword.GetComponent<keywordPts>().rare = rare;
        return keyword;
    }

    //pulls all cards from the database 
    void initCards()
    {
        IDbCommand dbcmd = dbConnect.CreateCommand();

        availableCards.Clear();
        for(int i=0; i< collNames.Count; i++) //8 new lists one for each collection 
        {
            availableCards.Add(new List<int>());
        }

        for (int i=0; i <collNames.Count;i++)
        {
            int id = i + 1;
      
            string query = "SELECT name FROM cards WHERE coll_id=" + id.ToString();
            dbcmd.CommandText = query;
            try
            {
                
                IDataReader rd = dbcmd.ExecuteReader();
                while (rd.Read())
                {
                    string temp = rd.GetString(0);
                    int val=0;
                    bool worked = int.TryParse(temp, out val);
                    if (worked)
                    {
                        availableCards[i].Add(val);
                    }
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
    }

    public void closeDataBase()
    {
        dbConnect.Close();
    }
    public void activateCardPopup()
    {
        popupPanel.SetActive(true);
        popupPanel.transform.SetAsLastSibling();
    }
}
