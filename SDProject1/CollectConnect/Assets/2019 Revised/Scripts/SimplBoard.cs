using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimplBoard : MonoBehaviour
{

    [SerializeField]
    int numOptions;
    [SerializeField]
    Transform startPos;
    [SerializeField]
    Transform parentCardPos;
    [SerializeField]
    GameObject holderPF;
    GameObject [] spots;
    [SerializeField]
    GameObject keyBank;


    [SerializeField]
    endGameStats stats;

    //List<glow> cardGlow;

    //glow keyBankGlow;

    GameObject[] dealtCards;
    List<GameObject> keywords;
    GameObject parentcard;
    GM gm;
	// Use this for initialization
	void Start ()
    {
        keywords = new List<GameObject>();
        gm = GetComponent<GM>();
        spots = new GameObject[numOptions];
        dealtCards = new GameObject[numOptions];
        //keyBankGlow = keyBank.GetComponentInParent<glow>();
        createBoard();
        dealCards();
        dealKeywords();
        keyWordAcess(true);
        
	}


    public void createBoard()
    {
        float offsetX = startPos.position.x;
        float offsetY = startPos.position.y;
        float widthX = holderPF.GetComponent<RectTransform>().sizeDelta.x;
        widthX +=10;
        for (int i = 0; i < numOptions;i++)
        {
            spots[i] = Instantiate(holderPF, new Vector3(i * widthX + offsetX,  offsetY, 0), Quaternion.identity);
            spots[i].transform.SetParent(transform);
            spots[i].GetComponent<glow>().enabled = false;
        }

        // make board and place holders equal distance aprt 

    }
    public void dealKeywords()
    {
        for(int i =0; i<numOptions+1;i++)
        {
            keywords.Add(gm.getKeywords());
            keywords[i].transform.SetParent(keyBank.transform);
        }
    }
    public void dealCards()
    {
        parentcard= gm.createCardObject();
        parentcard.transform.localScale=new Vector3(1.3f,1.3f,1.3f);
        parentcard.transform.SetParent(parentCardPos);
        RectTransform rt = parentcard.GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        parentcard.GetComponent<DragItems>().canBeMoved = false;

        stats.setParent(parentcard);

        int count = 0;
        foreach (var x in spots)
        {     
            GameObject card = gm.createCardObject();
            card.transform.SetParent(x.transform);
            card.GetComponent<DragItems>().canBeMoved = false;
//            x.GetComponent<glow>().enabled = false;
            x.GetComponent<Image>().color = Color.white;
            dealtCards[count] = card;
            count++;
        }
        stopCardGlow();
    }

    public void stopCardGlow()
    {
        foreach (var x in dealtCards)
        {
            x.GetComponent<glow>().enabled = false;
            x.GetComponent<Image>().color = Color.white;
        }
    }
    public void deactivate()
    {
        keyWordAcess(false);
        foreach (var x in dealtCards)
        {
            x.GetComponent<DragItems>().canBeMoved = false;
        }
        //   keyBankGlow.enabled = false;
        //   keyBank.GetComponentInParent<Image>().color = Color.white;

    }


	public void cardAcess()
    {
        foreach (var x in dealtCards)
        {
            x.GetComponent<DragItems>().canBeMoved = true;
            x.GetComponent<glow>().enabled = true;
        }
    }

    void keyWordAcess(bool access)
    {
        foreach (var x in keywords)
        {
            x.GetComponent<DragItems>().canBeMoved = access;
            if (x.GetComponentInChildren<glow>().enabled)
                x.GetComponentInChildren<glow>().background.color = new Color(0, 0, 0, 0);

            x.GetComponentInChildren<glow>().enabled = access;
           
        }
       // keyBankGlow.enabled = access;
    }
	// Update is called once per frame
    public void roundOver()
    {
        foreach(var x in dealtCards)
        {
            if(x!=endGameStats.lastCard)
                Destroy(x);
        }
        foreach(var x in keywords)
        {
            if(x!=endGameStats.lastKeyword)
                Destroy(x);
        }
        keywords.Clear();

        

        //Destroy(parentcard);
    }

    public void newRound()
    {
        dealCards();
        dealKeywords();
        keyWordAcess(true);
    }
}
