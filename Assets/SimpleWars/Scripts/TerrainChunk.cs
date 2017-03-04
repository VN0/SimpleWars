using UnityEngine;
using System.Collections.Generic;
using Noise;

namespace SimpleWars.Planets
{
    public class TerrainChunk : MonoBehaviour
    {
        public Material material;
        public List<Vector2> points;
        public EdgeCollider2D col;
        public Mesh mesh;
        public MeshFilter filter;
        public MeshRenderer rend;

        public float space = 0.5f;
        public float start = 0;
        public float end = 360;
        public float radius = 100;
        public float coefficient = 1;
        public float heightCoefficient = 1;
        public int seed = 0;
        public Algorithm algorithm = Algorithm.Perlin;

        public enum Algorithm
        {
            Perlin = 0,
            Simplex = 1
        }

        private void Awake ()
        {
            if (seed != 0)
            {
                Simplex.Seed = seed;
            }

            System.Func<float, float> algo = Perlin.Generate;
            switch (algorithm)
            {
                case Algorithm.Perlin:
                    algo = Perlin.Generate;
                    break;
                case Algorithm.Simplex:
                    algo = Simplex.Generate;
                    break;
            }

            float distPerDeg = radius * 2 * Mathf.PI / 360;

            for (float i = start; i < end; i += space)
            {
                var angle = Vector2FromAngle(i + 90);
                points.Add(angle * (algo(i * distPerDeg * coefficient) * heightCoefficient + radius));
            }
            var _angle = Vector2FromAngle(end + 90);
            points.Add(_angle * (algo(end * distPerDeg * coefficient) * heightCoefficient + radius));

            col = gameObject.GetOrAddComponent<EdgeCollider2D>();
            col.points = points.ToArray();

            int size = points.Count + 1;
            //points.Add(transform.position);
            //Triangulator tr = new Triangulator(points.ToArray());
            //int[] indices = tr.Triangulate();
            //points.RemoveAt(size - 1);
            int[] indices = Triangulate(points.Count);
            Vector3[] vertices = new Vector3[size];
            for (int i = 0; i < size - 1; i++)
            {
                vertices[i] = (points[i]);
            }
            vertices[size - 1] = (transform.position);
            mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = indices;
            points.Add(transform.position);
            mesh.SetUVs(0, points);
            points.RemoveAt(size - 1);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            filter = gameObject.GetOrAddComponent<MeshFilter>();
            filter.sharedMesh = mesh;
            rend = gameObject.GetOrAddComponent<MeshRenderer>();
            rend.material = material;
            rend.receiveShadows = false;
            rend.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            rend.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            rend.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        public static Vector2 Vector2FromAngle (float a)
        {
            a *= Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
        }

        public static int[] Triangulate (int size)
        {
            var lst = new List<int>(size * 3);

            for (int i = 0; i < size - 1; i++)
            {
                lst.Add(i);
                lst.Add(size);
                lst.Add(i + 1);
            }

            return lst.ToArray();
        }
    }
}