using UnityEngine;
using System.Collections.Generic;

namespace SimpleWars.Parts
{
    public class PartFunction : MonoBehaviour
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

        /// <summary>
        /// Don't override this.
        /// </summary>
        public virtual void Initialize ()
        {
            if (active)
            {
                return;
            }
            vehicle = transform.root;
        }

        /// <summary>
        /// Don't override this.
        /// </summary>
        protected virtual void Awake ()
        {
            instances.Add(this);
        }

        /// <summary>
        /// You can override this.
        /// </summary>
        protected virtual void ActiveUpdate () { }

        /// <summary>
        /// You can override this.
        /// </summary>
        protected virtual void ActiveStart () { }

        /// <summary>
        /// Don't override this.
        /// </summary>
        protected virtual void Update ()
        {
            if (active)
            {
                ActiveUpdate();
            }

            if (firstActive & active)
            {
                ActiveStart();
            }
        }
    }
}