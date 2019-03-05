using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class glow : MonoBehaviour

{
    Color color1;
    Color color2 = new Color(255, 255,255,0);
    public float speed= 2.5f;
    Color lerpedColor;
    public Image background;


    private void Start()
    {
        background = GetComponent<Image>();
        color1 = background.color;
    }
    // Update is called once per frame
    void Update()
    {

        lerpedColor = Color.Lerp(color2, color1, Mathf.PingPong(Time.time*speed, 1));
        background.color = lerpedColor;
    }
}