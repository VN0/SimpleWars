using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

public class VehicleBuilder : MonoBehaviour
{

    // Use this for initialization
    void Awake ()
    {
        if(FindObjectsOfType<VehicleBuilder>().Length > 1)
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update ()
    {

    }
}
