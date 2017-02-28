using UnityEngine;
using System.Collections.Generic;

namespace SimpleWars.Planets
{
    public class TerrainPiece : MonoBehaviour
    {
        List<Bezier> spline;
        List<Vector2> points;
        EdgeCollider2D col;
        Mesh mesh;
        MeshRenderer rend;

        Vector2 start;
        Vector2 end;


    }

    public class BezierTerrain : MonoBehaviour
    {
        List<TerrainPiece> terrains;
        int z;

        void Awake ()
        {

        }

        void Update ()
        {

        }
    }
}