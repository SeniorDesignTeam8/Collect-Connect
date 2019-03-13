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

    List<int> parentCardName;
    List<int> parentCardColl;

    List<int> smallCardNames;
    List<int> smallCardColl;

    List<string> keywords;
    List<bool> correct;
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

        keywords = new List<string>();
        correct = new List<bool>();
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
        keywords.Add(keyword.GetComponent<TextMeshProUGUI>().text);
        rare.Add(keyword.GetComponent<keywordPts>().rare); 
    }

    public void setCorrect(bool correct)
    {

        this.correct.Add(correct);
    }


    public void finalScore()
    {
        pscore1.text = gm.players[0].score.ToString();
        pscore2.text = gm.players[1].score.ToString();
        //saveToDatabase();

    }


    // this is where you would save the parent card info, keyword, chosen card, correctness 
    private void saveToDatabase()
    {
        //close off the game manager from the database so it can be opened here 
        gm.closeDataBase();

        IDbConnection dbConnect;
        string conn = "URI=file:" + Application.dataPath + "/testDB.db";
        dbConnect = (IDbConnection)new SqliteConnection(conn);
        dbConnect.Open();


        //INSERT INTO "connections" ("id","user_id","user2_id", "card1_id","card2_id","keyword_id","keyword_match","keyword_match_rare","keyword_in_coll","time") VALUES ('3','44','32','12','34','67','0','0','0','2019-03-01 0:00:00');

    }



}





















    //correctly posistions holder slots according to screen size. 
    //public void setSlotPos()
    //{
        
    //    RectTransform rt;
    //    RectTransform thisRT = GetComponent<RectTransform>();
    //    float offset = thisRT.sizeDelta.y/ gm.MaxRound;
    //    float topOfScreen = thisRT.sizeDelta.y / 2f;
    //    float halfSize;

    //    for(int i =0; i<gm.MaxRound; i++)
    //    {
    //        rt = roundSlots[i].GetComponent<RectTransform>();
    //        halfSize = rt.sizeDelta.y / 2;
    //        Vector3 newPos = rt.anchoredPosition;
    //        newPos.y = topOfScreen - (offset * i)- halfSize;
    //        rt.anchoredPosition = newPos;
    //    }

    //}