using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showSelectedWord : MonoBehaviour
{
    [SerializeField]Image glow;
   // Color filled = new Color(255, 201, 0, 255);
   // Color clear= new Color(255,201,0,0);
    public float speed = 1.5f;
    Color lerpedColor;
    GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSelection.selected==parent)
        {
            lerpedColor = Color.Lerp(Color.clear, Color.yellow, Mathf.PingPong(Time.time * speed, 1));
            glow.color = lerpedColor;
        }
        else { glow.color = Color.clear; }
        //make the color completely transparent
        //else if not null set this to being a 
    }
}
