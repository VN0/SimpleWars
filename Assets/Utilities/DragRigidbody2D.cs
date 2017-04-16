using UnityEngine;
using System.Collections;
using SimpleWars;

public class DragRigidbody2D : MonoBehaviour
{
    SpringJoint2D joint;
    Rigidbody2D self;
    Rigidbody2D other;

    void Awake ()
    {
        self = gameObject.GetOrAddComponent<Rigidbody2D>();
        self.bodyType = RigidbodyType2D.Kinematic;
        joint = gameObject.GetOrAddComponent<SpringJoint2D>();
        joint.autoConfigureDistance = false;
        //LineDrawer.instance.noCache = true;
    }

    void Update ()
    {
        if (Input.GetMouseButton(2))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = pos;
            Vector2 start = Input.mousePosition + new Vector3(-10, 0, 0);
            Vector2 end = Input.mousePosition + new Vector3(10, 0, 0);
            float width = end.x - start.x;
            if (other)
            {
                LineDrawer.quads.Clear();
                LineDrawer.quads.Add(new LineDrawer.Quad(start, end, width, new Color(1, 1, 1, 0.5f), true));
                LineDrawer.quads.Add(new LineDrawer.Quad(
                    Input.mousePosition, Camera.main.WorldToScreenPoint(other.transform.position),
                    width/3, new Color(1, 1, 1, 0.5f), true));
            }
            if (Input.GetMouseButtonDown(2))
            {
                var col = Physics2D.OverlapPoint(pos);
                if (col == null)
                {
                    return;
                }
                other = col.attachedRigidbody;
                joint.connectedBody = other;
            }
        }
        else if (Input.GetMouseButtonUp(2))
        {
            other = null;
            joint.connectedBody = null;
            Unbug.Log(joint.connectedBody);
            LineDrawer.quads.Clear();
        }
    }
}
