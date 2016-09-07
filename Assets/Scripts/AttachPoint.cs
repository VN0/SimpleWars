using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    public Material normal;
    public Material highlighted;

    public GameObject Reference { get; set; }

    Collider2D col;
    LineRenderer lr;
    bool placed = false;
    GameObject target;
    
    public void Place ()
    {
        placed = true;
    }

    void Awake ()
    {
        Time.timeScale = 0;
        col = GetComponent<BoxCollider2D>();
        lr = GetComponent<LineRenderer>();
    }
    
    void OnTriggerStay2D (Collider2D other)
    {
        if (!col.bounds.Intersects(other.bounds))
        {
            lr.material = highlighted;
        }
        else
        {
            lr.material = normal;
        }
    }

    void OnTriggerExit2D (Collider2D other)
    {
        lr.material = normal;
    }
}
