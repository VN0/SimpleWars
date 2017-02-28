using UnityEngine;

namespace SimpleWars
{
    [RequireComponent(typeof(Camera))]
    public class SkyboxRotator : MonoBehaviour
    {
        public float speed = 5;

        private void Awake ()
        {
            GetComponent<Rigidbody>().angularVelocity = new Vector3(0, speed/60, 0);
        }
    }
}
