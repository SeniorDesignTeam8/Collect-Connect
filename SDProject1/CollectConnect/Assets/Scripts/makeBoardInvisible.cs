using UnityEngine;
using System.Collections;

public class makeBoardInvisible : MonoBehaviour
{

    public Renderer rend;

	// Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = false;
    }
}
