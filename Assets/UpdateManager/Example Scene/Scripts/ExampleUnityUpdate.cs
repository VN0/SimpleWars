using UnityEngine;
using System.Collections;

public class ExampleUnityUpdate : MonoBehaviour
{
    private int i;
    private void Update ()
    {
        i++;
    }
    void FixedUpdate ()
    {
        i--;
    }
}
