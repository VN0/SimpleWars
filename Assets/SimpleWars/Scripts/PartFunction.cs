using UnityEngine;
using System.Collections.Generic;

namespace SimpleWars
{
    public class PartFunction : MonoBehaviour
    {
        public static List<PartFunction> instances;
        public static void InitializeAll ()
        {
            foreach (var instance in instances)
            {
                instance.Initialize();
            }
        }

        public virtual void Initialize ()
        {

        }

        protected virtual void Awake ()
        {
            instances.Add(this);
        }
    }
}