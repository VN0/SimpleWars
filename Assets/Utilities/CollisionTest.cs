using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    public GameObject a;
    public int speedA;
    public int speedB;

    void Start()
    {
        //Time.timeScale = 0.1f;
        a.GetComponent<Rigidbody>().AddForce(new Vector3(speedA, 0, 0), ForceMode.VelocityChange);
        GetComponent<Rigidbody>().AddForce(new Vector3(-speedB, 0, 0), ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void OnCollisionStay (Collision col)
    {
        Debug.LogFormat("impulse: {0} , relativeVelocity: {1}", col.impulse.magnitude, col.relativeVelocity.magnitude);
    }
}
