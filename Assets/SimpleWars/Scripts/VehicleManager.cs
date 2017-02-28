using UnityEngine;
using System.Collections.Generic;

namespace SimpleWars
{
    public class VehicleManager : MonoBehaviour
    {
        Transform root;

        public void LoadVehicle (Vehicle vehicle)
        {
            root = new GameObject("Vehicle").transform;
            var goDict = new Dictionary<int, GameObject>();

            foreach(var part in vehicle.parts)
            {
                GameObject go = null;
                Manager.parts.TryGetValue(part.type, out go);
                if (go != null)
                {
                    go = Instantiate(go, part.pos, Quaternion.Euler(new Vector3(0, 0, part.r)));
                    goDict.Add(part.id, go);
                    go.name = part.id.ToString();
                }
                else
                {
                    go = new GameObject(part.id.ToString());
                }
            }

            foreach(var conn in vehicle.connections)
            {
                var child = goDict[conn.Key];
                var parent = goDict[conn.Value];
                child.transform.SetParent(parent.transform);
                child.GetOrAddComponent<FixedJoint2D>().connectedBody = parent.GetOrAddComponent<Rigidbody2D>();
            }
        }

        // Use this for initialization
        void Start ()
        {

        }

        // Update is called once per frame
        void Update ()
        {

        }
    }
}
