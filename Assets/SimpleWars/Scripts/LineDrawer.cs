using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace SimpleWars
{
    public class LineDrawer : Singleton<LineDrawer>
    {
        // Please assign a material that is using position and color.
        public static List<Quad> quads = new List<Quad>();
        public Material material;
        public bool clearOnSceneUnloaded = true;
        public bool noCache = false;
        Camera cam;
        float screenHeight;

        [System.Serializable]
        public class Quad
        {
            public Vector2[] pts = new Vector2[4];
            public Vector2 offset;
            public Color color = Color.white;
            public bool onScreen;

            public Quad (Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Color? color = null, bool screenSpace = false, Vector2? offset = null)
            {
                pts[0] = p0;
                pts[1] = p1;
                pts[2] = p2;
                pts[3] = p3;
                onScreen = screenSpace;
                this.offset = offset ?? Vector2.zero;
                this.color = color ?? Color.white;
            }

            public Quad (Vector2 start, Vector2 end, float width = 1, Color? color = null, bool screenSpace = false, Vector2? offset = null)
            {
                FromLine(start, end, width);
                onScreen = screenSpace;
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

        protected override void Initialize ()
        {
            useGUILayout = false;
            Refresh();
            if (clearOnSceneUnloaded)
            {
                SceneManager.sceneUnloaded += delegate (Scene s)
                {
                    quads.Clear();
                };
            }
            SceneManager.sceneLoaded += delegate (Scene s, LoadSceneMode mode)
            {
                cam = Camera.main;
            };
        }

        void OnGUI ()
        {
            //if (color != lastColor)
            //{
            //    material = new Material(material);
            //    material.color = color;
            //    lastColor = color;
            //}
            DrawQuads(quads);
            if (noCache)
            {
                quads.Clear();
            }
        }

        void DrawQuads (IEnumerable<Quad> quads)
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
            foreach (Quad quad in quads)
            {
                GL.Color(quad.color);
                var positions = quad.pts;
                var offset = quad.offset;
                foreach (Vector2 v in positions)
                {
                    Vector2 vec;
                    if (!quad.onScreen)
                    {
                        vec = cam.WorldToScreenPoint(v + offset);
                    }
                    else
                    {
                        vec = v + offset;
                    }
                    GL.Vertex3(vec.x, screenHeight - vec.y, 0);
                }
            }

            GL.End();
        }

    }
}