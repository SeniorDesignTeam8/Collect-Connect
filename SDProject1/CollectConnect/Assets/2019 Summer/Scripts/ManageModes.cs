using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageModes : MonoBehaviour
{
    [SerializeField]Canvas selectMode;
    [SerializeField] Canvas rateMode;
    // Start is called before the first frame update
    public void switchFocus()
    {
        int temp = selectMode.sortingOrder;
        selectMode.sortingOrder = rateMode.sortingOrder;
        rateMode.sortingOrder = temp;
    }
}
