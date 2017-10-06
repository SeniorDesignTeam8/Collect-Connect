using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour {

	public void SwitchCard()
    {
        BoardManager instance = BoardManager.Instance;
        int playerNum = instance.CurrentPlayer;
        Player player = instance._playerScriptRefs[playerNum];

        int cardIndex = instance.GetCurrentPlayer().GetHand().IndexOf(player.Card1); //Find the index of the current card played
     
   
        CardCollection deck = BoardManager.Deck;
        Card temp = deck.Draw();

        Debug.Log("Deck Card:  " + temp.ToString() + "  Hand Card " + player.Card1.ToString());

        deck.AddSingleCard(player.Card1);

        BoardManager.Instance.GetCurrentPlayer().GetHand()._cardList[cardIndex] = temp;

        Debug.Log("After Swap ->" + "Deck Card:  " + temp.ToString() + "  Hand Card: " + player.Card1.ToString());


    }
}
