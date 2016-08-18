using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    public float interpVelocity;
    public float minDistance;
    public float followDistance;
    public float followSpeed = 1;
    public Transform target;
    public Vector3 offset;
    Vector3 targetPos;


    void Start ()
    {
        targetPos = transform.position;
    }
    

    void FixedUpdate ()
    {
        if (target)
        {
            /*Vector3 posNoZ = transform.position;
            posNoZ.z = target.position.z;

            Vector3 targetDirection = (target.position - posNoZ);

            interpVelocity = targetDirection.magnitude * 5f;

            targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime * followSpeed);

            transform.position = Vector3.Lerp(transform.position, targetPos + offset, 0.25f);*/
            transform.position = new Vector3(target.position.x, target.position.y, -10);

        }
        else
        {
            try
            {
                target = GameObject.FindGameObjectWithTag("Player").transform;
            }
            catch { }
        }
    }
}