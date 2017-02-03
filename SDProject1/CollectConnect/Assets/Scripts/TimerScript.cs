using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public static int Timeleft;
    private BoardManager bM;
    Text t;

    // Use this for initialization
    void Start()
    {
        t = GetComponent<Text>();
        bM = FindObjectOfType<BoardManager>();
        InvokeRepeating("decreaseTime", 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Timeleft < 0)
        {
            bM.PassBtnHit();
        }

        t.text = "" + Timeleft;
    }

    void decreaseTime()
    {
        Timeleft--;
    }
}
