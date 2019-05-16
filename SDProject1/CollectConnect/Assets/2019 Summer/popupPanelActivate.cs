using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popupPanelActivate : MonoBehaviour
{
    public GameObject popupPanel;
    public void activateCardPopup()
    {
        popupPanel.SetActive(true);
        popupPanel.transform.SetAsLastSibling();
    }

}
