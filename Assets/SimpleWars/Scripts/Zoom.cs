using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleWars
{
    [RequireComponent(typeof(Camera))]
    public class Zoom : MonoBehaviour
    {
        public float zoomSpeed = 1;
        public float pinchZoomSpeed = 1;
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
                    target -= scroll * zoomSpeed * cam.orthographicSize / 20;
                    target = Mathf.Clamp(target, minOrtho, maxOrtho);
                }

                cam.orthographicSize = Mathf.MoveTowards(
                    cam.orthographicSize, target, smoothSpeed * Time.unscaledDeltaTime * cam.orthographicSize / 20);
            }
            if (Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                // ... change the orthographic size based on the change in distance between the touches.
                cam.orthographicSize += deltaMagnitudeDiff * pinchZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                cam.orthographicSize = Mathf.Max(cam.orthographicSize, 0.1f);
            }
            cam.orthographicSize = Mathf.MoveTowards(
                cam.orthographicSize, target, smoothSpeed * Time.unscaledDeltaTime * cam.orthographicSize);
        }
    }
}