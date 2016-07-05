using UnityEngine;

public class DragObject : MonoBehaviour {

    SpringJoint2D spring;
	
	void Update () {
	    if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D col = Physics2D.OverlapPoint(mousePos);
            spring = col.gameObject.AddComponent<SpringJoint2D>();
            spring.dampingRatio = 0.5f;
            spring.frequency = 100;
            spring.anchor = mousePos;
            spring.distance = 1;
        }
        else if(Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            spring.connectedAnchor = mousePos;
        }
        else if(spring)
        {
            Destroy(spring);
        }
	}
}
