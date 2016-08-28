using UnityEngine;

public class Explosion : MonoBehaviour
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
    Explosion parentEx;
    bool firstTime = true;


    public void Explode ()
    {
        try
        {
            Destroy(GetComponent<SpriteRenderer>());
        }
        catch (System.NullReferenceException) { }
        try
        {
            Destroy(GetComponent<Collider2D>());
        }
        catch (System.NullReferenceException) { }
        try
        {
            Destroy(GetComponent<Joint2D>());
        }
        catch (System.NullReferenceException) { }
        try
        {
            Destroy(GetComponent<PartFunction>());
        }
        catch (System.NullReferenceException) { }
        try
        {
            Destroy(GetComponent<Rigidbody2D>());
        }
        catch (System.NullReferenceException) { }
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
                parentEx = GetComponent<AnchoredJoint2D>().connectedBody.GetComponent<Explosion>();
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
                Destroy(gameObject);
            }
            else
            {
                Destroy(GetComponent<PointEffector2D>());
            }
        }
        try
        {
            float reactionForce = GetComponent<AnchoredJoint2D>().reactionForce.sqrMagnitude;
            if (reactionForce > Mathf.Pow(forceToExplode * 2, 2))
            {
                Explode();
            }
            if (parentEx != null && reactionForce > Mathf.Pow(parentEx.forceToExplode * 2, 2))
            {
                parentEx.Explode();
            }
        }
        catch (MissingComponentException) { }
    }

    void OnCollisionEnter2D (Collision2D col)
    {
        //相对速度
        float v = col.relativeVelocity.magnitude;
        if (exploded)
        {
            return;
        }
        //满足自身爆炸条件
        if (v * Mathf.Pow(mass + (col.rigidbody != null ? col.rigidbody.mass : 5), 2) / 2 > forceToExplode)
        {
            Explode();
            return;
        }
    }
}