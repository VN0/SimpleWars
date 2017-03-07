using System.Collections.Generic;
using UnityEngine;

namespace SimpleWars.Planets
{
    class Gravity : MonoBehaviour
    {
        public const float G = 6.67408E-11f;
        public float planetMass = 10000;
        public float planetRadius = 100;

        public static float Force (float dist, float m1, float m2)
        {
            /*F = G m1*m2 / r^2  
             G = gravitational constant | G = 6.67300 × 10^−11
             m1 = mass 1 (kg)
             m2 = mass 2 (kg)
             r = distance between the centers of the masses*/

            //find the distance between the x y and z pairs

            //calculate the distance between the two objects r^2 = x^2 + y^2 + z^2 

            return G * ((m1 * m2) / (dist * dist));
        }
        
        public static float Acceleration (float dist, float mass)
        {
            return G * (mass / (dist * dist));
        }

        public static Vector2 VectorForce (Vector2 planet, Vector2 target, float mass)
        {
            var diff = planet - target;
            float dist = diff.magnitude;
            return diff.normalized * (G * ((mass * mass) / (dist * dist)));
        }

        private void Awake ()
        {
            Physics2D.gravity = Vector2.zero;
        }

        private void OnTriggerStay2D (Collider2D other)
        {
            var f = VectorForce(transform.position, other.transform.position, planetMass);
            other.attachedRigidbody.AddForce(f);
            print(f);
        }
    }
}