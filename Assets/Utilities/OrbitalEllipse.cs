using UnityEngine;
using System.Collections;

//===================================//
//===  Elliptical orbit datatype  ===//
//===================================//

/*
  Calculates an ellipse to use as an orbital path
*/

public class OrbitalEllipse : Object
{

    // "Starting" apsis is the position of the transform.position of the orbiter.
    // "Ending" apsis is the distance that we've defined in the inspector.
    // Each apsis defines the distance from the object we're orbiting to the orbiter
    public float startingApsis;
    public float endingApsis;

    public float semiMajorAxis;
    public float semiMinorAxis;
    public float focalDistance;
    public Vector3 difference; // difference between the object we're orbiting and the orbiter


    //==== Instance Methods ====//

    // Constructor
    public OrbitalEllipse (Vector3 orbitAroundPos, Vector3 orbiterPos, float endingApsis, bool circular)
    {
        Update(orbitAroundPos, orbiterPos, endingApsis, circular);
    }

    // Update ellipse when orbiter properties change
    public void Update (Vector3 orbitAroundPos, Vector3 orbiterPos, float endingApsis, bool circular)
    {
        this.difference = orbiterPos - orbitAroundPos;
        this.startingApsis = difference.magnitude;
        if (endingApsis == 0 || circular)
            this.endingApsis = this.startingApsis;
        else
            this.endingApsis = endingApsis;
        this.semiMajorAxis = CalcSemiMajorAxis(this.startingApsis, this.endingApsis);
        this.focalDistance = CalcFocalDistance(this.semiMajorAxis, this.endingApsis);
        this.semiMinorAxis = CalcSemiMinorAxis(this.semiMajorAxis, this.focalDistance);
    }

    // The global position
    public Vector3 GetPosition (float degrees, Vector3 orbitAroundPos)
    {
        // Use the difference between the orbiter and the object it's orbiting around to determine the direction
        // that the ellipse is aimed
        // Angle is given in degrees
        float ellipseDirection = Vector3.Angle(Vector3.left, difference); // the direction the ellipse is rotated
        if (difference.y < 0)
        {
            ellipseDirection = 360 - ellipseDirection; // Full 360 degrees, rather than doubling back after 180 degrees
        }

        float beta = ellipseDirection * Mathf.Deg2Rad;
        float sinBeta = Mathf.Sin(beta);
        float cosBeta = Mathf.Cos(beta);

        var alpha = degrees * Mathf.Deg2Rad;
        var sinalpha = Mathf.Sin(alpha);
        var cosalpha = Mathf.Cos(alpha);

        // Position the ellipse relative to the "orbit around" transform
        Vector3 ellipseOffset = difference.normalized * (semiMajorAxis - endingApsis);

        Vector3 finalPosition = new Vector3();
        finalPosition.x = ellipseOffset.x + (semiMajorAxis * cosalpha * cosBeta - semiMinorAxis * sinalpha * sinBeta) * -1;
        finalPosition.y = ellipseOffset.y + (semiMajorAxis * cosalpha * sinBeta + semiMinorAxis * sinalpha * cosBeta);

        // Offset entire ellipse proportional to the position of the object we're orbiting around
        finalPosition += orbitAroundPos;

        return finalPosition;
    }


    //==== Private Methods ====//

    private float CalcSemiMajorAxis (float startingApsis, float endingApsis)
    {
        return (startingApsis + endingApsis) * 0.5f;
    }
    private float CalcSemiMinorAxis (float semiMajorAxis, float focalDistance)
    {
        float distA = semiMajorAxis + focalDistance * 0.5f;
        float distB = semiMajorAxis - focalDistance * 0.5f;
        return Mathf.Sqrt(Mathf.Pow(distA + distB, 2) - focalDistance * focalDistance) * 0.5f;
    }
    // private float CalcEccentricity ( float semiMajorAxis ,   float focalDistance  ){
    // 	return focalDistance / (semiMajorAxis * 2);
    // }
    private float CalcFocalDistance (float semiMajorAxis, float endingApsis)
    {
        return (semiMajorAxis - endingApsis) * 2;
    }
}