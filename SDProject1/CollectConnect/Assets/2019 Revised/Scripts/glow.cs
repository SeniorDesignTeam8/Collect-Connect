using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class glow : MonoBehaviour

//{
//    public Color inital;
//    public Color lerpedColor = Color.blue;
//    Image background;


//    private void Start()
//    {
//        background = GetComponent<Image>();
//    }
//    // Update is called once per frame
//    void Update()
//    {

//        lerpedColor = Color.Lerp(Color.blue, Color.white, Mathf.PingPong(Time.time, 1));
//        background.color = lerpedColor;
//    }
//}
{
    public Color color1 = Color.blue;
    public Color color2 = new Color(255, 164, 0);
    public float speed= 2.5f;
    Color lerpedColor;
    public Image background;


    private void Start()
    {
        background = GetComponent<Image>();
    }
    // Update is called once per frame
    void Update()
    {

        lerpedColor = Color.Lerp(color2, color1, Mathf.PingPong(Time.time*speed, 1));
        background.color = lerpedColor;
    }
}