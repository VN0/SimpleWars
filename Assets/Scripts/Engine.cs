using UnityEngine;

public static class VectorExtension
{
    public static Vector3 Rotate (this Vector3 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
    public static Vector2 Rotate (this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}

public class Engine : PartFunction
{
    public float force = 200;
    public float turn = 60;
    public float consumption = 1;
    public SpriteRenderer flame;
    public ParticleSystem smoke;
    public AudioSource sound;
    //float rotation = 0;
    bool active;
    Tank tank;
    FuelControl controller;
    float smokeMax;
    float smokeSpeed;
    Rigidbody2D rb;

    void Awake ()
    {
        smokeMax = smoke.startSpeed;
        rb = GetComponent<Rigidbody2D>();
    }

    void Start ()
    {
        controller = FindObjectOfType<FuelControl>();
        try
        {
            tank = transform.GetComponentInParent<Tank>();
            if (tank.fuel > 0)
            {
                active = true;
            }
        }
        catch (System.NullReferenceException)
        {
            Destroy(flame);
            Destroy(this);
        }
    }

    void Update ()
    {
        if (active)
        {
            if (controller.currentForce > 0.01)
            {
                flame.enabled = true;
                if (!smoke.isPlaying)
                {
                    smoke.Play();
                    sound.Play();
                }
            }
            else
            {
                flame.enabled = false;
                if (smoke.isPlaying)
                {
                    smoke.Stop();
                    sound.Stop();
                }
            }
        }
        else
        {
            enabled = false;
            Destroy(flame);
            if (smoke.isPlaying)
            {
                smoke.Stop();
                sound.Stop();
            }
        }
    }

    void OnDestroy ()
    {
        Destroy(smoke);
        if(flame != null)
        {
            Destroy(flame);
        }
    }

    void FixedUpdate ()
    {
        float allowedForce = controller.currentForce;
        smoke.startSpeed = smokeMax * allowedForce;
        if (allowedForce > 0.01)
        {
            try
            {
                active = tank.Consume(consumption * Time.deltaTime * allowedForce);
            }
            catch (System.NullReferenceException)
            {
                active = false;
            }
            rb.AddForce(transform.up * force * Time.deltaTime * allowedForce);

            if (Input.GetKey(KeyCode.A))
            {
                rb.AddTorque(turn * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddTorque(-turn * Time.deltaTime);
            }
            else
            {
                
            }
        }
    }
}
