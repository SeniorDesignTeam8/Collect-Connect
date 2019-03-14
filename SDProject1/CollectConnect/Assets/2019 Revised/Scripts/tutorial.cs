using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorial : MonoBehaviour
{
    public static bool active=false;
    public int roundsActive=0;
    [SerializeField]
    GameEvent help;
	public void setHelpText()
    {
        active = true;
        help.Raise();
    }
    private void Start()
    {
        roundsActive = 0;
        active = false;
    }

    public void updateTimeLeft()
    {
        roundsActive++;
        if(roundsActive>2)
        {
            active = false;
            roundsActive = 0;
        }
    }


}
