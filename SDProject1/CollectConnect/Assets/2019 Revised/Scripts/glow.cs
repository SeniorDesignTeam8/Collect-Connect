using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class glow : MonoBehaviour {

    Color lerpedColor = Color.blue;
    Image background;
    
     
    private void Start()
    {
        background = GetComponent<Image>();
    }
    // Update is called once per frame
    void Update ()
    {
        
        lerpedColor = Color.Lerp(Color.blue, Color.white, Mathf.PingPong(Time.time, 1));
        background.color = lerpedColor;
	}
}
