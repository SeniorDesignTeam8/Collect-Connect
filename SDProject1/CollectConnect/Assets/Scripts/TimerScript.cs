using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public static int Timeleft;
    private BoardManager bM;
    private Text t;

    // Use this for initialization
    private void Start()
    {
        t = GetComponent<Text>();
        bM = FindObjectOfType<BoardManager>();
        InvokeRepeating("DecreaseTime", 1, 1);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Timeleft < 0)
        {
            bM.PassBtnHit();
        }

        t.text = "" + Timeleft;
    }

    private void DecreaseTime()
    {
        Timeleft--;
    }
}
