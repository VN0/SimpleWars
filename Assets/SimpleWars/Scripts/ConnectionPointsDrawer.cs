using UnityEngine;
using System.Collections;
using EPPZ.Lines;

namespace SimpleWars
{
    public class ConnectionPointsDrawer : DirectLineRenderer
    {
        public Material mat;

        // Use this for initialization
        void Start ()
        {

        }

        // Update is called once per frame
        void Update ()
        {

        }

        protected override void OnDraw ()
        {
            DrawLine(new Vector2(-1, -1), new Vector2(1, 1), Color.red);
        }
    }

}