using UnityEngine;
using System.Collections;

public delegate void UpdateEvent (float deltaTime);
public delegate void ActivateEvent ();

namespace SimpleWars.Parts
{
    public class PartsManager : MonoBehaviour
    {
        public event UpdateEvent ActiveUpdate;
        public event UpdateEvent VehicleUpdate;
        public event ActivateEvent Activate;
        public event ActivateEvent Initialize;

        void Start ()
        {

        }

        // Update is called once per frame
        void Update ()
        {

        }
    }
}