using UnityEngine;

namespace SimpleWars
{
    [RequireComponent(typeof(Camera))]
    public class GradientBackground : MonoBehaviour
    {
        public Color topColor = Color.blue;
        public Color bottomColor = Color.white;
        public Shader shader;
        public int gradientLayer = 7;

        void Awake ()
        {
            Camera camera = this.gameObject.GetComponent<Camera>();
            gradientLayer = Mathf.Clamp(gradientLayer, 0, 31);
            if (!camera)
            {
                Debug.LogError("Must attach GradientBackground script to the camera");
                return;
            }

            camera.clearFlags = CameraClearFlags.Depth;
            camera.cullingMask = camera.cullingMask & ~(1 << gradientLayer);
            Camera gradientCam = new GameObject("Gradient Cam", typeof(Camera)).GetComponent<Camera>();
            gradientCam.depth = camera.depth - 1;
            gradientCam.cullingMask = 1 << gradientLayer;

            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[4]
            {new Vector3(-100f, .577f, 1f), new Vector3(100f, .577f, 1f), new Vector3(-100f, -.577f, 1f), new Vector3(100f, -.577f, 1f)};

            mesh.colors = new Color[4] { topColor, topColor, bottomColor, bottomColor };

            mesh.triangles = new int[6] { 0, 1, 2, 1, 3, 2 };

            Material mat = new Material(shader);

            GameObject gradientPlane = new GameObject("Gradient Plane", typeof(MeshFilter), typeof(MeshRenderer));

            ((MeshFilter)gradientPlane.GetComponent(typeof(MeshFilter))).mesh = mesh;
            Renderer gradRend = gradientPlane.GetComponent<Renderer>();
            gradRend.material = mat;
            gradientPlane.layer = gradientLayer;
        }
    }
}