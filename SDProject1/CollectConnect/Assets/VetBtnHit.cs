using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VetBtnHit : MonoBehaviour {

    public void on_btn_click()
    {
        BoardManager.Instance.VetBtnSelected();
        Debug.Log("Got here 1");
    }
}
