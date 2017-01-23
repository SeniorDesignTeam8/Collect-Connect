using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMovement : MonoBehaviour
{


    private Vector3 currentPosition;
	// Use this for initialization
	void Start ()
	{
	    currentPosition = transform.localPosition;
	}

    private void OnMouseDrag()
    {
        gameObject.transform.localPosition = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        currentPosition = transform.localPosition;
    }
    
}
