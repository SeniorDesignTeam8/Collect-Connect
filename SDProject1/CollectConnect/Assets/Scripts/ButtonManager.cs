using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonManager : MonoBehaviour
{
    private Button _yourbutton;

    // Use this for initialization
    void Start()
    {
        _yourbutton = GetComponent<Button>();
    }

    void OnPointerClick()
    {
        Debug.Log(_yourbutton);
    }
}
