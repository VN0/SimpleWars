using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PartInformations", menuName = "PartInfos")]
public class PartInfos : ScriptableObject
{
    [System.Serializable]
    public class AttachPoint
    {
        public Vector2 position;
        public float rotation = 0;
        public int length = 1;
        public bool childOnly = false;
        public bool parentOnly = false;
        public bool exportFuel = false;
        public bool acceptFuel = false;
    }


    [System.Serializable]
    public class PartInfo
    {
        public string id;
        public string type;
        public string name;
        public string description;
        public int cost;
        public bool canRotate = true;
        public bool canFlip = false;
        public bool hidden = false;
        public uint maxCount = 0;
        public AttachPoint[] attachPoints;
    }
    
    public List<PartInfo> partInformations;

    public PartInfo Get (string id)
    {
        foreach (PartInfo info in partInformations)
        {
            if (info.id == id)
            {
                return info;
            }
        }
        return new PartInfo();
    }
}
