using System.Collections.Generic;
using UnityEngine;
using Noise;

namespace SimpleWars.Planets
{
    class Planet : MonoBehaviour
    {
        public List<TerrainChunk> chunks;

        public Material material;

        public float space = 0.5f;
        public float radius = 100;
        public float coefficient = 1;
        public float heightCoefficient = 1;
        public int chunksCount = 20;
        public int seed = 0;
        public Algorithm algorithm = Algorithm.Perlin;

        public enum Algorithm
        {
            Perlin = 0,
            Simplex = 1,
            Flat = 2
        }

        void Awake ()
        {
            System.Func<float, float> algo = Perlin.Generate;
            switch (algorithm)
            {
                case Algorithm.Perlin:
                    algo = Perlin.Generate;
                    break;
                case Algorithm.Simplex:
                    algo = Simplex.Generate;
                    break;
                case Algorithm.Flat:
                    algo = TerrainChunk.Flat;
                    break;
            }

            var rot = transform.rotation;
            chunks = new List<TerrainChunk>(chunksCount);
            for (int i = 0; i < chunksCount; i++)
            {
                var go = new GameObject("Chunk" + i.ToString());
                go.isStatic = true;
                go.transform.SetParent(transform);
                go.transform.localPosition = Vector2.zero;
                go.transform.localRotation = rot;
                var c = go.AddComponent<TerrainChunk>();
                c.material = material;
                c.space = space;
                c.radius = radius;
                c.coefficient = coefficient;
                c.heightCoefficient = heightCoefficient;
                c.seed = seed;
                c.start = 360f / chunksCount * i;
                c.end = 360f / chunksCount * (i + 1);
                c.Generate(algo);
            }
        }
    }
}