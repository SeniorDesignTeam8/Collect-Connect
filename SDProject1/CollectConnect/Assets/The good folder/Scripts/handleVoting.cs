using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class handleVoting : MonoBehaviour
{
    public Transform locCard1;
    public Transform locCard2;
    public Transform locWord;
    RectTransform rt;
    public Canvas votingPrefab;
    Canvas voting;
    GameObject card1;
    GameObject card2;
    GameObject keyword;
	
    public void activateVoting()
    {
        Board board = GameObject.Find("mainCanvas").GetComponent<Board>();
        card1 = Instantiate(board.lastCardPlayed);
        card2 = Instantiate(board.lastCardConnected);
        keyword = Instantiate(board.usedConnection);
        voting = Instantiate(votingPrefab);
        
        
        voting.transform.SetParent(GameObject.Find("mainCanvas").transform);
        rt = voting.GetComponent<RectTransform>();
        rt.anchorMax = new Vector2(.5f, .5f);
        rt.anchorMin = new Vector2(.5f, .5f);
        rt.localPosition = new Vector3(0,0,0);

        keyword.transform.SetParent(voting.transform);
        keyword.transform.localPosition = locWord.localPosition;

        fixCard(card1);
        card1.transform.localPosition = locCard1.localPosition;
        fixCard(card2);
        card2.transform.localPosition = locCard2.localPosition;

    }

    void fixCard(GameObject copy)
    {
        Destroy(copy.GetComponent<InflateCard>());
        Destroy(copy.GetComponent<Dragable>());
        Component[] images = copy.transform.GetComponentsInChildren<Image>();
        Image wordBack = copy.transform.GetComponentInChildren<Image>();
        foreach (Image x in images)
        {
            if (x.tag == "wordBacking")
            {
                wordBack = x;
            }

        }
        Text des = copy.transform.GetComponentInChildren<Text>();
        des.enabled = true;
        wordBack.enabled = true;
        copy.transform.SetParent(voting.transform);
        
    }
	void Awake ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
