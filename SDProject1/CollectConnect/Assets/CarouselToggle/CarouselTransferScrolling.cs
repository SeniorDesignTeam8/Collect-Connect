using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace AsPerSpec
{

    [RequireComponent(typeof(ScrollRect))]
    public class CarouselTransferScrolling : MonoBehaviour, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler, IBeginDragHandler
    {
        public CarouselToggler targetCarousel;
        public enum AxisName { horizontal, vertical };
        public AxisName copyAxis;
        public bool clampTarget = false;
        public bool exclusive = false;
        Vector2 previousPosition;
        bool targetDrag = false;
        bool mainAxisY = false;

        public void OnDrag(PointerEventData eventData) {
            if (targetDrag) {
                // copy movement to carousel
                ScrollRect targetScrollRect = targetCarousel.GetComponent<ScrollRect>();
                RectTransform targetRectTransform = targetCarousel.GetComponent<RectTransform>();
                Vector3 transformedDelta = targetRectTransform.InverseTransformVector(eventData.delta.x, eventData.delta.y, 0);
                transformedDelta.x = transformedDelta.x / targetRectTransform.rect.width;
                transformedDelta.y = transformedDelta.y / targetRectTransform.rect.height;
                if (copyAxis == AxisName.horizontal) {
                    if ((!exclusive)||(!mainAxisY)) { // skip if exclusive and not main axis
                        targetScrollRect.horizontalNormalizedPosition = targetScrollRect.horizontalNormalizedPosition - transformedDelta.x;
                        if (clampTarget) {
                            targetScrollRect.horizontalNormalizedPosition = Mathf.Clamp(targetScrollRect.horizontalNormalizedPosition, 0, 1);
                        }
                    }
                }
                else {
                    if ((!exclusive)|| mainAxisY) { // skip if exclusive and not main axis
                        targetScrollRect.verticalNormalizedPosition = targetScrollRect.verticalNormalizedPosition - transformedDelta.y;
                        if (clampTarget) {
                            targetScrollRect.verticalNormalizedPosition = Mathf.Clamp(targetScrollRect.verticalNormalizedPosition, 0, 1);
                        }
                    }
                }
            }
        }

        public void OnInitializePotentialDrag(PointerEventData eventData) {
            ScrollRect targetScrollRect = targetCarousel.GetComponent<ScrollRect>();
            GameObject contentGameObject = targetScrollRect.content.gameObject;
            GameObject clickedGameObject = eventData.pointerCurrentRaycast.gameObject;
            if (
                (clickedGameObject == contentGameObject) ||
                (clickedGameObject == targetCarousel.gameObject) ||
                (clickedGameObject.transform.IsChildOf(contentGameObject.transform))
                ) {
                targetCarousel.OnInitializePotentialDrag(eventData);
                CarouselRotator carouselRotator = targetCarousel.GetComponent<CarouselRotator>();
                if (carouselRotator) {
                    carouselRotator.OnBeginDrag(eventData);
                }
                targetDrag = true;
            }
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if (Mathf.Abs(eventData.delta.y) > Mathf.Abs(eventData.delta.x)) {
                mainAxisY = true;
            }
            else {
                mainAxisY = false;
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (targetDrag) {
                CarouselRotator carouselRotator = targetCarousel.GetComponent<CarouselRotator>();
                if (carouselRotator) {
                    carouselRotator.OnEndDrag(eventData);
                }
                targetCarousel.SnapToClosest();
                targetDrag = false;

                if ((mainAxisY && (copyAxis == AxisName.vertical)) ||
                    (!mainAxisY && (copyAxis == AxisName.horizontal))) {
                    ScrollRect scrollrect = GetComponent<ScrollRect>();
                    if (scrollrect.inertia) {
                        scrollrect.StopMovement();
                    }
                }
            }
        }

        void Start() {
            ScrollRect scrollRect = GetComponent<ScrollRect>();
            RectTransform contentRectTransform = scrollRect.content.GetComponent<RectTransform>();
            previousPosition = contentRectTransform.anchoredPosition;
        }

        void LateUpdate() {
            // restore copied scroll on host ScrollRect
            ScrollRect scrollRect = GetComponent<ScrollRect>();
            RectTransform contentRectTransform = scrollRect.content.GetComponent<RectTransform>();
            Vector2 newPosition = contentRectTransform.anchoredPosition;
            if (exclusive && targetDrag) {
                if (!mainAxisY) {
                    if (copyAxis == AxisName.horizontal) {
                        contentRectTransform.anchoredPosition = previousPosition;
                    }
                    else {
                        contentRectTransform.anchoredPosition = new Vector2(newPosition.x, previousPosition.y);
                    }
                }
                else { // if mainAxisY
                    if (copyAxis == AxisName.horizontal) {
                        contentRectTransform.anchoredPosition = new Vector2(previousPosition.x, newPosition.y);
                    }
                    else {
                        contentRectTransform.anchoredPosition = previousPosition;
                    }
                }
            }
            previousPosition = contentRectTransform.anchoredPosition;
        }
    }
}