using UnityEngine;
using UnityEngine.EventSystems;
using TouchScript.Gestures;

public class Zoom : MonoBehaviour
{
	public float zoomSpeed = 1;
	public float targetOrtho;
	public float smoothSpeed = 2.0f;
	public float minOrtho = 1.0f;
	public float maxOrtho = 20.0f;
    public ScreenTransformGesture gesture;

    void Awake ()
    {
        gesture = FindObjectOfType<ScreenTransformGesture>();
    }
     
	void Start ()
	{
		targetOrtho = Camera.main.orthographicSize;
	}

    void Update ()
    {
        if (gesture.NumTouches == 2)
        {
            targetOrtho = Camera.main.orthographicSize /= gesture.DeltaScale;
        }

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
    }
}