using UnityEngine;
using System.Collections;

[AddComponentMenu("Hard Shell Studios/Examples/Player Move")]
public class Player_Move : MonoBehaviour {

    public bool canMove;
    public float moveSpeed;
    public float sprintMultiplier;
    public float jumpForce;
    float distToGround = 0.5f;

    Rigidbody rigid;

	// Use this for initialization
	void Start ()
    {
        rigid = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (canMove)
        {
            var localVel = transform.InverseTransformDirection(rigid.velocity);
            float speed = moveSpeed;

            //if (hardInput.GetKey("Sprint")) Use if you specifiy a sprint button
            //{
            //    speed *= sprintMultiplier;
            //}

            localVel = new Vector3((hardInput.GetAxis("Right", "Left", 7) + hardInput.GetAxis("ControllerLeftX", 7)) * speed, rigid.velocity.y, (hardInput.GetAxis("Forward", "Backward", 7) + hardInput.GetAxis("ControllerLeftY", 7)) * speed);


            rigid.velocity = transform.TransformDirection(localVel);
            if (hardInput.GetKeyDown("Jump") && IsGrouneded())
            {
                rigid.AddForceAtPosition(Vector3.up * jumpForce, Vector3.up);
            }
        }
	}

    bool IsGrouneded()
    {
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), -Vector3.up, Color.red);
        return Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), -Vector3.up, distToGround + 0.1f);
    }
}












