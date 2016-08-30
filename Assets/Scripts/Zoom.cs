using UnityEngine;
using UnityEngine.EventSystems;

public class Zoom : MonoBehaviour
{
    public float zoomSpeed = 1;
    public float target;
    public float smoothSpeed = 2.0f;
    public float minOrtho = 1.0f;
    public float maxOrtho = 20.0f;

    Camera cam;
    float startDistance;
    bool zooming = false;

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
            touch0 = cam.ScreenToWorldPoint(Input.GetTouch(0).position);
            touch1 = cam.ScreenToWorldPoint(Input.GetTouch(1).position);
            if (!zooming)
            {
                distance = Vector2.Distance(touch0, touch1);
                startDistance = distance;
            }
            zooming = true;
            target = cam.orthographicSize + startDistance - distance;
            target = Mathf.Clamp(target, minOrtho, maxOrtho);
        }
        else
        {
            zooming = false;
        }
        cam.orthographicSize = Mathf.MoveTowards(
            cam.orthographicSize, target, smoothSpeed * Time.unscaledDeltaTime * cam.orthographicSize / 30);
    }
}
