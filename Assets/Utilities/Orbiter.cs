using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Orbiter : MonoBehaviour
{

    //==============================//
    //===        Orbiter         ===//
    //==============================//

    /*
      Required component. Add Orbiter.js to the object that you would like to put into orbit.

      Dependencies:
        OrbitalEllipse.js - calculates the shape, orientation, and offset of an orbit
        OrbitState.js - calculates the initial state of the orbiter
    */

    public Transform orbitAround;
    public float orbitSpeed = 10.0f; // In the original orbital equations this is gravity, not speed
    public float apsisDistance; // By default, this is the periapsis (closest point in its orbit)
    public float startingAngle = 0; // 0 = starting apsis, 90 = minor axis, 180 = ending apsis
    public float planetMass;
    public bool circularOrbit = false;
    public bool counterclockwise = false;
    [HideInInspector]
    public float gravity;

    private float gravityConstant = 6.67408E-11f;
    private Rigidbody2D rb;
    private Transform trans;
    private OrbitalEllipse ellipse;
    private OrbitState orbitState;

    // Accessor
    public OrbitalEllipse Ellipse ()
    {
        return ellipse;
    }

    public Transform Transform ()
    {
        return trans;
    }
    public float GravityConstant ()
    {
        return gravityConstant;
    }


    // Setup the orbit when the is added
    public void Reset ()
    {
        if (!orbitAround)
            return;
        ellipse = new OrbitalEllipse(orbitAround.position, transform.position, apsisDistance, circularOrbit);
        apsisDistance = ellipse.endingApsis; // Default to a circular orbit by setting both apses to the same value
    }
    void OnApplicationQuit ()
    {
        ellipse = new OrbitalEllipse(orbitAround.position, transform.position, apsisDistance, circularOrbit);
    }

    void OnDrawGizmosSelected ()
    {
        if (!orbitAround)
            return;
        // This is required for the OrbitRenderer. For some reason the ellipse var is always null
        // if it's set anywhere else, even including OnApplicationQuit;
        if (!ellipse)
            ellipse = new OrbitalEllipse(orbitAround.position, transform.position, apsisDistance, circularOrbit);
        // Never allow 0 apsis. Start with a circular orbit.
        if (apsisDistance == 0)
        {
            apsisDistance = ellipse.startingApsis;
        }
    }


    void Start ()
    {
        // Cache transform
        trans = transform;
        // Cache & set up rigidbody
        rb = GetComponent<Rigidbody2D>();
        //rb.drag = 0;
        rb.gravityScale = 0;

        // Bail out if we don't have an object to orbit around
        if (!orbitAround)
        {
            Debug.LogWarning("Satellite has no object to orbit around");
            return;
        }

        // Update the ellipse with initial value
        if (!ellipse)
            Reset();
        ellipse.Update(orbitAround.position, transform.position, apsisDistance, circularOrbit);

        // Calculate starting orbit state
        Vector2 difference = trans.position - orbitAround.position;
        gravity = gravityConstant * (planetMass * rb.mass / difference.sqrMagnitude);
        orbitState = new OrbitState(startingAngle, this, ellipse);

        // Position the orbiter
        trans.position = ellipse.GetPosition(startingAngle, orbitAround.position);

        // Add starting velocity
        rb.AddForce(orbitState.velocity * rb.mass, ForceMode2D.Impulse);
    }

    // Coroutine to apply gravitational forces on each fixed update to keep the object in orbit
    void FixedUpdate ()
    {
        // Debug.DrawLine(orbitState.position - orbitState.tangent*4, orbitState.position + orbitState.tangent*4);
        Vector2 difference = trans.position - orbitAround.position;
        gravity = gravityConstant * (planetMass * rb.mass / difference.sqrMagnitude);
        //print(gravity);
        rb.AddForce(-difference.normalized * gravity);
    }
}