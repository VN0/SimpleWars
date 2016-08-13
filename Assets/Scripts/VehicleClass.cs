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
        public SerializableDictionary data;

        public Part () { }

        public Part (string type = "pod-0", int id = 1, float x = 0, float y = 0, float rotation = 0, bool flipX = false, bool flipY = false,
            float scaleX = 1, float scaleY = 1, SerializableDictionary data = null)
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


    [Serializable]
    public class DisconnectedSection
    {
        [XmlArray("Parts")]
        public List<Part> parts;
        [XmlArray("Connections")]
        public List<Connection> connections;

        public DisconnectedSection () { }

        public DisconnectedSection (List<Part> parts = null, List<Connection> connections = null)
        {
            this.parts = parts;
            this.connections = connections;
        }
    }
    #endregion

    #region fields
    static XmlSerializer formatter = new XmlSerializer(typeof(Vehicle));

    [XmlAttribute]
    public string name = "Vehicle";

    [XmlAttribute]
    public int version = 2;

    [XmlElement("GlobalData")]
    public SerializableDictionary globalData = new SerializableDictionary();

    [XmlArray("Parts")]
    public List<Part> parts = new List<Part>();

    [XmlArray("Disconnecteds")]
    public List<DisconnectedSection> disconnecteds = new List<DisconnectedSection>();

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
        using (FileStream file = File.Open(path, FileMode.Open))
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
        [XmlAttribute("partType")]
        public string type;
        [XmlAttribute]
        public int id;
        [XmlAttribute]
        public float x;
        [XmlAttribute]
        public float y;
        [XmlAttribute("angle")]
        public float r;
        [XmlAttribute("flippedX")]
        public bool flipX;
        [XmlAttribute("flippedY")]
        public bool flipY;
        [XmlElement("Pod")]
        public Pod pod;

        public Part () { }

        public Part (string type = "pod-0", int id = 1, float x = 0, float y = 0, float rotation = 0, bool flipX = false, bool flipY = false)
        {
            this.type = type;
            this.id = id;
            this.x = x;
            this.y = y;
            r = rotation;
            this.flipX = flipX;
            this.flipY = flipY;
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
        [XmlAttribute("Id")]
        public int id;

        public Activate () { }

        public Activate (int partId)
        {
            id = partId;
        }
    }


    [Serializable]
    public class Pod
    {
        [XmlAttribute]
        public string name;
        [XmlArray("Staging")]
        [XmlArrayItem("Step")]
        public List<List<Activate>> stages = new List<List<Activate>>();
    }


    [Serializable]
    public class DisconnectedPart
    {
        [XmlArray("Parts")]
        public List<Part> parts;
        [XmlArray("Connections")]
        public List<Connection> connections;
    }
    #endregion

    #region fields
    static XmlSerializer formatter = new XmlSerializer(typeof(VehicleSR));

    [XmlAttribute]
    public int version = 1;

    [XmlArray("DisconnectedParts")]
    public List<DisconnectedPart> disconnecteds = new List<DisconnectedPart>();

    [XmlArray("Parts")]
    public List<Part> parts = new List<Part>();

    [XmlArray("Connections")]
    public List<Connection> connections = new List<Connection>();
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


    public static VehicleSR Load (string dirName, string fileName, bool removeOnError = false)
    {
        return Load(Path.Combine(dirName, fileName));
    }


    public static VehicleSR Load (string path, bool removeOnError = false)
    {
        VehicleSR _v;
        using (FileStream file = File.Open(path, FileMode.Open))
        {
            try
            {
                _v = formatter.Deserialize(file) as VehicleSR;
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
            _v = new VehicleSR();
        }
        return _v;
    }


    public override string ToString ()
    {
        return string.Format("version={0}, parts={1}, connections={2}", version, parts.Count, connections.Count);
    }


    public Vehicle ToVehicle ()
    {
        Vehicle v = new Vehicle();
        v.parts.Capacity = parts.Count;
        v.connections.Capacity = connections.Count;
        v.disconnecteds.Capacity = disconnecteds.Count;
        foreach (Part p in parts)
        {
            if (p.type.ToLower().Contains("pod"))
            {
                if (p.pod.name != null)
                    v.name = p.pod.name;
                v.stages.Capacity = p.pod.stages.Count;
                foreach (List<Activate> s in p.pod.stages)
                {
                    List<Vehicle.Activation> act = new List<Vehicle.Activation>(s.Count);
                    foreach (Activate a in s)
                    {
                        act.Add(new Vehicle.Activation(a.id));
                    }
                    v.stages.Add(act);
                }
            }
            v.parts.Add(new Vehicle.Part(p.type, p.id, p.x, p.y, p.r * Mathf.Rad2Deg, p.flipX, p.flipY));
        }
        foreach (DisconnectedPart disconn in disconnecteds)
        {
            Vehicle.DisconnectedSection d = new Vehicle.DisconnectedSection(new List<Vehicle.Part>(), new List<Vehicle.Connection>());
            foreach (Part part in disconn.parts)
            {
                d.parts.Add(new Vehicle.Part(part.type, part.id, part.x, part.y, part.r * Mathf.Rad2Deg, part.flipX, part.flipY));
            }
            foreach (Connection conn in disconn.connections)
            {
                d.connections.Add(new Vehicle.Connection(conn.parentPart, conn.childPart));
            }
            v.disconnecteds.Add(d);
        }
        foreach (Connection conn in connections)
        {
            v.connections.Add(new Vehicle.Connection(conn.parentPart, conn.childPart));
        }
        return v;
    }
    #endregion
}


//Debug
public class VehicleClass : MonoBehaviour
{
    void Start ()
    {
#if UNITY_EDITOR
        Vehicle vehicle = new Vehicle();
        vehicle.name = "MyVehicle";
        vehicle.parts.Add(new Vehicle.Part("pod-0", 1));
        var data = new SerializableDictionary();
        data.Add("Tank", "123456");
        vehicle.parts.Add(new Vehicle.Part("tank-3", 5, 3, 4, data: data));
        vehicle.parts.Add(new Vehicle.Part("strut-0", 2, -4, 9, 90));
        vehicle.connections.Add(new Vehicle.Connection(1, 2));
        vehicle.connections.Add(new Vehicle.Connection(2, 5));
        vehicle.stages.Add(new List<Vehicle.Activation>());
        vehicle.stages[0].Add(new Vehicle.Activation(2));
        vehicle.globalData = new SerializableDictionary();
        vehicle.globalData.Add("Mods", "null");
        vehicle.globalData.Add("Modversion", "null");
        vehicle.Save(Application.persistentDataPath, "test.xml");
        Vehicle v2 = Vehicle.Load(Application.persistentDataPath, "test.xml");
        Debug.LogFormat("v2: {{{0}}}, vehicle: {{{1}}}", v2, vehicle);
        VehicleSR sr = VehicleSR.Load(Application.persistentDataPath, "Vehicles/Cannon1.xml");
        print(sr);
        sr.Save(Application.persistentDataPath, "test2.xml");
        sr.Save(Application.persistentDataPath, "Vehicles/Cannon2.xml");
        Vehicle v3 = VehicleSR.Load(Application.persistentDataPath, "test2.xml").ToVehicle();
        print(v3);
        v3.Save(Application.persistentDataPath, "test3.xml");
        v3.Save(Application.persistentDataPath, "Vehicles/CannonSW.xml");
#endif
    }
}
