using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


/// <summary>
/// Vehicle class for SimpleWars
/// </summary>

[Serializable]
[XmlRoot("Vehicle")]
public class Vehicle
{
    #region classes
    [Serializable]
    public class Part
    {
        [XmlAttribute]
        public string type;
        [XmlAttribute]
        public int id;
        [XmlAttribute]
        public float x;
        [XmlAttribute]
        public float y;
        [XmlAttribute]
        public float r;
        [XmlAttribute]
        public bool flipX;
        [XmlAttribute]
        public bool flipY;
        [XmlAttribute]
        public float scaleX;
        [XmlAttribute]
        public float scaleY;
        [XmlElement("Data")]
        public string data;

        public Part () { }

        public Part (string type = "pod-0", int id = 1, float x = 0, float y = 0, float rotation = 0, bool flipX = false, bool flipY = false, float scaleX = 1, float scaleY = 1, string data = null)
        {
            this.type = type;
            this.id = id;
            this.x = x;
            this.y = y;
            r = rotation;
            this.flipX = flipX;
            this.flipY = flipY;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.data = data;
        }
    }


    [Serializable]
    public class Connection
    {
        [XmlAttribute]
        public int parent;
        [XmlAttribute]
        public int child;

        public Connection () { }

        public Connection (int parent, int child)
        {
            this.parent = parent;
            this.child = child;
        }
    }


    [Serializable]
    public class Activation
    {
        [XmlAttribute]
        public int id;

        public Activation () { }

        public Activation (int partId)
        {
            id = partId;
        }
    }

    #endregion

    #region fields
    static XmlSerializer formatter = new XmlSerializer(typeof(Vehicle));

    [XmlAttribute]
    public string name = "Vehicle";
    [XmlAttribute]
    public int version = 1;

    [XmlArray("Parts")]
    public List<Part> parts = new List<Part>();

    [XmlArray("Disconnecteds")]
    public List<Part> disconnecteds = new List<Part>();

    [XmlArray("Connections")]
    public List<Connection> connections = new List<Connection>();

    [XmlArray("ActivationStages")]
    [XmlArrayItem("Stage")]
    public List<List<Activation>> stages = new List<List<Activation>>();
    #endregion

    #region methods
    public void Save (string dirName, string fileName)
    {
        Save(Path.Combine(dirName, fileName));
    }


    public void Save (string path)
    {
        using (XmlWriter file = XmlWriter.Create(path, new XmlWriterSettings()
        {
            Indent = true,
            Encoding = System.Text.Encoding.UTF8
        }))
        {
            formatter.Serialize(file, this);
        }
    }


    public static Vehicle Load (string dirName, string fileName, bool removeOnError = false)
    {
        return Load(Path.Combine(dirName, fileName));
    }


    public static Vehicle Load (string path, bool removeOnError = false)
    {
        Vehicle _v;
        using (FileStream file = File.Open(path, FileMode.OpenOrCreate))
        {
            try
            {
                _v = formatter.Deserialize(file) as Vehicle;
            }
            catch (Exception ex)
            {
                _v = null;
                if (ex is XmlException)
                {
                    Debug.LogError("Invalid XML");
                }
                else if (ex is FormatException)
                {
                    Debug.LogError("Error while Deserializing");
                }
                else
                {
                    throw;
                }
            }
        }
        if (_v == null && removeOnError)
        {
            File.Delete(path);
            _v = new Vehicle();
        }
        return _v;
    }


    public override string ToString ()
    {
        return string.Format("name={0}, version={1}, parts={2}, disconnecteds={3}, connections={4}, steps={5}",
            name, version, parts.Count, disconnecteds.Count, connections.Count, stages.Count);
    }
    #endregion
}


/// <summary>
/// Vehicle class for SimpleRockets
/// </summary>

[Serializable]
[XmlRoot("Ship")]
public class VehicleSR
{
    #region classes
    [Serializable]
    public class Part
    {
        [XmlAttribute]
        public string partType;
        [XmlAttribute]
        public int id;
        [XmlAttribute]
        public float x;
        [XmlAttribute]
        public float y;
        [XmlAttribute]
        public float angle;
        [XmlAttribute]
        public bool flippedX;
        [XmlAttribute]
        public bool flippedY;

        public Part () { }

        public Part (string type = "pod-0", int id = 1, float x = 0, float y = 0, float rotation = 0, bool flipX = false, bool flipY = false)
        {
            partType = type;
            this.id = id;
            this.x = x;
            this.y = y;
            angle = rotation;
            flippedX = flipX;
            flippedY = flipY;
        }
    }


    [Serializable]
    public class Connection
    {
        [XmlAttribute]
        public int parentPart;
        [XmlAttribute]
        public int childPart;

        public Connection () { }

        public Connection (int parent, int child)
        {
            parentPart = parent;
            childPart = child;
        }
    }


    [Serializable]
    public class Activate
    {
        [XmlAttribute]
        public int Id;

        public Activate () { }

        public Activate (int partId)
        {
            Id = partId;
        }
    }


    [Serializable]
    public class Pod
    {

    }

    #endregion

    #region fields
    static XmlSerializer formatter = new XmlSerializer(typeof(Vehicle));
    
    [XmlAttribute]
    public int version = 1;

    [XmlArray("Parts")]
    public List<Part> parts = new List<Part>();

    [XmlArray("Connections")]
    public List<Connection> connections = new List<Connection>();

    [XmlArray("Staging")]
    [XmlArrayItem("Step")]
    public List<List<Activate>> stages = new List<List<Activate>>();
    #endregion

    #region methods
    public void Save (string dirName, string fileName)
    {
        Save(Path.Combine(dirName, fileName));
    }


    public void Save (string path)
    {
        using (XmlWriter file = XmlWriter.Create(path, new XmlWriterSettings()
        {
            Indent = true,
            Encoding = System.Text.Encoding.UTF8
        }))
        {
            formatter.Serialize(file, this);
        }
    }


    public static Vehicle Load (string dirName, string fileName, bool removeOnError = false)
    {
        return Load(Path.Combine(dirName, fileName));
    }


    public static Vehicle Load (string path, bool removeOnError = false)
    {
        Vehicle _v;
        using (FileStream file = File.Open(path, FileMode.OpenOrCreate))
        {
            try
            {
                _v = formatter.Deserialize(file) as Vehicle;
            }
            catch (Exception ex)
            {
                _v = null;
                if (ex is XmlException)
                {
                    Debug.LogError("Invalid XML");
                }
                else if (ex is FormatException)
                {
                    Debug.LogError("Error while Deserializing");
                }
                else
                {
                    throw;
                }
            }
        }
        if (_v == null && removeOnError)
        {
            File.Delete(path);
            _v = new Vehicle();
        }
        return _v;
    }


    public override string ToString ()
    {
        return string.Format("name={0}, version={1}, parts={2}, connections={3}, steps={4}", "null", version, parts.Count, connections.Count, stages.Count);
    }
    #endregion
}


//Debug
public class VehicleClass : MonoBehaviour
{
    void Start ()
    {
        Vehicle vehicle = new Vehicle();
        vehicle.name = "myVehicle";
        vehicle.parts.Add(new Vehicle.Part("pod-0", 1));
        vehicle.parts.Add(new Vehicle.Part("tank-3", 5, 3, 4, data:"<Tank fuel=\"10\"/ >"));
        vehicle.parts.Add(new Vehicle.Part("strut-0", 2, -4, 9, 90));
        vehicle.disconnecteds.Add(new Vehicle.Part("solar-1", 3, -2, 3, 270, true));
        vehicle.connections.Add(new Vehicle.Connection(1, 2));
        vehicle.connections.Add(new Vehicle.Connection(2, 5));
        vehicle.stages.Add(new List<Vehicle.Activation>());
        vehicle.stages[0].Add(new Vehicle.Activation(2));
        vehicle.Save(Application.persistentDataPath, "test.xml");
        Vehicle v2 = Vehicle.Load(Application.persistentDataPath, "test.xml");
        Debug.LogFormat("v2: {{{0}}}, vehicle: {{{1}}}", v2, vehicle);
    }
}
