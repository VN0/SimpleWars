using UnityEngine;

public class DragCamera : MonoBehaviour
{
	Vector3 dragOrigin;

	void Update ()
	{
		if (Input.GetMouseButtonDown (1)) {
			dragOrigin = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);
			dragOrigin = GetComponent<Camera>().ScreenToWorldPoint (dragOrigin);
		}
		
		if (Input.GetMouseButton (1)) {
			Vector3 currentPos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);
			currentPos = GetComponent<Camera>().ScreenToWorldPoint (currentPos);
			Vector3 movePos = dragOrigin - currentPos;
			transform.position = transform.position + movePos;
		}
	}
}