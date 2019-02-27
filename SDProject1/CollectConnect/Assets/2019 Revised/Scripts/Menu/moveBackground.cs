using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class moveBackground : MonoBehaviour
{
    public RectTransform canvas;
    RectTransform rt;
    Vector3 startPos;
    public float fallSpeed=5;
    //if pos. y <screen 
	// Use this for initialization
	void Start ()
    {
		rt= GetComponent<RectTransform>();
        startPos= rt.anchoredPosition; 
    }
	void test()
    {
        float bottomOfScreen =0- canvas.sizeDelta.y/ 2f;
        float leftOfScreen= 0 - canvas.sizeDelta.x / 2f;
        Vector3 newPos = rt.anchoredPosition;
        newPos.y -= fallSpeed;
        newPos.x -= fallSpeed;
        if(newPos.y<bottomOfScreen ||newPos.x<leftOfScreen)
        {
            newPos = startPos;
        }
        rt.anchoredPosition = newPos;
    }
	// Update is called once per frame
	void Update ()
    {
        test();
	}
}
