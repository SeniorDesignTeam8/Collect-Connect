using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public string name;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<cardID>().setImageName(name, 1);
        GetComponent<cardID>().setImage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
