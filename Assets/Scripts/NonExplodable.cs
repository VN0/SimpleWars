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
                parentEx = GetComponent<AnchoredJoint2D>().connectedBody.GetComponent<Explosion>();
            }
            catch (MissingComponentException) { }
            catch (System.NullReferenceException) { }
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
