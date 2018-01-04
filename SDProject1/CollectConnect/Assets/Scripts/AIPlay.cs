using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System;
using System.IO;
public class AIPlay : MonoBehaviour {

    private static readonly float[] AiPassThresholds =
{
        0.05f, 0.05f, 0.05f, 0.05f
    };
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayAI(CardCollection PlayerHand)
    {
        //Debug.Log("AI Control: " + name);
        bool alreadyPlayed = false;
        List<int> unplayedCardIndices = new List<int>();
        foreach (Card c in BoardManager.Instance.GetPlayersUnplayedCards())
        {
            if (!c.IsOnBoard() && PlayerHand.IndexOf(c) != -1 && c.GetComponent<Renderer>().enabled)
            {
                unplayedCardIndices.Add(PlayerHand.IndexOf(c));
            }
        }
        if (unplayedCardIndices.Count == 0)
        {
            BoardManager.Instance.PassBtnHit();
            return;
        }
        int randomIndex = Random.Range(0, unplayedCardIndices.Count);
        Card pickedCard = PlayerHand.At(unplayedCardIndices[randomIndex]);
        BoardManager.Instance.SelectCardInHand(pickedCard);
        CardCollection playedCards = BoardManager.Instance.GetPlayedCards();


        float passChance = Random.Range(.45f, 1.0f);
        if (passChance <= AiPassThresholds[BoardManager.Instance.CurrentPlayer])
        {
            //Debug.Log("AI Passed.");
            BoardManager.Instance.PassBtnHit();
        }
        else
        {
            playedCards.Shuffle();
            // More organized way of choosing a random card than just picking a random index.
            float aiValidPlayChance = Random.Range(0.0f, 1.0f);
            foreach (Card c in playedCards)
            {
                //Card c = playedCards.At(0);
                //Debug.Log("trying a card in hand...");
                List<Card.CardProperty> commonProps = c.FindCommonProperties(pickedCard);
                //random index to determine if valid play should happen 80% of the time...
                if (aiValidPlayChance < 0.8)
                {
                    if (commonProps.Count <= 0)
                    {
                        //Debug.Log("no common props");
                        //c = playedCards.At(2);
                        continue;
                    }
                    BoardManager.Instance.SelectCardOnBoard(c);
                    ShufflePropertyList(ref commonProps);
                    BoardManager.Instance.SelectKeyword(commonProps[0]);
                    alreadyPlayed = true;
                    //Debug.Log("AI play valid");
                    break;
                }
                //...otherwise this invalid play should happen
                BoardManager.Instance.SelectCardOnBoard(c);
                BoardManager.Instance.SelectKeyword(c.PropertyList.First());
                alreadyPlayed = true;
                //Debug.Log("AI play invalid");
                break;
            }
            if (!alreadyPlayed && playedCards.Size > 0)
            {
                //...otherwise this invalid play should happen
                int randomindex = Random.Range(0, playedCards.Size);
                BoardManager.Instance.SelectCardOnBoard(playedCards.At(randomindex));
                BoardManager.Instance.SelectKeyword(playedCards.At(randomindex).PropertyList.First());
                //Debug.Log("AI play invalid after");
                //break;
            }

        }
    }


    private static void ShufflePropertyList(ref List<Card.CardProperty> propList)
    {
        for (int i = 0; i < propList.Count; i++)
        {
            Card.CardProperty temp = propList[i];
            int randIndex = Random.Range(0, propList.Count);
            propList[i] = propList[randIndex];
            propList[randIndex] = temp;
        }
    }

}
