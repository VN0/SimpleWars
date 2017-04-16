using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Orbiter))]
[ExecuteInEditMode]
public class OrbitalRenderer : MonoBehaviour
{

    //===============================//
    //===     Orbit Renderer      ===//
    //===============================//

    /*
      Optional component. Display the Orbiter component's properties in the editor. Does nothing in-game.
    */

    Color orbitPointsColor = new Color(1, 1, 0, 0.5f); // Yellow
    float orbitPointsSize = 0.5f;
    float ellipseResolution = 24;
    //bool  renderAsLines = false;

    Color startPointColor = new Color(1, 0, 0, 0.7f); // Red
    float startPointSize = 1.0f;

    private Orbiter orbiter;
    private OrbitalEllipse ellipse;

    void Awake ()
    {
        // Remove the component in the compiled game. Likely not a noticeable optimization, just an experiment.
        if (!Application.isEditor)
            Destroy(this);
    }

    void Reset ()
    {
        orbiter = GetComponent<Orbiter>();
    }
    void OnApplicationQuit ()
    {
        orbiter = GetComponent<Orbiter>();
    }


    void OnDrawGizmosSelected ()
    {
        if (!orbiter)
            orbiter = GetComponent<Orbiter>();

        // Bail out if there is no object to orbit around
        if (!orbiter.orbitAround)
            return;

        // Recalculate the ellipse only when in the editor
        if (!Application.isPlaying)
        {
            if (!orbiter.Ellipse())
                return;
            orbiter.Ellipse().Update(orbiter.orbitAround.position, transform.position, orbiter.apsisDistance, orbiter.circularOrbit);
        }

        DrawEllipse();
        DrawStartingPosition();
    }

    void DrawEllipse ()
    {
        for (float angle = 0; angle < 360; angle += 360 / ellipseResolution)
        {
            Gizmos.color = orbitPointsColor;
            Gizmos.DrawSphere(orbiter.Ellipse().GetPosition(angle, orbiter.orbitAround.position), orbitPointsSize);
        }
    }

    void DrawStartingPosition ()
    {
        Gizmos.color = startPointColor;
        Gizmos.DrawSphere(orbiter.Ellipse().GetPosition(orbiter.startingAngle, orbiter.orbitAround.position), startPointSize);
    }
}
