using UnityEngine;
using UnityEngine.EventSystems;

public class FollowCamera : MonoBehaviour
{
    //public float interpVelocity;
    //public float minDistance;
    //public float followDistance;
    //public float followSpeed = 1;
    public Transform target;
    public Vector3 offset;
    Vector3 dragOrigin;
    bool dragging = false;
    int button = 1;


    void Awake ()
    {
        button = Input.mousePresent ? 1 : 0;
    }


    void Update ()
    {
        if (target)
        {
            transform.position = new Vector3(target.position.x + offset.x, target.position.y + offset.y, -10);
        }
        else
        {
            try
            {
                target = GameObject.FindGameObjectWithTag("Player").transform;
            }
            catch { }
        }

        if (Input.GetMouseButtonDown(button) && !EventSystem.current.IsPointerOverGameObject())
        {
            dragOrigin = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            dragging = true;
        }

        if (dragging)
        {
            offset = Camera.main.ScreenToWorldPoint(dragOrigin) - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        }

        if (Input.GetMouseButtonUp(button))
        {
            dragging = false;
            offset = Vector3.zero;
        }
    }
}