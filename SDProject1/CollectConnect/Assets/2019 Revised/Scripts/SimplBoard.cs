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
    GameObject keyBank1;
    [SerializeField]
    GameObject keyBank2;
    [SerializeField]
    GameObject keyBank3;

    [SerializeField]
    endGameStats stats;

    public Button refreshButton;

    GameObject[] dealtCards;
    List<GameObject> keywords;
    GameObject parentcard;
    GM gm;

	void Start ()
    {
        keywords = new List<GameObject>();
        gm = GetComponent<GM>();
        spots = new GameObject[numOptions];
        dealtCards = new GameObject[numOptions];

        createBoard();
        dealCards();
        dealKeywords();
        keyWordAcess(true);
        
	}


    public void createBoard()
    {
        float offsetX = startPos.position.x;
        float offsetY = startPos.position.y;
        holderPF.transform.localScale = new Vector3(1, 1, 1);

        float widthX = holderPF.GetComponent<RectTransform>().sizeDelta.x;
        widthX +=widthX/2;
        for (int i = 0; i < numOptions;i++)
        {
            spots[i] = Instantiate(holderPF, new Vector3(i * widthX + offsetX,  offsetY, 0), Quaternion.identity);
            spots[i].transform.SetParent(transform);
         //   spots[i].GetComponent<glow>().enabled = false;
        }

        // make board and place holders equal distance aprt 

    }
    public void dealKeywords()
    {
        int row=0;
        bool col = true;
        for(int i =0; i<6;i++)
        {
            if (i < 2) row = 0;
            else if (i == 2) { row = 1; }
            else if (i == 3) { row = 1; col = false; }
            else if (i > 3) row = 2;

            keywords.Add(gm.getKeywords(row, col));
            if (i < 2) keywords[i].transform.SetParent(keyBank1.transform);
            else if (i ==2||i==3) keywords[i].transform.SetParent(keyBank2.transform);
            else if (i > 3) keywords[i].transform.SetParent(keyBank3.transform);

            keywords[i].transform.localScale = new Vector3(.75f, .75f, .75f);

        }
        keyWordAcess(true);
    }
    public void dealCards()
    {
        refreshButton.interactable = true;
        parentcard= gm.dealCard(true);
        parentcard.transform.SetParent(parentCardPos);
        RectTransform rt = parentcard.GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        parentcard.transform.localScale = new Vector3(.90f, .90f, .90f);
        stats.setParent(parentcard);

        int count = 0;
        foreach (var x in spots)
        {
            x.transform.localScale = new Vector3(1, 1, 1);
            GameObject card = gm.dealCard(false);
            card.transform.SetParent(x.transform);
            card.GetComponent<DragItems>().canBeMoved = false;
            card.transform.localScale = new Vector3(.85f, .85f, .85f);
          //  x.GetComponent<Image>().color = Color.white;
           dealtCards[count] = card;
            count++;
        }
       // stopCardGlow();
    }

    public void stopCardGlow()
    {
        foreach (var x in dealtCards)
        {
            if (x.GetComponent<glow>() != null)
            {
                x.GetComponent<glow>().enabled = false;
                x.GetComponent<Image>().color = Color.white;
            }
        }
    }
    public void deactivate()
    {
        keyWordAcess(false);
        foreach (var x in dealtCards)
        {
            x.GetComponent<DragItems>().canBeMoved = false;
        }

    }


	public void cardAcess()
    {
        refreshButton.interactable = false;
        foreach (var x in dealtCards)
        {
            x.GetComponent<DragItems>().canBeMoved = true;
           // x.GetComponent<glow>().enabled = true;
        }
    }

    void keyWordAcess(bool access)
    {
        foreach (var x in keywords)
        {
            x.GetComponent<DragItems>().canBeMoved = access;
            if(x.GetComponentInChildren<glow>()!=null)
            { 
            if (x.GetComponentInChildren<glow>().enabled)
                x.GetComponentInChildren<glow>().background.color = new Color(0, 0, 0, 0);

            x.GetComponentInChildren<glow>().enabled = access;
           }
        }

    }

    public void refreshKeyBank()
    {
        foreach (var x in keywords)
        {
            Destroy(x);
        }
        keywords.Clear();
        Invoke("dealKeywords", .1f);
       

    }


    public void roundOver()
    {
        foreach(var x in dealtCards)
        {
                Destroy(x);
        }
        foreach(var x in keywords)
        {
                Destroy(x);
        }
        Destroy(parentcard);
        keywords.Clear();

    }

    public void newRound()
    {
        dealCards();
        dealKeywords();
        keyWordAcess(true);
    }
}
