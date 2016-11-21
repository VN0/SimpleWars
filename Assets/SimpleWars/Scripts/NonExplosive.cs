using UnityEngine;

namespace SimpleWars
{
    public class NonExplosive : MonoBehaviour
    {
        Explosive parentEx;
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
                    parentEx = joint.connectedBody.GetComponent<Explosive>();
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
            catch (System.NullReferenceException) { }
        }
    }
}
