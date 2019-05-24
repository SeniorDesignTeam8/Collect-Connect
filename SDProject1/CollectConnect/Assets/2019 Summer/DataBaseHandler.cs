using Mono.Data.Sqlite;
using System;

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
public static class DataBaseHandler 
{
    //static IDbCommand dbcmd;
    static string conn;
    static IDbConnection dbConnect;
    
    static void setUp()
    {
       // conn = "URI=file:" + Application.dataPath + "/testDB.db";
        

        if (Application.platform != RuntimePlatform.Android)
        {

            conn = "URI=file:" + Application.dataPath + "/testDB.db";
            dbConnect = new SqliteConnection(conn);
        }
        else
        {

            conn =  Application.persistentDataPath + "/" +"testDB.db";
            if (!File.Exists(conn))
            {
                UnityWebRequest load = new UnityWebRequest("jar:file://" + Application.dataPath + "!/assets/" + "testDB.db");
                load.downloadHandler = new DownloadHandlerBuffer();
                load.SendWebRequest();
                if(load.isNetworkError || load.isHttpError)
                {
                    Debug.Log(load.error);
                }
                else
                {
                    Debug.Log("Trying to get database bytes /n");
                    byte[] results = load.downloadHandler.data;
                    File.WriteAllBytes(conn, results);
                    dbConnect = new SqliteConnection(conn);
                    if (dbConnect != null)
                        Debug.Log("Success");
                    else Debug.Log("Failed at writing bytes to database");
                }
                
                // while (!load.isDone) { }

                
            }
        }

        dbConnect = new SqliteConnection(conn);
    }
     
   

    public static void closeDataBase()
    {
        dbConnect.Close();
    }

    //get random keyword based on rareity and collection 
     public static string getKeywordByColl(int coll, int rare)
    {
        setUp();
        dbConnect.Open();
        IDbCommand dbcmd = dbConnect.CreateCommand(); 
        string query;
        List<string> words = new List<string>();
        query = "SELECT NAME FROM keywords_current WHERE coll_id=" + coll.ToString() + " AND rare=" + rare.ToString();
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

        dbConnect.Close();
        System.Random rnd = new System.Random();
        int i = rnd.Next(0, words.Count);

        
        return words[i];

    }
    public static List<List<int>> getAllCards()
    {
        setUp();
        dbConnect.Open();
        List<List<int>> availableCards = new List<List<int>>();

        IDbCommand dbcmd = dbConnect.CreateCommand();

        availableCards.Clear();
        for (int i = 0; i < GM.collNames.Count; i++) //8 new lists one for each collection 
        {
            availableCards.Add(new List<int>());
        }

        for (int i = 0; i < GM.collNames.Count; i++)
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
                    int val = 0;
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
        dbConnect.Close();
        return availableCards;
    }
}
