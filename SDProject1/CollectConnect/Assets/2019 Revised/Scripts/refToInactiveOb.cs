using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class refToInactiveOb : MonoBehaviour {
    public Image art;
    public Image back;
    RectTransform rt;

    public void deletThis()
    {
        Destroy(this.gameObject);
    }

    //public void move()
    //{
    //    rt = GetComponent<RectTransform>();
    //    rt.anchoredPosition = new Vector3(0, 0, 0);

    //}


}
