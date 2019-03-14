using Mono.Data.Sqlite;
using System;

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class loadLastPlay : MonoBehaviour {
    public GameObject cardPF;
    public GameObject keywordPF;
    public Image right;
    public Image wrong;

    List<int> parents;
    List<int> children;
    List<int> keywords;
    List<int> correct;

    List<cardID> parentCard;
    List<cardID> childCards;
    List<string> words;
    IDbCommand dbcmd;
    IDbConnection dbConnect;

    private void Start()
    {
        string conn = "URI=file:" + Application.dataPath + "/testDB.db";
        dbConnect = (IDbConnection)new SqliteConnection(conn);
        dbConnect.Open();
        dbcmd = dbConnect.CreateCommand();

        parents= new List<int>();
        children = new List<int>();
        keywords= new List<int>();
        correct=new List<int>();

        parentCard = new List<cardID>();
        childCards = new List<cardID>();
        words = new List<string>();

        getLastGame();
        createKeywords();
        createParentCards();
        creatChildCard();
    }
    private void getLastGame()
    {
        int startID=0;
        string query = "SELECT id FROM connections";
        dbcmd.CommandText = query;
        IDataReader rd = dbcmd.ExecuteReader();

        //get the last id from the connections table 
        try
        {
            while (rd.Read())
            {
                startID = rd.GetInt16(0);
            }
            rd.Close();
            rd = null;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }

        int idP=-1, idC=-1, idK=-1, idcorrect=-1;
        int count = 0;
        for (int i=startID; i>0;i--)
        {
            query = "SELECT * FROM connections WHERE id=" + i.ToString();
            dbcmd.CommandText = query;
            rd = dbcmd.ExecuteReader();
            if (rd.Read())
            {
                idP = rd.GetInt16(3);
                idC = rd.GetInt16(4);
                idK = rd.GetInt16(5);
                idcorrect =  rd.GetInt16(6) ;
            }
                    
            rd.Close(); //rd = null;

            parents.Add(idP);
            children.Add(idC);
            keywords.Add(idK);
            correct.Add(idcorrect);

   //         Debug.Log(idP.ToString() + " " + idC.ToString() + " " + idK.ToString() + " " + idcorrect.ToString());
            count++;
            if (count > 5) i = 0;
            
        }

    }
    private void createKeywords()
    {
        IDataReader rd;
        string query;
        for(int i=0; i<keywords.Count; i++)
        {
            query = "SELECT name FROM keywords_current WHERE id=" + keywords[i];
            dbcmd.CommandText = query;
            rd= dbcmd.ExecuteReader();
            string temp= rd.Read() ? rd.GetString(0) : "nope >:(";
            rd.Close();
//            Debug.Log(temp);
            words.Add(temp);
        }
 
    }
    private void createParentCards()
    {
        IDataReader rd;
        string query;
        int collid = -1, nameid=-1;

        for (int i = 0; i < parents.Count; i++)
        {
            query = "SELECT * FROM cards WHERE id=" + parents[i];
            dbcmd.CommandText = query;
            rd = dbcmd.ExecuteReader();
            if (rd.Read())
            {
                //0 = id, coll_id=1, name=2
                collid = rd.GetInt16(1);
                bool worked = int.TryParse(rd.GetString(2), out nameid);
            }
            rd.Close();
            cardID card = new cardID();
            card.setImageName(GM.collNames[collid], nameid);
            card.setImage();
            parentCard.Add(card);

        }

    }
    private void creatChildCard()
    {
        IDataReader rd;
        string query;
        int collid = -1, nameid = -1;

        for (int i = 0; i < children.Count; i++)
        {
            query = "SELECT * FROM cards WHERE id=" + children[i];
            dbcmd.CommandText = query;
            rd = dbcmd.ExecuteReader();
            if (rd.Read())
            {
                collid = rd.GetInt16(1);
                bool worked = int.TryParse(rd.GetString(2), out nameid);
            }
            rd.Close();
            cardID card = new cardID();
            card.setImageName(GM.collNames[collid], nameid);
            card.setImage();
            childCards.Add(card);

        }
    }
}
