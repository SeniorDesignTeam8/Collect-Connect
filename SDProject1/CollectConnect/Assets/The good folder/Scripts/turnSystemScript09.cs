using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnSystemScript09 : MonoBehaviour {

    public List<TurnClass09> playersGroup;

	// Use this for initialization
	void Start () {
        ResetTurns();
	}
	
	// Update is called once per frame
	void Update () {
        UpdateTurns();
	}

    // resets the player turn to player 1
    void ResetTurns()
    {
        for (int i = 0; i < playersGroup.Count; i++)
        {
            if (i == 0)
            {
                playersGroup[i].isTurn = true;
                playersGroup[i].wasTurnPrev = false;
            }
            else
            {
                playersGroup[i].isTurn = false;
                playersGroup[i].wasTurnPrev = false;
            }
        }
    }

    // Switches the current player's turn to next player
    void UpdateTurns()
    {
        for (int i = 0; i < playersGroup.Count; i++)
        {
            if (!playersGroup[i].wasTurnPrev)
            {
                playersGroup[i].isTurn = true;
                break;
            }
            else if (i == playersGroup.Count - 1 && playersGroup[i].wasTurnPrev)
            {
                ResetTurns();
            }
        }
    }
}

[System.Serializable]
public class TurnClass09
{
    public GameObject playerGameObject;
    public bool isTurn = false;
    public bool wasTurnPrev = false;
}
