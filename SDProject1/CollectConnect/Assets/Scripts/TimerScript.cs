using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public static int Timeleft;
    private BoardManager bM;
    private Button rButton;
    private int numOfClicks = 0;
    Text t;

    // Use this for initialization
    void Start()
    {
        t = GetComponent<Text>();
        rButton = GetComponent<Button>();
        bM = FindObjectOfType<BoardManager>();
        InvokeRepeating("decreaseTime", 1, 1);
        rButton.onClick.AddListener(resetTimer);
    }

    // Update is called once per frame
    void Update()
    {
        if (Timeleft < 15)
        {
            t.color = Color.red;
        }
        else
        {
            t.color = Color.black;
        }

        if (Timeleft < 0)
        {
            bM.PassBtnHit();
        }

        if(Input.GetKeyDown(KeyCode.Space))
            CancelInvoke();
        if(Input.GetKeyUp(KeyCode.Space))
            InvokeRepeating("decreaseTime", 1, 1);

        t.text = "" + Timeleft;
    }

    void decreaseTime()
    {
        Timeleft--;
    }

    void resetTimer()
    {
        numOfClicks++;
        if (numOfClicks%3 == 0)
            Timeleft = 90;
    }
}
