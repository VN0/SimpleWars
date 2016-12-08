using UnityEngine;

namespace SimpleWars.Parts
{
    public class Explosive : MonoBehaviour
    {
        public GameObject explosion;
        public float rate = 1f;
        public float size = 10f;
        public float force = 500f;
        public int delay = 10;
        public int forceToExplode = 100;
        float cRadius = 0f;

        bool exploded = false;
        CircleCollider2D radius;
        PointEffector2D exploder;
        float mass;
        GameObject ex;
        Explosive parentEx;
        bool firstTime = true;
        AnchoredJoint2D joint;


        public void Explode ()
        {
            Unbug.SafeDestroy(GetComponent<SpriteRenderer>());
            Unbug.SafeDestroy(GetComponent<Collider2D>());
            Unbug.SafeDestroy(GetComponent<Joint2D>());
            Unbug.SafeDestroy(GetComponent<PartFunction>());
            Unbug.SafeDestroy(GetComponent<Rigidbody2D>());
            //tr.DetachChildren ();
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                transform.GetChild(i).transform.SetParent(transform.root);
            }
            exploded = true;

            radius = gameObject.AddComponent<CircleCollider2D>();
            radius.isTrigger = true;
            radius.usedByEffector = true;
            exploder = gameObject.AddComponent<PointEffector2D>();
            if (exploder == null)
            {
                exploder = GetComponent<PointEffector2D>();
            }
            exploder.forceMagnitude = force;
            exploder.forceMode = EffectorForceMode2D.InverseLinear;
            exploder.forceTarget = EffectorSelection2D.Collider;
            exploder.forceSource = EffectorSelection2D.Collider;
            exploder.forceVariation = force / 5;
            ex = Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360))) as GameObject;
            ex.transform.SetParent(transform);
        }

        void Awake ()
        {
            mass = gameObject.GetComponent<Rigidbody2D>().mass;
            if (rate == 0f)
            {
                rate = size;
            }
            joint = GetComponent<AnchoredJoint2D>();
        }


        void Update ()
        {
            if (!exploded && Input.GetKeyDown(KeyCode.E))
            {
                Explode();
            }
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
            if (exploded == true)
            {
                if (cRadius < size)
                {
                    cRadius += rate;
                    radius.radius = cRadius;
                }
                else if (delay > 0)
                {
                    delay -= 1;
                }
                else if (!ex.GetComponent<ParticleSystem>().IsAlive() && !ex.GetComponent<AudioSource>().isPlaying)
                {
                    //Lean.LeanPool.Despawn(ex);
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(GetComponent<PointEffector2D>());
                }
            }
            try
            {
                float reactionForce = joint.reactionForce.sqrMagnitude;
                if (reactionForce > Mathf.Pow(forceToExplode * 3, 2))
                {
                    Explode();
                }
                if (parentEx != null && reactionForce > Mathf.Pow(parentEx.forceToExplode * 3, 2))
                {
                    parentEx.Explode();
                }
            }
            catch (MissingReferenceException) { }
            catch (System.NullReferenceException) { }
        }

        void OnCollisionEnter2D (Collision2D col)
        {
            float v = col.relativeVelocity.sqrMagnitude;
            if (exploded)
            {
                return;
            }
            if (v * (mass + (col.rigidbody != null ? col.rigidbody.mass : 0)) > forceToExplode * 2)
            {
                Explode();
                return;
            }
        }
    }
}