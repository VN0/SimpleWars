using UnityEngine;

[System.Serializable]
public class Bezier : Object
{
    public Vector2 v0, v1, v2, v3;

    public Bezier (Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        v0 = p0;
        v1 = p1;
        v2 = p2;
        v3 = p3;
    }

    public Vector2 GetPoint (float t)
    {
        return GetPointS(v0, v1, v2, v3, t);
    }

    public static Vector2 GetPointS (Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * oneMinusT * p0 +
            3f * oneMinusT * oneMinusT * t * p1 +
            3f * oneMinusT * t * t * p2 +
            t * t * t * p3;
    }
}