using UnityEngine;

public class MobileNode : MonoBehaviour
{
    private Vector3 _pointerDownPosition;
    private bool _isDragging;
    // Use this for initialization
    private void Start()
    {
        _isDragging = false;
    }


    private void OnMouseDown()
    {
        // TODO Check for another dragging node with non-null Vector3?
        _pointerDownPosition = transform.position;
    }

    private void OnMouseUp()
    {
        _isDragging = false;
    }

    private void OnMouseDrag()
    {
        if (Vector3.Distance(_pointerDownPosition, Input.mousePosition) >
            gameObject.GetComponent<RectTransform>().rect.width / 2)
        _isDragging = true;
    }

    private void LateUpdate()
    {
        if (_isDragging)
        {
            Vector3 actualMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(actualMousePosition.x, actualMousePosition.y, transform.position.z);
        }
    }
}