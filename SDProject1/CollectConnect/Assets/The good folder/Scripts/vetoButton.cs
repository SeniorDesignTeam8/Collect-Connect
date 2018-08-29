using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vetoButton : MonoBehaviour
{
    
    public void vetoClicked()
    {
        CardManager GM = GameObject.Find("mainCanvas").GetComponent<CardManager>();
        playerInfo player = GM.players[GM.turn].GetComponent<playerInfo>();

    }
}
