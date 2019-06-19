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
    public int votes;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(ownedBy!=3)
        {
            castTheVote();
        }
    }

    public void castTheVote()
    {

    }
}
