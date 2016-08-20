using UnityEngine;

public class Wheel : PartFunction
{
    public float force = 200;
    FuelControl controller;
    Rigidbody2D rb;

    void Start ()
    {
        controller = FindObjectOfType<FuelControl>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update ()
    {
        if (controller.turnLeft)
        {
            //rb.AddTorque(force);
        }
        else if (controller.turnRight)
        {
            //rb.AddTorque(-force);
        }
    }
}
