using UnityEngine;

public class MobileNode : MonoBehaviour
{
    private Vector3 _pointerDownPosition;
    private Vector3 _screenPoint;
    private Vector3 _offset;
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
        _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
		_isDragging = true;
    }

    private void OnMouseUp()
    {
        _isDragging = false;
    }

    private void OnMouseDrag()
    {
        if (Vector3.Distance(_pointerDownPosition, Input.mousePosition) > gameObject.GetComponent<RectTransform>().rect.width)
            _isDragging = true;
        if (_isDragging)
        {
            Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + _offset;
            transform.position = cursorPosition;
        }
    }
}