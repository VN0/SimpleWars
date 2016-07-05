using UnityEngine;

public class DirectionalDrag : MonoBehaviour {

    public Vector2 drag;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
	
	void Update()
    {
        Vector2 localVel = transform.InverseTransformDirection(rb.velocity);
        rb.AddForce(
            transform.TransformDirection(
                new Vector2(-localVel.x  * drag.x * Time.deltaTime ,-localVel.y  * drag.y * Time.deltaTime)
            )
        );
	}
}
