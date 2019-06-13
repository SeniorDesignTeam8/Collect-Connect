using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class returnFirstChild : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void sendBack()
    {
        GameObject child1 = transform.GetChild(0).gameObject;
        Transform back = child1.GetComponent<DragItems>().spawn;
        child1.transform.localScale = child1.GetComponent<DragItems>().startScale;
        child1.transform.SetParent(back);
    }
}
