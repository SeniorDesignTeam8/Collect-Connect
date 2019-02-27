using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.UI;
using TMPro;


public class endGameStats : MonoBehaviour
{
    [SerializeField]
    TextAsset gameStats;
    List<rounds> playThrough;
    int count = 0;

    public GameObject[] roundSlots;


    [SerializeField]
    GameObject correctPF;
    [SerializeField]
    GameObject incorrectPF;

    public static GameObject lastCard;
    public static GameObject lastKeyword;
    [SerializeField]
    public GM gm;


    public struct rounds
    {
        public GameObject [] round;
        public rounds(int i=1)
        {
            round = new GameObject[4];
        }
    }

    public static string path = "Assets/Resources/Records/lastGame.txt";

    private void Awake()
    {
        
        newGame();
        
        //saveStats();
    }

    //correctly posistions holder slots according to screen size. 
    public void setSlotPos()
    {
        
        RectTransform rt;
        RectTransform thisRT = GetComponent<RectTransform>();
        float offset = thisRT.sizeDelta.y/ gm.MaxRound;
        float topOfScreen = thisRT.sizeDelta.y / 2f;
        float halfSize;

        for(int i =0; i<gm.MaxRound; i++)
        {
            rt = roundSlots[i].GetComponent<RectTransform>();
            halfSize = rt.sizeDelta.y / 2;
            Vector3 newPos = rt.anchoredPosition;
            newPos.y = topOfScreen - (offset * i)- halfSize;
            rt.anchoredPosition = newPos;
        }

    }

    public void newGame()
    {
        count = 0;
        playThrough = new List<rounds>();
        for(int i=0; i<gm.MaxRound; i++)
        {
            rounds temp = new rounds(1);
            playThrough.Add(temp);
        }
    }

    public void setParent(GameObject parent)
    {
        playThrough[count].round[0] = parent;
        playThrough[count].round[0].transform.SetParent(transform);
    }

    public void setCard( GameObject card)
    {
        lastCard = card;
        playThrough[count].round[2] = card;
        playThrough[count].round[2].transform.SetParent(transform);
    }

    public void setKeyWord(GameObject keyword)
    {
        lastKeyword = keyword;
        playThrough[count].round[1] = keyword;
        playThrough[count].round[1].transform.SetParent(transform);
    }

    public void setCorrect(bool correct)
    {
         
        if (correct)
            playThrough[count].round[3] = Instantiate(correctPF, transform);
        else
            playThrough[count].round[3] = Instantiate(incorrectPF,transform);

        for(int i=0; i<4;i++)
        {
            playThrough[count].round[i].SetActive(false); 
        }
        count++;
    }

    // deletes the game objects
    // should be called for play again and going back to main menu 
    public void resetStats()
    {
        for(int i =0; i< playThrough.Count; i++)
        {
            for( int j=0; j<4; j++)
            {
                Destroy(playThrough[i].round[j]);
            }
        }
        playThrough.Clear();
    }

    public void setUpSlots()
    {
        setSlotPos();
        for( int i =0; i < playThrough.Count;i++)
        {
            for(int j=0; j<4; j++)
            {
                if (j % 2 == 0)
                {
                    playThrough[i].round[j].transform.localScale = new Vector3(.5f, .5f, .5f);
                    playThrough[i].round[j].GetComponent<glow>().enabled = false;
                    playThrough[i].round[j].GetComponent<Image>().color = Color.white;
                }
                playThrough[i].round[j].transform.SetParent(roundSlots[i].transform);
                playThrough[i].round[j].SetActive(true);
            }
        }
        writeStats();
    }

    // this is where you would save to a file the parent card info, keyword, chosen card, correctness 
    private void writeStats()
    {
        StreamWriter writer = new StreamWriter(path);
        
        //clear last game
        writer.WriteLine("");
        writer.Close();

        for (int i = 0; i < playThrough.Count; i++)
        {
            //save parent card info in file
            CardInfo card = playThrough[i].round[0].GetComponent<CardInfo>();
            File.AppendAllText(path, card.completeDes + "***" + card.ImageLoc + "***");


            //save keyword
            string word = playThrough[i].round[1].GetComponentInChildren<TextMeshProUGUI>().text;
            File.AppendAllText(path, word + "****");

            //save chosen card
            card = playThrough[i].round[2].GetComponent<CardInfo>();
            File.AppendAllText(path, card.completeDes + "***" + card.ImageLoc + "***");

            if (playThrough[i].round[3].tag == "Yes")
                File.AppendAllText(path, "true \n\n");
            else
                File.AppendAllText(path, "false \n\n");
        }


        AssetDatabase.ImportAsset(path);
        

        //Print the text from the file
        Debug.Log(gameStats.text);

    }



}
