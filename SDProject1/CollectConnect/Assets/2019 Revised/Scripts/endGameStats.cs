using Mono.Data.Sqlite;
using System;

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class endGameStats : MonoBehaviour
{
    public static int startCon=0;

    List<int> parentCardName;
    List<int> parentCardColl;

    List<int> smallCardNames;
    List<int> smallCardColl;

    List<int> keywords;
    List<int> correct;
    List<int> rare;

    [SerializeField]
    public GM gm;


    [SerializeField]
    TextMeshProUGUI pscore1;
    [SerializeField]
    TextMeshProUGUI pscore2;



    public static string path = "Assets/Resources/Records/lastGame.txt";

    private void Awake()
    {
        parentCardName= new List<int>();
        parentCardColl= new List<int>();

        smallCardNames= new List<int>();
        smallCardColl= new List<int>();

        keywords = new List<int>();
        correct = new List<int>();
        rare = new List<int>();

    }

    public void setParent(GameObject parent)
    {
        parentCardName.Add(parent.GetComponent<cardID>().cardName);
        parentCardColl.Add(parent.GetComponent<cardID>().coll_id);
    }

    public void setCard( GameObject card)
    {
        smallCardNames.Add(card.GetComponent<cardID>().cardName);
        smallCardColl.Add(card.GetComponent<cardID>().coll_id);
    }

    public void setKeyWord(GameObject keyword)
    {
        keywords.Add(keyword.GetComponent<keywordPts>().keyID);
        rare.Add(keyword.GetComponent<keywordPts>().rare); 
    }

    public void setCorrect(bool correct)
    {
        if (correct)
            this.correct.Add(1);
        else this.correct.Add(0);
    }


    public void finalScore()
    {
        pscore1.text = gm.players[0].score.ToString();
        pscore2.text = gm.players[1].score.ToString();
        saveToDatabase();

    }


    // this is where you would save the parent card info, keyword, chosen card, correctness 
    private void saveToDatabase()
    {
        int startID = 0;
        //close off the game manager from the database so it can be opened here 
        gm.closeDataBase();

        IDbConnection dbConnect;
        string conn = "URI=file:" + Application.dataPath + "/testDB.db";
        dbConnect = (IDbConnection)new SqliteConnection(conn);
        dbConnect.Open();
        IDbCommand dbcmd = dbConnect.CreateCommand();
        

        string query = "SELECT id FROM connections";
        dbcmd.CommandText = query;

        //get the last id from the connections table 
        try
        {

            IDataReader rd = dbcmd.ExecuteReader();
            while (rd.Read())
            {              
                startID=rd.GetInt16(0);
            }
            rd.Close();
            rd = null;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }
        startID++;// increment for new entry 
        startCon = startID;

        IDbTransaction transaction = dbConnect.BeginTransaction();
        // for each round of play insert that into the database 
        for (int i=0; i<gm.MaxRound;i++)
        {
            
            int parentID = -1;
            int cardID = -1;
            int keywordID =-1;
            try
            {

                string queryX = "SELECT id FROM cards WHERE coll_id=" + parentCardColl[i] + " AND name=" + parentCardName[i];
                dbcmd.CommandText = queryX;
                IDataReader rd = dbcmd.ExecuteReader();
                while(rd.Read())
                    parentID = rd.GetInt16(0);
                rd.Close();
                rd = null;

                queryX = "SELECT id FROM cards WHERE coll_id=" + smallCardColl[i] + " AND name=" + smallCardNames[i];
                dbcmd.CommandText = queryX;
                rd = dbcmd.ExecuteReader();
                while (rd.Read())
                    cardID = rd.GetInt16(0);
                rd.Close();
                rd = null;


                keywordID = keywords[i];


                
                dbcmd.Transaction = transaction;
                queryX = "INSERT INTO connections (id, user_id, user2_id, card1_id, card2_id, keyword_id, keyword_match, keyword_match_rare, keyword_in_coll,time)" +
                " VALUES ("+startID+", 101, 102, "+parentID+ ", " + cardID + ", " + keywordID + ", " + correct[i] + ", " + rare[i] + ", 0, '2019-03-01 0:00:00')";
                dbcmd.CommandText=queryX;
                dbcmd.ExecuteNonQuery();
                Debug.Log("Conn: " + i.ToString() + " succesfull");
                if(i==gm.MaxRound-1)
                  transaction.Commit();
                startID++;
                

            }
            catch (Exception ex)
            {
                Debug.Log("Commit Failed");
                var x = ex.GetType();
                // Attempt to roll back the transaction.
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    // This catch block will handle any errors that may have occurred
                    // on the server that would cause the rollback to fail, such as
                    // a closed connection.
                    Debug.Log("Rollback Exception Type: {2}");
                    Debug.Log("  Message: {2}");
                }
            }
           

        }

        //INSERT INTO connections ("id","user_id","user2_id", "card1_id","card2_id","keyword_id","keyword_match","keyword_match_rare","keyword_in_coll","time") VALUES ('3','101','102','12','34','67','0','0','0','2019-03-01 0:00:00');

    }



}
