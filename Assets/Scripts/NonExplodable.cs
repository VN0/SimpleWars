using UnityEngine;

public class NonExplodable : MonoBehaviour
{
    Explosion parentEx;
    bool firstTime = true;
    
    void FixedUpdate ()
    {
        if (firstTime)
        {
            firstTime = false;
            try
            {
                parentEx = transform.parent.GetComponent<Explosion>();
            }
            catch (MissingComponentException) { }
        }
        try
        {
            if (parentEx != null && GetComponent<AnchoredJoint2D>().reactionForce.sqrMagnitude > Mathf.Pow(parentEx.forceToExplode * 2, 2))
            {
                parentEx.Explode();
            }
        }
        catch (MissingComponentException) { }
    }
}
