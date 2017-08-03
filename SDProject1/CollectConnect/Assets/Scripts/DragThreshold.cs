using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragThreshold : MonoBehaviour {

	void Start () {
		int defaultValue = EventSystem.current.pixelDragThreshold;		
		EventSystem.current.pixelDragThreshold = 
			Mathf.Max(
				defaultValue , 
				(int) (defaultValue * Screen.dpi / 160f));

		//Debug.Log (EventSystem.current.pixelDragThreshold);
	}
	

}
