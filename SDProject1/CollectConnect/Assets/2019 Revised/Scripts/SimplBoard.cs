using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        widthX *= 1.5f;
        for (int i = 0; i < numOptions;i++)
        {
            spots[i] = Instantiate(holderPF, new Vector3(i * widthX + offsetX,  offsetY, 0), Quaternion.identity);
            spots[i].transform.SetParent(transform);
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
        parentcard.transform.SetParent(parentCardPos);
        RectTransform rt = parentcard.GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        parentcard.GetComponent<DragItems>().canBeMoved = false;

        int count = 0;
        foreach (var x in spots)
        {
            GameObject card = gm.createCardObject();
            card.transform.SetParent(x.transform);
            card.GetComponent<DragItems>().canBeMoved = false;
            dealtCards[count] = card;
            count++;
        }
    }
    public void deactivate()
    {
        keyWordAcess(false);
    }


	public void cardAcess()
    {
        foreach (var x in dealtCards)
        {
            x.GetComponent<DragItems>().canBeMoved = true;
        }
    }

    void keyWordAcess(bool access)
    {
        foreach (var x in keywords)
        {
            x.GetComponent<DragItems>().canBeMoved = access;
        }
    }
	// Update is called once per frame
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
        keywords.Clear();
        Destroy(parentcard);
    }

    public void newRound()
    {
        dealCards();
        dealKeywords();
        keyWordAcess(true);
    }
}
