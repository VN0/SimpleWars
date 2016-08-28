using UnityEngine;

public class NonExplodable : MonoBehaviour
{
    Explosion parentEx;
    bool firstTime = true;
    AnchoredJoint2D joint;

    void Awake ()
    {
        joint = GetComponent<AnchoredJoint2D>();
    }
    
    void FixedUpdate ()
    {
        if (firstTime)
        {
            firstTime = false;
            try
            {
                parentEx = joint.connectedBody.GetComponent<Explosion>();
            }
            catch (MissingComponentException) { }
            catch (System.NullReferenceException) { }
        }
        try
        {
            if (parentEx != null && joint.reactionForce.sqrMagnitude > Mathf.Pow(parentEx.forceToExplode * 3, 2))
            {
                parentEx.Explode();
            }
        }
        catch (MissingReferenceException) { }
    }
}
