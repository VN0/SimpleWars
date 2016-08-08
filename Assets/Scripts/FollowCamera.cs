using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    public float interpVelocity;
    public float minDistance;
    public float followDistance;
    public float followSpeed = 1;
    public GameObject target;
    public Vector3 offset;
    Vector3 targetPos;
    bool gotTarget = false;
    // Use this for initialization
    void Start ()
    {
        targetPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        if (target)
        {
            Vector3 posNoZ = transform.position;
            posNoZ.z = target.transform.position.z;

            Vector3 targetDirection = (target.transform.position - posNoZ);

            interpVelocity = targetDirection.magnitude * 5f;

            targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime * followSpeed);

            transform.position = Vector3.Lerp(transform.position, targetPos + offset, 0.25f);

        }
        if (!gotTarget)
        {
            try
            {
                target = GameObject.FindGameObjectWithTag("Player");
                gotTarget = true;
            }
            catch { }
        }
    }
}