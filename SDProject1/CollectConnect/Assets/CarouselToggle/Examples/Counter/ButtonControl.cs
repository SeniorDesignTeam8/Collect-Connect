using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonControl : MonoBehaviour {
	public ButtonControl nextDigit;
	public Toggle first;
	public Toggle last;
	AsPerSpec.CarouselRotator rotator;

	void Start() {
		rotator = gameObject.GetComponent<AsPerSpec.CarouselRotator> ();
	}

	public void Move(bool forward) {
		if (nextDigit != null) {
			if (forward) {
				if (last.isOn) {
					nextDigit.Move(true);
				}
			}
			else {
				if (first.isOn) {
					nextDigit.Move(false);
				}
			}
		}
		rotator.Step (forward);
	}

}
