using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldAmount : MonoBehaviour
{
    public int amount=0;
    private void Start()
    {
        if (this.tag == "card")
            amount = 1;
        else amount = 2;
    }

}
