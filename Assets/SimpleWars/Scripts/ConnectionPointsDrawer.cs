using UnityEngine;
using System.Collections;

namespace SimpleWars
{
    public class ConnectionPointsDrawer : MonoBehaviour
    {

        // Use this for initialization
        void Start ()
        {

        }

        // Update is called once per frame
        void Update ()
        {

        }

        void OnPostRender ()
        {
            GL.LoadOrtho();
            GL.Begin(GL.QUADS);
            GL.Color(Color.red);
            GL.Vertex3(0, 0.5F, 0);
            GL.Vertex3(0.5F, 1, 0);
            GL.Vertex3(1, 0.5F, 0);
            GL.Vertex3(0.5F, 0, 0);
            GL.Color(Color.cyan);
        }
    }

}