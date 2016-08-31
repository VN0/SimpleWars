using UnityEngine;
using UnityEngine.EventSystems;

public class DragCamera : MonoBehaviour
{
    Camera cam;
    Vector3 dragOrigin;
    bool dragging = false;
    int button = 1;
    int lastTouchCount;

    void Awake ()
    {
        button = Input.mousePresent ? 1 : 0;
        cam = GetComponent<Camera>();
    }

    void Update ()
    {
        if (Input.GetMouseButtonDown(button) && !EventSystem.current.IsPointerOverGameObject())
        {
            dragOrigin = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            dragOrigin = cam.ScreenToWorldPoint(dragOrigin);
            dragging = true;
        }

        if (Input.GetMouseButtonUp(button))
        {
            dragging = false;
        }

        if (dragging)
        {
            if (Input.touchCount != lastTouchCount)
            {
                dragOrigin = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                dragOrigin = cam.ScreenToWorldPoint(dragOrigin);
                lastTouchCount = Input.touchCount;
                return;
            }
            Vector3 currentPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            currentPos = cam.ScreenToWorldPoint(currentPos);
            Vector3 movePos = dragOrigin - currentPos;
            transform.Translate(movePos);
        }
    }
}