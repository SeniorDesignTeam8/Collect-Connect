using UnityEngine;

public class makeInvisible : MonoBehaviour
{
	// Use this for initialization
    private void Start()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
    }
}
