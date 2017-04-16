using UnityEngine;
using UnityEngine.EventSystems;

public class FollowCamera : MonoBehaviour
{
    //public float interpVelocity;
    //public float minDistance;
    //public float followDistance;
    //public float followSpeed = 1;
    public bool followTarget = true;
    public Transform target;
    public Vector3 offset;
    public Vector3 targetOffset;
    Vector3 lastPos;
    Vector2 currentVelocity = Vector2.zero;
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
            target = new GameObject("DragOrigin").transform;
            target.position = transform.position;
            followTarget = false;
            offset = Vector2.zero;
            targetOffset = Vector2.zero;
        }

        if (Input.GetMouseButtonDown(button) && !EventSystem.current.IsPointerOverGameObject())
        {
            lastPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            dragging = true;
        }

        if (!Input.GetMouseButton(button))
        {
            dragging = false;
            if (followTarget)
            {
                //offset.x = Mathf.SmoothStep(offset.x, 0, 10 * Time.deltaTime);
                //offset.y = Mathf.SmoothStep(offset.y, 0, 10 * Time.deltaTime);
                offset = Vector2.SmoothDamp(offset, Vector2.zero, ref currentVelocity, 0.5f, Mathf.Infinity, Time.deltaTime);
                targetOffset = offset;
            }
            else
            {
                offset = Vector2.SmoothDamp(offset, targetOffset, ref currentVelocity, 0.3f, Mathf.Infinity, Time.unscaledDeltaTime);
            }
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
            //offset.x = Mathf.SmoothStep(offset.x, targetOffset.x, 20 * Time.unscaledDeltaTime);
            //offset.y = Mathf.SmoothStep(offset.y, targetOffset.y, 20 * Time.unscaledDeltaTime);
            offset = Vector2.SmoothDamp(offset, targetOffset, ref currentVelocity, 0.3f, Mathf.Infinity, Time.unscaledDeltaTime);
            lastPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        }
    }
}