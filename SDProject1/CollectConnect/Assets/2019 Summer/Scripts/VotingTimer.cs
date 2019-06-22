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

    private void OnEnable()
    {
        begin = DateTime.Now;
        timer.value = maxTime;
    }
    public void endTimer()
    {
        startMode1.Raise();
    }
    private void Update()
    {

        TimeSpan delta = DateTime.Now - begin;
        double multiplier = delta.TotalSeconds;
        Debug.Log(multiplier);
        if (multiplier <= maxTime)
        {
            timer.value = (maxTime - (float)multiplier);
        }

        else endTimer();
    }
}
