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
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector3 touch0_pos = touch0.position;
            Vector3 touch1_pos = touch1.position;

            Vector3 touch0_prevPos = touch0.position - touch0.deltaPosition;
            Vector3 touch1_prevPos = touch1.position - touch1.deltaPosition;

            float prev_TouchDeltaMag = (cam.ScreenToWorldPoint(touch0_prevPos) - cam.ScreenToWorldPoint(touch1_prevPos)).magnitude;
            float current_TouchDeltaMag = (cam.ScreenToWorldPoint(touch0_pos) - cam.ScreenToWorldPoint(touch1_pos)).magnitude;

            float deltaMagDiff = prev_TouchDeltaMag - current_TouchDeltaMag;

            float currentDistance = Vector3.Distance(cam.ScreenToWorldPoint(touch0_pos), cam.ScreenToWorldPoint(touch1_pos));
            float lastDistance = Vector3.Distance(cam.ScreenToWorldPoint(touch0_prevPos), cam.ScreenToWorldPoint(touch1_prevPos));

            float diff = lastDistance - currentDistance;

            Debug.LogFormat("{0} {1}", deltaMagDiff, diff);

            target += deltaMagDiff;
            
            target = Mathf.Clamp(target, minOrtho, maxOrtho);
        }
        cam.orthographicSize = Mathf.MoveTowards(
            cam.orthographicSize, target, smoothSpeed * Time.unscaledDeltaTime * cam.orthographicSize);
    }
}
