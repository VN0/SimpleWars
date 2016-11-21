using UnityEngine;

namespace SimpleWars
{
    [RequireComponent(typeof(Camera))]
    public class SkyboxRotator : MonoBehaviour
    {
        public float speed = 5;

        void FixedUpdate ()
        {
            transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0));
        }
    }
}
