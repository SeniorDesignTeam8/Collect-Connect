using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonManager : MonoBehaviour
{
    private Button _yourButton;

    // Use this for initialization
    void Start()
    {
        _yourButton = GetComponent<Button>();
        Button btn = _yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void TaskOnClick()
    {
        Debug.Log(tag);
    }
}
