using UnityEngine;
using System.Collections.Generic;

namespace SimpleWars
{
    public class LineDrawer : MonoBehaviour
    {
        // Please assign a material that is using position and color.
        public Material material;
        public List<Quad> quads = new List<Quad>();
        Camera cam;
        float screenHeight;

        [System.Serializable]
        public class Quad
        {
            public Vector2[] pts = new Vector2[4];
            public Vector2 offset;
            public Color color = Color.white;

            public Quad (Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Color? color = null, Vector2? offset = null)
            {
                pts[0] = p0;
                pts[1] = p1;
                pts[2] = p2;
                pts[3] = p3;
                this.offset = offset ?? Vector2.zero;
                this.color = color ?? Color.white;
            }

            public Quad (Vector2 start, Vector2 end, float width = 1, Color? color = null, Vector2? offset = null)
            {
                FromLine(start, end, width);
                this.offset = offset ?? Vector2.zero;
                this.color = color ?? Color.white;
            }

            public void FromLine (Vector2 start, Vector2 end, float width = 1)
            {
                var rot = AngleInDeg(start, end) + 90;
                var posw = Vector2FromAngle(rot) * width / 2;
                var negw = -posw;
                pts[0] = posw + start;
                pts[1] = negw + start;
                pts[2] = negw + end;
                pts[3] = posw + end;
            }

            public static Vector2 Vector2FromAngle (float a)
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

        public Quad AddQuad (Vector2 start, Vector2 end, float width = 1, Color? color = null)
        {
            var q = new Quad(start, end, width, color);
            quads.Add(q);
            return q;
        }

        public void Refresh ()
        {
            cam = Camera.main;
            screenHeight = Screen.height;
        }

        private void Awake ()
        {
            Refresh();
        }

        void OnGUI ()
        {
            //if (color != lastColor)
            //{
            //    material = new Material(material);
            //    material.color = color;
            //    lastColor = color;
            //}
            foreach (var quad in quads)
            {
                DrawRectangle(quad.pts, quad.offset, quad.color);
            }
        }

        void DrawRectangle (IEnumerable<Vector2> positions, Vector2 offset, Color color)
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
            GL.Begin(GL.QUADS);
            GL.Color(color);
            foreach (Vector2 v in positions)
            {
                var vec = cam.WorldToScreenPoint(v + offset);
                GL.Vertex3(vec.x, screenHeight - vec.y, 0);
            }

            GL.End();
        }

    }
}