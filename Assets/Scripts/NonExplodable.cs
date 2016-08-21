using UnityEngine;

public class NonExplodable : MonoBehaviour
{
    Explosion parentEx;
    
    void Start ()
    {
        try
        {
            parentEx = transform.parent.GetComponent<Explosion>();
        }
        catch (MissingComponentException) { }
    }
    
    void FixedUpdate ()
    {
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
