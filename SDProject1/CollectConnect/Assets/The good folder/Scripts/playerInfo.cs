using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerInfo : MonoBehaviour {

    public Text score;
    public bool turn = false;
    public int points=0;
    public bool penalized;
    CardManager GM;


	// Use this for initialization
	void Start ()
    {
        GM = GameObject.Find("mainCanvas").GetComponent<CardManager>();
        for(int i=0; i<4; i++)
        {
            Invoke("getCards", .2f);
        }
        Invoke("setLookActive", 1);

    }
    public void setLookActive()
    {
        if (turn)
        {
            Component[] images;
            for (int i = 0; i < transform.childCount; i++)
            {
                images = transform.GetChild(i).gameObject.GetComponentsInChildren<Image>();
                foreach (Image x in images)
                {
                    if (x.tag == "No")
                    {
                        x.enabled = false;
                    }
                }
            }
        }
    }

    public void updateScore()
    {
        score.text = points.ToString();
    }
	void getCards()
    {
        GM.dealCards(gameObject);
    }
	
    public void greyOutCards()
    {
        Component[] images;
        for (int i =0; i<transform.childCount;i++)
        {
            images = transform.GetChild(i).gameObject.GetComponentsInChildren<Image>();
            foreach(Image x in images)
            {
                if(x.tag=="No")
                {
                    x.enabled = true;
                }
            }
        }
        
      
        
        
    }
	void Update ()
    {
		
	}
}
