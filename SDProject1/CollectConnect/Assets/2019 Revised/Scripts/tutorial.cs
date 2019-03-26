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
        roundsActive = 0;
        active = true;
        help.Raise();
    }
    private void Awake()
    {
        roundsActive = 0;
        active = true;
        help.Raise();
    }

    public void updateTimeLeft()
    {
        roundsActive++;
        if(roundsActive>0)
        {
            active = false;
            roundsActive = 0;
        }
    }


}
