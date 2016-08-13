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


    void Explode ()
    {
        try
        {
            Destroy(GetComponent<SpriteRenderer>());
            Destroy(GetComponent<Collider2D>());
            Destroy(GetComponent<Joint2D>());
            Destroy(GetComponent<PartFunction>());
            Destroy(GetComponent<Rigidbody2D>());
        }
        catch (System.NullReferenceException) { }
        Transform tr = gameObject.transform;
        //tr.DetachChildren ();
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            tr.GetChild(i).transform.SetParent(tr.parent);
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
        }
    }

    void OnCollisionEnter2D (Collision2D col)
    {
        float v = col.relativeVelocity.magnitude;
        if (v * Mathf.Pow(mass + (col.rigidbody != null ? col.rigidbody.mass : mass), 2) / 2 > forceToExplode)
        {
            Explode();
        }
    }
}