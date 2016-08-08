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

    void Awake ()
    {
        smokeMax = smoke.startSpeed;
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
            if (Input.GetKey(KeyCode.W))
            {
                flame.enabled = true;
                if (!smoke.isPlaying)
                {
                    smoke.Play();
                    sound.Play();
                }
            }
            if (Input.GetKeyUp(KeyCode.W))
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
        if (Input.GetKey(KeyCode.W) && active)
        {
            try
            {
                active = tank.Consume(consumption * Time.deltaTime * allowedForce);
            }
            catch (System.NullReferenceException)
            {
                active = false;
            }
            GetComponent<Rigidbody2D>().AddForce(transform.up * force * Time.deltaTime * allowedForce);

            if (Input.GetKey(KeyCode.A))
            {
                //rotation = -turn;
                GetComponent<Rigidbody2D>().AddTorque(turn * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                //rotation = turn;
                GetComponent<Rigidbody2D>().AddTorque(-turn * Time.deltaTime);
            }
            else
            {
                //rotation = 0;
            }
        }
    }
}
