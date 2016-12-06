using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

namespace AsPerSpec {

	[RequireComponent (typeof(CarouselToggler))]
	public class CarouselRotator : MonoBehaviour, IBeginDragHandler, IEndDragHandler {
		public float delay = 5;
		public bool animating = true;
		bool dragging = false;
		float nextChangeTime;
		int currentToggleIndex=0;
		RectTransform content;
		Toggle lastToggle = null;
		CarouselToggler carouselToggler;
		
		public void OnBeginDrag (PointerEventData eventData) {
			dragging = true;
		}
		
		public void OnEndDrag (PointerEventData eventData) {
			ResetWaitTimer ();
			dragging = false;
		}
		
		void Start () {
			carouselToggler = gameObject.GetComponent<CarouselToggler> ();
			ScrollRect scrollRect = gameObject.GetComponent<ScrollRect> ();
			content = scrollRect.content;
			StartCoroutine (WaitInactiveCycleAndReset());
		}
		
		public void ResetWaitTimer() {
			nextChangeTime = Time.realtimeSinceStartup + delay;
		}
		
		IEnumerator WaitInactiveCycleAndReset()
		{
			ResetWaitTimer ();
			while(Time.realtimeSinceStartup < nextChangeTime) //check time and listen for keypress
			{
				yield return 0;
			}
			if ((animating) && (!dragging)) {
				Step (true);
			}
			StartCoroutine (WaitInactiveCycleAndReset());
		}

		public void Step(bool forward) {
			Toggle[] toggles = content.GetComponentsInChildren<Toggle> ();
			if (toggles.Length > 0) {
				// if any toggles to cycle
				if (lastToggle == null) {
					lastToggle = toggles[0];
				}
				if (!lastToggle || !lastToggle.isOn) {
					// toggle was changed, try to continue from there
					Toggle onToggle = null;
					for (int i=0;i<toggles.Length;i++) {
						if (toggles[i].isOn) {
							onToggle = toggles[i];
						}
					}
					if (onToggle!=null) {
						// found a selected toggle to continue from
						lastToggle = onToggle;
					}
				}
				// it's not the first cycle
				if ((currentToggleIndex >= toggles.Length)||
				    (lastToggle != toggles[currentToggleIndex])) {
					// last activated toggle is not in the same index, find it
					currentToggleIndex = Array.IndexOf(toggles, lastToggle);
					if (currentToggleIndex < 0) { // didn't find it
						currentToggleIndex = 0; // start list from scratch
					}
				}
				if (forward) {
					currentToggleIndex++;
				}
				else {
					currentToggleIndex--;
				}

				if (currentToggleIndex>=toggles.Length) {
					currentToggleIndex = 0; // wrap
				}
				if (currentToggleIndex<0) {
					currentToggleIndex = toggles.Length-1;
				}
				lastToggle = toggles[currentToggleIndex];
				if (toggles.Length > 0 && lastToggle) {
					lastToggle.isOn = true;
					carouselToggler.CenterOnToggled ();
				}

			}
		}
	}

}
