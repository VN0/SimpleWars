using UnityEngine;

public class Detacher : PartFunction
{
    void Update ()
    {
        try
        {
            Destroy(GetComponent<AnchoredJoint2D>());
        }
        catch (System.NullReferenceException)
        {
            Debug.LogWarningFormat("Unable to destroy the joint of {0}", gameObject);
        }
        transform.SetParent(GameObject.Find("Vehicle").transform);
        GetComponent<Rigidbody2D>().AddForce(-transform.up * 5);
        enabled = false;
    }
}
