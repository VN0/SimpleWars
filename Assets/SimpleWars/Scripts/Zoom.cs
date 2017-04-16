using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleWars
{
    [RequireComponent(typeof(Camera))]
    public class Zoom : MonoBehaviour
    {
        public float zoomSpeed = 1;
        public float pinchZoomSpeed = 1;
        public bool exact = false;
        [DG.DeInspektor.Attributes.DeConditional("exact", false)]
        public float smoothSpeed = 2.0f;
        [DG.DeInspektor.Attributes.DeConditional("exact", false)]
        public float target;
        public float minOrtho = 1.0f;
        public float maxOrtho = 20.0f;

        Camera cam;
        float startDistance;

        // Ortographic camera zoom towards a point (in world coordinates). Negative amount zooms in, positive zooms out
        public void ZoomOrthoCamera (Vector3 zoomTowards, float amount)
        {
            float size = cam.orthographicSize;
            if ((amount > 0 && size <= minOrtho + Mathf.Epsilon) || (amount < 0 && size >= maxOrtho - Mathf.Epsilon))
            {
                return;
            }
            // Calculate how much we will have to move towards the zoomTowards position
            float multiplier = (1.0f / cam.orthographicSize * amount);

            // Move camera
            transform.position += (zoomTowards - transform.position) * multiplier;

            // Zoom camera
            size -= amount;

            // Limit zoom
            cam.orthographicSize = Mathf.Clamp(size, minOrtho, maxOrtho);
        }

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
                    if (exact)
                    {
                        ZoomOrthoCamera(cam.ScreenToWorldPoint(Input.mousePosition), zoomSpeed * scroll * cam.orthographicSize / 20);
                        return;
                    }
                    target -= scroll * zoomSpeed * cam.orthographicSize / 20;
                    target = Mathf.Clamp(target, minOrtho, maxOrtho);
                }

                //if (!exact)
                //{
                //    cam.orthographicSize = Mathf.MoveTowards(
                //        cam.orthographicSize, target, smoothSpeed * Time.unscaledDeltaTime * cam.orthographicSize / 20);
                //}
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
            if (!exact)
            {
                cam.orthographicSize = Mathf.SmoothStep(
                    cam.orthographicSize, target, smoothSpeed * Time.unscaledDeltaTime);
            }
        }
    }
}