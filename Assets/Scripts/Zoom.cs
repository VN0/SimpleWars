using UnityEngine;
using UnityEngine.EventSystems;

public class Zoom : MonoBehaviour
{
     
	public float zoomSpeed = 1;
	public float targetOrtho;
	public float smoothSpeed = 2.0f;
	public float minOrtho = 1.0f;
	public float maxOrtho = 20.0f;
	float lastTime;
	float currentTime;

	void Awake ()
	{
		lastTime = Time.realtimeSinceStartup;
	}
     
	void Start ()
	{
		targetOrtho = Camera.main.orthographicSize;
	}

	void Update ()
	{
		currentTime = Time.realtimeSinceStartup;
		float scroll = Input.GetAxis ("Mouse ScrollWheel");
		if (scroll != 0.0f && !EventSystem.current.IsPointerOverGameObject()) {
			targetOrtho -= scroll * zoomSpeed * Camera.main.orthographicSize / 30;
			targetOrtho = Mathf.Clamp (targetOrtho, minOrtho, maxOrtho);
		}
         
		Camera.main.orthographicSize = Mathf.MoveTowards (Camera.main.orthographicSize, targetOrtho, smoothSpeed * (currentTime - lastTime) * Camera.main.orthographicSize / 30);
		lastTime = currentTime;
	}
}