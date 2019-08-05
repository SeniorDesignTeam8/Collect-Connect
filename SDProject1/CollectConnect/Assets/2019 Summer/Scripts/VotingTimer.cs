using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class VotingTimer : MonoBehaviour
{
    public float maxTime = 30;
    DateTime begin;
    [SerializeField] GameEvent startMode1;
    [SerializeField]Slider timer;
    // Start is called before the first frame update
    bool countDown;
    private void OnEnable()
    {
        countDown = true;
        begin = DateTime.Now;
        timer.value = maxTime;

    }
    public void endTimer()
    {
        // startMode1.Raise();
        FindObjectOfType<ConnectGM>().startRound();
    }
    private void Update()
    {
        if (countDown)
        {
            TimeSpan delta = DateTime.Now - begin;
            double timePassed = delta.TotalSeconds;

            if (timePassed <= maxTime)
            {
                timer.value = (maxTime - (float)timePassed);
            }

            else
            {
                countDown = false;
                endTimer();
            }
        }

        
    }
}
