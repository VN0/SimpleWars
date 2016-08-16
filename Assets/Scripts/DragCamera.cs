using UnityEngine;

public class DragCamera : MonoBehaviour
{
	Vector3 dragOrigin;
    int button = 1;

    void Awake ()
    {
        button = Input.mousePresent ? 1 : 0;
    }

    void Update ()
	{
        if (Input.GetMouseButtonDown(button))
        {
            dragOrigin = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            dragOrigin = GetComponent<Camera>().ScreenToWorldPoint(dragOrigin);
        }

        if (Input.GetMouseButton(button))
        {
            Vector3 currentPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            currentPos = GetComponent<Camera>().ScreenToWorldPoint(currentPos);
            Vector3 movePos = dragOrigin - currentPos;
            transform.position = transform.position + movePos;
        }
    }
}