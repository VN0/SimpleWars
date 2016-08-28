using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    public Material normal;
    public Material highlighted;

    public GameObject reference { get; set; }

    Collider2D collider2d;
    LineRenderer lr;

    void Awake ()
    {
        collider2d = GetComponent<Collider2D>();
        lr = GetComponent<LineRenderer>();
    }

    void FixedUpdate ()
    {
        
    }

    void OnCollisionStay2D (Collision2D col)
    {
        print(col);
    }
}
