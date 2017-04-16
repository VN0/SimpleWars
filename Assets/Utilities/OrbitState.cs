using UnityEngine;
using System.Collections;

//================================//
//===   Orbit State datatype   ===//
//================================//

/*
 The OrbitState is the initial state of the orbiter at a particular point along the ellipse
 The state contains all of the information necessary to apply a force to get the orbiter moving along the ellipse
*/

public class OrbitState : Object
{
    public Vector2 position; // local position relative to the object we're orbiting around
    public Vector2 normal;
    public Vector2 tangent;
    public Vector2 velocity;

    private Orbiter orbiter;
    private OrbitalEllipse ellipse;

    //==== Instance Methods ====//

    // Constructor
    public OrbitState (float angle, Orbiter orbiter, OrbitalEllipse ellipse)
    {
        Update(angle, orbiter, ellipse);
    }

    // Update the state of the orbiter when its position along the ellipse changes
    // Note: Make sure the ellipse is up to date before updating the orbit state
    public void Update (float orbiterAngle, Orbiter orbiter, OrbitalEllipse ellipse)
    {
        this.orbiter = orbiter;
        this.ellipse = ellipse;
        this.normal = CalcNormal(orbiterAngle);
        this.tangent = CalcTangent(normal);
        this.position = ellipse.GetPosition(orbiterAngle, orbiter.orbitAround.position);
        this.velocity = CalcVelocity(orbiter.orbitSpeed * orbiter.gravity, position, orbiter.orbitAround.position);
    }


    //==== Private Methods ====//

    // Returns the normal on the ellipse at the given angle
    // Assumes a vertical semi-major axis, and a rotation of 0 at the top of the ellipse, going clockwise
    private Vector3 CalcNormal (float rotationAngle)
    {
        // Part 1: Find the normal for the orbiter at its starting angle
        // Rotate an upward vector by the given starting angle around the ellipse. Gives us the normal for a circle.
        Vector3 localNormal = Quaternion.AngleAxis(rotationAngle, Vector3.forward * -1) * Vector3.up;
        // Sqash the normal into the shape of the ellipse
        localNormal.x *= ellipse.semiMajorAxis / ellipse.semiMinorAxis;

        // Part 2: Find the global rotation of the ellipse
        float ellipseAngle = Vector3.Angle(Vector3.up, ellipse.difference);
        if (ellipse.difference.x < 0)
            ellipseAngle = 360 - ellipseAngle; // Full 360 degrees, rather than doubling back after 180 degrees

        // Part 3: Rotate our normal to match the rotation of the ellipse
        Vector3 globalNormal = Quaternion.AngleAxis(ellipseAngle, Vector3.forward * -1) * localNormal;
        return globalNormal.normalized;
    }

    private Vector3 CalcTangent (Vector3 normal)
    {
        float angle = 90;
        int direction = orbiter.counterclockwise ? -1 : 1;
        var tangent = Quaternion.AngleAxis(angle * direction, Vector3.forward * -1) * normal;
        return tangent;
    }

    private Vector3 CalcVelocity (float gravity, Vector3 orbiterPos, Vector3 orbitAroundPos)
    {
        // Vis Viva equation
        float speed = Mathf.Sqrt(gravity * (2 / Vector3.Distance(orbiterPos, orbitAroundPos) - 1 / ellipse.semiMajorAxis));
        Vector3 velocityVec = tangent * speed;
        return velocityVec;
    }
}