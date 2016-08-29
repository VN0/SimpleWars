using UnityEngine;
using UnityEngine.UI;

public static class VectorExtension
{
    public static Vector3 Rotate (this Vector3 v, float degrees)
    {
        return Quaternion.Euler(0, 0, degrees) * v;
    }
    public static Vector2 Rotate (this Vector2 v, float degrees)
    {
        return Quaternion.Euler(0, 0, degrees) * v;
    }
}


public class Engine : PartFunction
{
    public float force = 200;
    public float turnAngle = 10;
    public float consumption = 1;
    public SpriteRenderer flame;
    public ParticleSystem smoke;
    public AudioSource sound;

    float rotation = 0;
    bool active;
    Tank tank;
    FuelControl controller;
    float smokeMax;
    float dragMax;
    float smokeSpeed;
    ParticleSystem.ForceOverLifetimeModule drag;
    Rigidbody2D rb;
    Button leftButton;
    Button rightButton;

    void Awake ()
    {
        smokeMax = smoke.startSpeed;
        dragMax = smoke.forceOverLifetime.z.constant;
        drag = smoke.forceOverLifetime;
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
        if (smoke.isPlaying)
        {
            smoke.Stop();
            sound.Stop();
        }
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
        drag.z = new ParticleSystem.MinMaxCurve(dragMax * allowedForce);
        if (allowedForce > 0.01 && active)
        {
            try
            {
                active = tank.Consume(consumption * Time.deltaTime * allowedForce);
            }
            catch (System.NullReferenceException)
            {
                active = false;
            }
            rb.AddForce(transform.up.Rotate(rotation) * force * Time.deltaTime * allowedForce);

            if (controller.turnLeft)
            {
                rotation = -turnAngle;
                flame.transform.localRotation = Quaternion.Euler(0, 0, rotation);
            }
            else if (controller.turnRight)
            {
                rotation = turnAngle;
                flame.transform.localRotation = Quaternion.Euler(0, 0, rotation);
            }
            else
            {
                rotation = 0;
                flame.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
