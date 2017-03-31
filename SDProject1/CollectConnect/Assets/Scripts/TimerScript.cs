using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TimerScript : MonoBehaviour
{
    public static int Timeleft;
    private BoardManager bM;
    private Button rButton;
    private bool isPaused = false;
    int lastButNum = -1;
    float lastClickTime = -99;
    const float D_CLICK_DELAY = 0.25f;
    Text t;

    // Use this for initialization
    private void Start()
    {
        t = GetComponent<Text>();
        rButton = GetComponent<Button>();
        bM = FindObjectOfType<BoardManager>();
        InvokeRepeating("decreaseTime", 1, 1);
        rButton.onClick.AddListener(resetTimer);
    }

    // Update is called once per frame
    private void Update()
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
        if (Input.GetKeyUp(KeyCode.Space))
            InvokeRepeating("decreaseTime", 1, 1);

        t.text = "" + Timeleft;
    }

    void decreaseTime()
    {
        Timeleft--;
    }

    void resetTimer()
    {
        if (lastClickTime > Time.time - D_CLICK_DELAY && lastButNum == 2)
        {
            CancelInvoke();
            Timeleft = 90;
            InvokeRepeating("decreaseTime", 1, 1);
        }
        else if (lastClickTime > Time.time - D_CLICK_DELAY && lastButNum == 1)
        {
            lastClickTime = Time.time;
            lastButNum = 2;
        }
        else
        {
            lastClickTime = Time.time;
            lastButNum = 1;
        }
    }
}
