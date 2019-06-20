using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class castVote : MonoBehaviour, IPointerClickHandler
{
    // check whether the keyword they selected is what they played last round. 
    public int ownedBy;
    public int votes=0;
    public GameObject voteHolder;
    public void OnPointerClick(PointerEventData eventData)
    {
        connectPlayer player = FindObjectOfType<connectPlayer>();
        castTheVote((int)ConnectGM.names.Player, player._voteIcon);
    }

    public bool castTheVote(int voterID, GameObject voteIcon)
    {
        if(voterID!=ownedBy)
        {
            
            voteIcon.transform.SetParent(voteHolder.transform);
            votes = voteHolder.transform.childCount;
            return true;
        }

        return false;
    }
    
}
