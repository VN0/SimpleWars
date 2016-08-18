using UnityEngine;
using UnityEngine.EventSystems;

public class DragObject : MonoBehaviour, IDragHandler
{
    public bool dragSelf = false;
    public Transform objectToDrag;
    public float dragDistance;

    void Awake ()
    {
        if (dragSelf || objectToDrag == null)
        {
            objectToDrag = transform;
        }
    }

    public void OnDrag (PointerEventData eventData)
    {
        objectToDrag.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
    }
}
