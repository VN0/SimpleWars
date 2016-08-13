using UnityEngine;

public class Wheel : PartFunction
{
    WheelJoint2D whl;

    void Awake ()
    {
        //whl = GetComponent<WheelJoint2D>();
    }

    void Update ()
    {
        /*if (Input.GetKey(KeyCode.D))
        {
            whl.useMotor = true;
        }
        else
        {
            whl.useMotor = false;
        }*/
    }
}
