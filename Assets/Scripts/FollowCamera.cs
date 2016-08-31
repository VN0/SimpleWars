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
    public Vector3 targetOffset;
    Vector3 lastPos;
    int lastTouchCount = 0;
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
            lastPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            dragging = true;
        }

        if (!Input.GetMouseButton(button))
        {
            dragging = false;
            offset.x = Mathf.SmoothStep(offset.x, 0, 10 * Time.deltaTime);
            offset.y = Mathf.SmoothStep(offset.y, 0, 10 * Time.deltaTime);
            targetOffset = offset;
        }

        if (dragging)
        {
            if (Input.touchCount != lastTouchCount)
            {
                lastPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                lastTouchCount = Input.touchCount;
                return;
            }
            targetOffset += Camera.main.ScreenToWorldPoint(lastPos) - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            offset.x = Mathf.SmoothStep(offset.x, targetOffset.x, 20 * Time.unscaledDeltaTime);
            offset.y = Mathf.SmoothStep(offset.y, targetOffset.y, 20 * Time.unscaledDeltaTime);
            lastPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        }
    }
}