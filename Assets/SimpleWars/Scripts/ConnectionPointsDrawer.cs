using UnityEngine;
using System.Collections.Generic;

namespace SimpleWars
{
    public class ConnectionPointsDrawer : MonoBehaviour
    {
        // Please assign a material that is using position and color.
        public Material material;
        public Vector2 from, to;
        public float width;
        public Color color = Color.white;
        Color lastColor;
        Camera cam;
        List<Vector2> points;
        float screenHeight;

        public struct Quad
        {
            public Vector2[] pts
            {
                get;
                private set;
            }

            public Quad (IEnumerable<Vector2> points)
            {
                pts = new Vector2[4];
                SetPoints(points);
            }

            public void SetPoint (Vector2 value, int pos)
            {
                pts[pos] = value;
            }

            public void SetPoints (IEnumerable<Vector2> points)
            {
                using (var penum = points.GetEnumerator())
                {
                    for (int i = 0; i <= 4; i++)
                    {
                        pts[i] = penum.Current;
                    }
                }
            }
        }

        private void Awake ()
        {
            cam = Camera.main;
            screenHeight = Screen.height;
        }

        private void Update ()
        {
            var _from = cam.WorldToScreenPoint(from);
            var _to = cam.WorldToScreenPoint(to);
            var _width = cam.WorldToScreenPoint(new Vector2(0, width)).y;
            _width -= cam.WorldToScreenPoint(Vector2.zero).y;
            var rot = AngleInDeg(from, to) + 90;
            var posw = (Vector3)Vector2FromAngle(rot) * _width / 2;
            var negw = (Vector3)Vector2FromAngle(rot) * (-_width / 2);
            var pts = new List<Vector2>();
            pts.Add(posw + _from);
            pts.Add(negw + _from);
            pts.Add(negw + _to);
            pts.Add(posw + _to);
            points = pts;
            Debug.LogFormat("{0}, {1}, {2}, {3}", pts[0], pts[1], pts[2], pts[3]);
        }

        void OnGUI ()
        {
            if (color != lastColor)
            {
                material = new Material(material);
                material.color = color;
                lastColor = color;
            }
            DrawRectangle(points, color);
        }

        void DrawRectangle (IEnumerable<Vector2> positions, Color color)
        {
            // We shouldn't draw until we are told to do so.
            if (Event.current.type != EventType.Repaint)
                return;

            // Please assign a material that is using position and color.
            if (material == null)
            {
                Debug.LogError("You have forgot to set a material.");
                return;
            }

            material.SetPass(0);

            // Optimization hint: 
            // Consider Graphics.DrawMeshNow
            GL.Color(color);
            GL.Begin(GL.QUADS);
            //GL.Vertex3(position.x, position.y, 0);
            //GL.Vertex3(position.x + position.width, position.y, 0);
            //GL.Vertex3(position.x + position.width, position.y + position.height, 0);
            //GL.Vertex3(position.x, position.y + position.height, 0);
            foreach (Vector2 v in positions)
            {
                GL.Vertex3(v.x, screenHeight - v.y, 0);
            }

            GL.End();
        }

        public Vector2 Vector2FromAngle (float a)
        {
            a *= Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
        }

        public static float AngleInRad (Vector2 vec1, Vector2 vec2)
        {
            return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
        }

        public static float AngleInDeg (Vector2 vec1, Vector2 vec2)
        {
            return AngleInRad(vec1, vec2) * 180 / Mathf.PI;
        }

    }
}