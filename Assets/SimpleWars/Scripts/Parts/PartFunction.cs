using UnityEngine;
using System.Collections.Generic;

namespace SimpleWars.Parts
{
    public abstract class PartFunction : MonoBehaviour
    {
        public Transform vehicle;
        public bool active = false;
        public bool detached = false;

        public static List<PartFunction> instances;

        bool firstActive = true;

        public static void InitializeAll ()
        {
            foreach (var instance in instances)
            {
                instance.Initialize();
            }
        }

        public void Initialize ()
        {
            if (active)
            {
                return;
            }
            vehicle = transform.root;
        }

        protected void Awake ()
        {
            instances.Add(this);
        }

        protected virtual void ActiveUpdate (float deltaTime) { }

        protected virtual void ActiveStart (float deltaTime) { }

        protected virtual void VehicleUpdate () { }
    }
}