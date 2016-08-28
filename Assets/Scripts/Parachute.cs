using UnityEngine;

public class Parachute : PartFunction
{
    public float drag = 400;

    void Update ()
    {
        GetComponent<Rigidbody2D>().drag = drag;
        enabled = false;
    }
}
