using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    public Material normal;
    public Material highlighted;

    public GameObject reference { get; set; }

    Collider2D col;
    LineRenderer lr;

    void Awake ()
    {
        col = GetComponent<BoxCollider2D>();
        lr = GetComponent<LineRenderer>();
    }

    void Update ()
    {
        if(col.IsTouchingLayers(10))
        {
            print("0");
        }
    }
    
    void OnTriggerEnter2D (Collider2D other)
    {
        print(other);
    }
}
