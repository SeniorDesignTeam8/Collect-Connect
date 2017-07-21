using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace AsPerSpec {

	[RequireComponent (typeof(ScrollRect))]
	public class CarouselToggler : MonoBehaviour, IInitializePotentialDragHandler, IEndDragHandler {
        public bool scaleTime = true;
        public bool snap = true;
		public bool nonStick = false;
		[Range(0.001f, 2f)] public float stickyFactor = 1;
		public bool reveal = false;
		public bool centerOnStartup=true;
		public float snapSpeed =10;
		public bool inertia=false;
		public float inertialDamping = 1F;
		public float inertiaStopThreshold = 0.1F;
		public bool horizontalWrap = false;
		public bool verticalWrap = false;

		Vector2 wrapCount = new Vector2 (0, 0);
		RectTransform contentRectTransform;
		Vector2 targetPosition;
		Vector2 startingPosition;
		Vector2 lastPos = new Vector2(0,0);
		Vector2 secondLastPos = new Vector2(0,0);
		Vector2 inertiaSpeed = new Vector2(0,0);
		float currentProgress = 0;
		Toggle targetToggle;
		bool movingToPosition = false;
		bool sliding = false;
		public bool moving {
			get { return sliding || movingToPosition; }
			private set {}
		}

		public void OnInitializePotentialDrag (PointerEventData eventData) {
			movingToPosition = false;
			sliding = false;
			wrapCount = new Vector2 (0, 0);
			if (reveal) {
				Mask mask = gameObject.GetComponent<Mask> ();
				mask.enabled = false;
			}
		}

		public void OnEndDrag (PointerEventData eventData) {
			if (reveal) {
				Mask mask = gameObject.GetComponent<Mask> ();
				mask.enabled = true;
			}
			if (inertia) {
				sliding = true;
			} else {
				if (snap) {
					SnapToClosest ();
				}
			}
		}

		void Start() {
			ScrollRect scrollRect = gameObject.GetComponent<ScrollRect> ();
			contentRectTransform = scrollRect.content;
			if (centerOnStartup) {
				CenterOnToggled ();
				currentProgress = 1; // jump to destination
			}
		}

		void LateUpdate () {
			ScrollRect scrollRect = gameObject.GetComponent<ScrollRect> ();

			inertiaSpeed = (lastPos - secondLastPos + wrapCount);
            if (scaleTime)
            {
                inertiaSpeed /= Time.deltaTime;
            }
            else
            {
                inertiaSpeed /= Time.unscaledDeltaTime;
            }


			if (sliding) {

				if (inertiaSpeed.magnitude<inertiaStopThreshold) {
					sliding = false;
					if (snap) {
						SnapToClosest();
					}
				}
				else {
                    float time = scaleTime ? Time.deltaTime : Time.unscaledDeltaTime;
					float updatedMagnitude = inertiaSpeed.magnitude-(time*inertialDamping*inertialDamping);
					if (updatedMagnitude<0) {

						updatedMagnitude = 0;
					}
					inertiaSpeed = inertiaSpeed.normalized*updatedMagnitude;
					scrollRect.normalizedPosition+=inertiaSpeed*time;
				}
			}
			else {
				if (movingToPosition) {
					float motionPercent = snapSpeed*(1.1f-currentProgress); //1.1 -> non asymptotic approach
					motionPercent *= scaleTime ? Time.deltaTime : Time.unscaledDeltaTime;
                    currentProgress +=motionPercent;
					if (currentProgress > 1) {
						currentProgress = 1; // catch rounding and timing errors
					}
					contentRectTransform.anchoredPosition =
						startingPosition +(currentProgress*(targetPosition-startingPosition));
					if (currentProgress == 1) {
						movingToPosition = false;
						targetToggle.isOn = true;
					}
				}
			}

			if (horizontalWrap||verticalWrap) {
				if (horizontalWrap) {
					wrapCount.x = 0;
					if (scrollRect.horizontalNormalizedPosition<0) {
						wrapCount.x = -1+(int)scrollRect.horizontalNormalizedPosition;
					}

					if (scrollRect.horizontalNormalizedPosition>=1) {
						wrapCount.x = (int)scrollRect.horizontalNormalizedPosition;
					}
				}
				if (verticalWrap) {
					wrapCount.y = 0;
					if (scrollRect.verticalNormalizedPosition<0) {
						wrapCount.y = -1+(int)scrollRect.verticalNormalizedPosition;
					}

					if (scrollRect.verticalNormalizedPosition>=1) {
						wrapCount.y = (int)scrollRect.verticalNormalizedPosition;
					}
				}
				scrollRect.normalizedPosition = scrollRect.normalizedPosition - wrapCount;
			}
			secondLastPos = lastPos + wrapCount;
			lastPos = scrollRect.normalizedPosition;
		}

		public void SnapToClosest () {
			Toggle[] toggles = contentRectTransform.GetComponentsInChildren<Toggle> ();
			if (toggles.Length > 0) {
				Toggle toggleClosest = findClosestToggle (toggles);
				CenterToggleOnRect (toggleClosest);
			}
		}

		public void CenterOnToggled () {
			Toggle[] toggles = contentRectTransform.GetComponentsInChildren<Toggle> ();
			Toggle onToggle = null;
			for (int i=0; !onToggle && (i<toggles.Length); ++i) {
				if (toggles[i].isOn) {
					onToggle = toggles[i];
				}
			}
			if (onToggle) { // at least one item is already toggled, focus it
				CenterToggleOnRect(onToggle);
			}
		}

		Toggle findClosestToggle(Toggle[] toggles) { // closest to scrollRect center
			RectTransform toggleRectTransform;
			RectTransform rectTransform = gameObject.GetComponent<RectTransform> ();
			Toggle toggleClosest = toggles[0];
			Toggle last = toggles [0];
			Vector3 diff,minDiff;
			float distance,minDistance;
			toggleRectTransform = toggleClosest.GetComponent<RectTransform>();
			minDiff = GetWorldCenter(rectTransform) - GetWorldCenter(toggleRectTransform);
			minDistance = minDiff.magnitude;
			if (toggleClosest.isOn) {
				last = toggleClosest;
				minDistance/=stickyFactor;
				if (nonStick && toggles.Length > 1) {
					toggleClosest = toggles[1];
					toggleRectTransform = toggleClosest.GetComponent<RectTransform>();
					minDiff = GetWorldCenter(rectTransform) - GetWorldCenter(toggleRectTransform);
					minDistance = minDiff.magnitude;
				}
			}
			foreach (Toggle toggle in toggles) {
				toggleRectTransform = toggle.GetComponent<RectTransform>();
				diff = GetWorldCenter(rectTransform) - GetWorldCenter(toggleRectTransform);
				distance = diff.magnitude;
				if (toggle.isOn) {
					last = toggle;
					distance/=stickyFactor;
				}
				if (
				    ( distance < minDistance )
					 ) {
					if (!(nonStick && toggle.isOn)) { // if nonStick and is last, skip.
						minDiff = diff;
						minDistance = distance;
						toggleClosest = toggle;
					}
				}
			}

			if (nonStick || stickyFactor < 1) {
				toggleRectTransform = last.GetComponent<RectTransform>();
				diff = GetWorldCenter(rectTransform) - GetWorldCenter(toggleRectTransform);
				distance = diff.magnitude;
				if (distance < minDistance) { // we ignored last only because of nonstick
					Vector3 togglesDiff = GetWorldCenter(last.GetComponent<RectTransform>()) - GetWorldCenter(toggleClosest.GetComponent<RectTransform>());
					if (togglesDiff.magnitude < minDistance) { // dragged outwards and last was edge
						toggleClosest = last; // correct *edge* case
					}
				}
			}
			return toggleClosest;
		}

		void CenterToggleOnRect(Toggle toggle) {
			RectTransform toggleRectTransform = toggle.GetComponent<RectTransform> ();
			RectTransform rectTransform = gameObject.GetComponent<RectTransform> ();
			CarouselToggler carousel = gameObject.GetComponent<CarouselToggler> ();
			ScrollRect scrollRect = gameObject.GetComponent<ScrollRect> ();
			Vector3 diff = GetWorldCenter(rectTransform) - GetWorldCenter(toggleRectTransform);
			diff = contentRectTransform.InverseTransformVector (diff);
			if (carousel.horizontalWrap && scrollRect.horizontal) {
				if ( Mathf.Abs(diff[0]) > Mathf.Abs(scrollRect.content.rect.height-Mathf.Abs(diff[0])-rectTransform.rect.height) ) {
					diff[0] = -1*Mathf.Sign(diff[0])*Mathf.Abs(scrollRect.content.rect.height-Mathf.Abs(diff[0])-rectTransform.rect.height);
				}
			}
			if (carousel.verticalWrap && scrollRect.vertical) {
				if ( Mathf.Abs(diff[1]) > Mathf.Abs(scrollRect.content.rect.height-Mathf.Abs(diff[1])-rectTransform.rect.height) ) {
					diff[1] = -1*Mathf.Sign(diff[1])*Mathf.Abs(scrollRect.content.rect.height-Mathf.Abs(diff[1])-rectTransform.rect.height);
				}
			}
			Vector2 currentPosition = contentRectTransform.anchoredPosition;
			targetPosition.Set (
				scrollRect.horizontal? currentPosition.x + (diff[0]) : currentPosition.x,
				scrollRect.vertical?   currentPosition.y + (diff[1]) : currentPosition.y
				);
			startingPosition.Set(currentPosition.x,currentPosition.y);
			movingToPosition = true;
			currentProgress = 0;
			targetToggle = toggle;
		}

		public void AddImpulse(Vector2 impulse) {
            float time = scaleTime ? Time.deltaTime : Time.unscaledDeltaTime;
            if (inertia) {
				inertiaSpeed += impulse; // will be overwritten at update
				secondLastPos = lastPos-(inertiaSpeed*time);
				sliding = true;
			}
		}

		public void Stop() {
			sliding = false;
		}

		public Vector3 GetWorldCenter(RectTransform recttransform) {
			Vector2 center = recttransform.rect.center;
			return recttransform.TransformPoint (center.x, center.y, 0);
		}

	}
}
