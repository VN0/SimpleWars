using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;


namespace SimpleWars
{
    /// <summary>
    /// Vehicle class for SimpleWars
    /// </summary>

    [Serializable]
    public class Vehicle
    {
        #region classes
        [Serializable]
        public class Part
        {
            public string type;
            public int id;
            [JsonConverter(typeof(WanzyeeStudio.Json.Vector2Converter))]
            public Vector2 pos;
            public float r;
            public bool flipX;
            public bool flipY;
            [JsonConverter(typeof(WanzyeeStudio.Json.Vector2Converter))]
            public Vector2 scale;
            public Dictionary<string, string> data;

            public Part () { }

            public Part (string type = "pod-0", int id = 1, Vector2? pos = null, float rotation = 0, bool flipX = false, bool flipY = false,
                Vector2? scale = null, Dictionary<string, string> data = null)
            {
                this.type = type;
                this.id = id;
                this.pos = pos ?? Vector2.zero;
                r = rotation;
                this.flipX = flipX;
                this.flipY = flipY;
                this.scale = scale ?? Vector2.one;
                this.data = data;
            }
        }


        [Serializable]
        public class Activation
        {
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
            public List<Part> parts;
            public Dictionary<int, int> connections;

            public DisconnectedSection () { }

            public DisconnectedSection (List<Part> parts = null, Dictionary<int, int> connections = null)
            {
                this.parts = parts;
                this.connections = connections;
            }
        }
        #endregion

        #region fields
        static JsonSerializer formatter = new JsonSerializer() { Formatting = Newtonsoft.Json.Formatting.Indented };

        public string name = "Vehicle";

        public int version = 2;

        public Dictionary<string, string> globalData = new Dictionary<string, string>();

        public List<Part> parts = new List<Part>();

        public List<DisconnectedSection> disconnecteds = new List<DisconnectedSection>();

        /// <summary>
        /// Key=child, Value=parent
        /// </summary>
        public Dictionary<int, int> connections = new Dictionary<int, int>();

        public List<List<Activation>> stages = new List<List<Activation>>();
        #endregion

        #region methods

        public void Save (string dirName, string fileName)
        {
            Save(Path.Combine(dirName, fileName));
        }


        public void Save (string path)
        {
            using (var stream = new StreamWriter(path))
            using (var file = new JsonTextWriter(stream))
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
            using (var stream = new StreamReader(path))
            using (var file = new JsonTextReader(stream))
            {
                try
                {
                    _v = formatter.Deserialize<Vehicle>(file);
                }
                catch (Exception ex)
                {
                    _v = null;
                    if (ex is JsonException)
                    {
                        Unbug.LogError("InvalidJSON");
                    }
                    else if (ex is FormatException)
                    {
                        Unbug.LogError("Error while Deserializing");
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


        public string ExportToString ()
        {
            return JsonConvert.SerializeObject(this);
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
                        Unbug.LogError("Invalid XML");
                    }
                    else if (ex is FormatException)
                    {
                        Unbug.LogError("Error while Deserializing");
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


        public string ExportToString ()
        {
            MemoryStream stream = new MemoryStream();
            using (XmlWriter file = XmlWriter.Create(stream, new XmlWriterSettings()
            {
                Indent = true,
                Encoding = System.Text.Encoding.UTF8
            }))
            {
                formatter.Serialize(file, this);
            }
            return new StreamReader(stream).ReadToEnd();
        }


        public Vehicle ToVehicle ()
        {
            Vehicle v = new Vehicle();
            v.parts.Capacity = parts.Count;
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
                v.parts.Add(new Vehicle.Part(p.type, p.id, new Vector2(p.x, p.y), p.r * Mathf.Rad2Deg, p.flipX, p.flipY));
            }
            foreach (DisconnectedPart disconn in disconnecteds)
            {
                Vehicle.DisconnectedSection d = new Vehicle.DisconnectedSection(new List<Vehicle.Part>(), new Dictionary<int, int>());
                foreach (Part part in disconn.parts)
                {
                    d.parts.Add(new Vehicle.Part(part.type, part.id, new Vector2(part.x, part.y), part.r * Mathf.Rad2Deg, part.flipX, part.flipY));
                }
                foreach (Connection conn in disconn.connections)
                {
                    d.connections.Add(conn.childPart, conn.parentPart);
                }
                v.disconnecteds.Add(d);
            }
            foreach (Connection conn in connections)
            {
                v.connections.Add(conn.childPart, conn.parentPart);
            }
            return v;
        }
        #endregion
    }
}
