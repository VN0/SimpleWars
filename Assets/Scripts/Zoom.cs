using UnityEngine;
using UnityEngine.EventSystems;

public class Zoom : MonoBehaviour
{
    public float zoomSpeed = 1;
    public float target;
    public float smoothSpeed = 2.0f;
    public float minOrtho = 1.0f;
    public float maxOrtho = 20.0f;
    public float minPinchDistance = 20;

    Camera cam;
    float startDistance;
    bool zooming = true;

    void Awake ()
    {
        cam = GetComponent<Camera>();
    }

    void Start ()
    {
        target = cam.orthographicSize;
    }

    void Update ()
    {
        if (Input.mousePresent)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0.0f && !EventSystem.current.IsPointerOverGameObject())
            {
                target -= scroll * zoomSpeed * cam.orthographicSize / 30;
                target = Mathf.Clamp(target, minOrtho, maxOrtho);
            }

            cam.orthographicSize = Mathf.MoveTowards(
                cam.orthographicSize, target, smoothSpeed * Time.unscaledDeltaTime * cam.orthographicSize / 30);
        }

        if (Input.touchCount >= 2)
        {
            Vector2 touch0, touch1;
            float distance = 0;
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            if (!zooming && Vector2.Distance(touch0, touch1) > minPinchDistance)
            {
                touch0 = cam.ScreenToWorldPoint(touch0);
                touch1 = cam.ScreenToWorldPoint(touch1);
                distance = Vector2.Distance(touch0, touch1);
                startDistance = distance;
            }
            else
            {
                touch0 = cam.ScreenToWorldPoint(touch0);
                touch1 = cam.ScreenToWorldPoint(touch1);
            }
            zooming = true;
            target = (distance - startDistance) + cam.orthographicSize;
            target = Mathf.Clamp(target, minOrtho, maxOrtho);
        }
        else
        {
            startDistance = 0;
            zooming = false;
        }
        cam.orthographicSize = Mathf.MoveTowards(
            cam.orthographicSize, target, smoothSpeed * Time.unscaledDeltaTime * cam.orthographicSize / 30);
    }
}