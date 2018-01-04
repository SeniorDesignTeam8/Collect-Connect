using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour {

	public void SwitchCard()
    {
		
        BoardManager instance = BoardManager.Instance;
        int playerNum = instance.CurrentPlayer;
        Player player = instance._playerScriptRefs[playerNum];

		if (player.SwitchesLeft > 0) {
			int cardIndex = instance.GetCurrentPlayer ().GetHand ().IndexOf (player.Card1); //Find the index of the current card played
     	
			CardCollection deck = BoardManager.Deck;
			//Card temp = deck.Draw();

			//Debug.Log("Deck Card:  " + temp.ToString() + "  Hand Card " + player.Card1.ToString());

			//deck.AddSingleCard(player.Card1);

//        BoardManager.Instance.GetCurrentPlayer().GetHand()._cardList[cardIndex] = temp;

			//      Debug.Log("After Swap ->" + "Deck Card:  " + temp.ToString() + "  Hand Card: " + player.Card1.ToString());
			instance.GetCurrentPlayer ().HandSize++; //makes a spot in the players hand
			//GetCurrentPlayer ().GetHand ().RemoveAt (cIndex);
			instance.GetCurrentPlayer ().GetComponent<Player> ()._slotStatus [cardIndex] = false; //registers an open spot in the hand
			instance.GetCurrentPlayer ().RedealCards (); //adds the new card
			//instance.SwapCardPos (cardIndex); //swaps the cards so the slot match up

			instance.InHandGlowOff (player.Card1);

			Card temp = new Card ();
			temp = instance.GetCurrentPlayer ().GetHand ()._cardList [cardIndex]; //Temp equals the card played
			instance.GetCurrentPlayer ().GetHand ()._cardList [cardIndex] = instance.GetCurrentPlayer ().GetHand ()._cardList [instance.GetCurrentPlayer ().GetHand ()._cardList.Count - 1]; //replace the played card with the drawn card
			instance.GetCurrentPlayer ().GetHand ()._cardList [instance.GetCurrentPlayer ().GetHand ()._cardList.Count - 1] = temp; //the last card in the deck is the played card

			player.Card1.SetIsOnBoard (true);
			player.Card1.SetIsSelected (false);
			player.Card1.transform.position = new Vector3 (20, 5, 0);

            player.RemoveToken();
			player.SwitchesLeft--;
            instance.sound.PlayExpand();
		}

		else 
		{
            instance.sound.ErrorSound();
			// add error message
		}
    }
}
