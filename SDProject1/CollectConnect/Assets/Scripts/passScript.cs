using UnityEngine;
using System.Collections;

public class passScript : MonoBehaviour
{

    public void OnStartGame()
    {
        BoardManager.Instance.passBtnHit();
    }

    public void Update()
    {
        
    }


}
