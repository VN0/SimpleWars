using UnityEngine;
using UnityEngine.EventSystems;

public class Zoom : MonoBehaviour
{
    public float zoomSpeed = 1;
    public float targetOrtho;
    public float smoothSpeed = 2.0f;
    public float minOrtho = 1.0f;
    public float maxOrtho = 20.0f;
    public float minPinchDistance = 20;

    float startDistance;
    bool zooming = true;

    void Start ()
    {
        targetOrtho = Camera.main.orthographicSize;
    }

    void Update ()
    {
        if (Input.mousePresent)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0.0f && !EventSystem.current.IsPointerOverGameObject())
            {
                targetOrtho -= scroll * zoomSpeed * Camera.main.orthographicSize / 30;
                targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
            }

            Camera.main.orthographicSize = Mathf.MoveTowards(
                Camera.main.orthographicSize, targetOrtho, smoothSpeed * Time.unscaledDeltaTime * Camera.main.orthographicSize / 30);
        }

        if (Input.touchCount >= 2)
        {
            Vector2 touch0, touch1;
            float distance = 0;
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            if (!zooming && Vector2.Distance(touch0, touch1) > minPinchDistance)
            {
                touch0 = Camera.main.ScreenToWorldPoint(touch0);
                touch1 = Camera.main.ScreenToWorldPoint(touch1);
                distance = Vector2.Distance(touch0, touch1);
                startDistance = distance;
            }
            else
            {
                touch0 = Camera.main.ScreenToWorldPoint(touch0);
                touch1 = Camera.main.ScreenToWorldPoint(touch1);
            }
            zooming = true;
            float target = (distance - startDistance) + Camera.main.orthographicSize;
            //print(new Vector2(target, Camera.main.orthographicSize));
            target = Mathf.Clamp(target, minOrtho, maxOrtho);
            Camera.main.orthographicSize = Mathf.MoveTowards(
                Camera.main.orthographicSize, target, smoothSpeed * Time.unscaledDeltaTime * Camera.main.orthographicSize / 30);
        }
        else
        {
            startDistance = 0;
            zooming = false;
        }
    }
}