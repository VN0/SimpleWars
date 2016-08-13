using System.Collections.Generic;
using System.Xml.Serialization;


public class SerializableDictionary
    : Dictionary<string, string>, IXmlSerializable
{
    #region IXmlSerializable Members
    public System.Xml.Schema.XmlSchema GetSchema ()
    {
        return null;
    }

    public void ReadXml (System.Xml.XmlReader reader)
    {
        bool wasEmpty = reader.IsEmptyElement;
        reader.Read();

        if (wasEmpty)
            return;

        while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
        {
            string key = reader.Name;
            reader.ReadStartElement();
            string value = reader.ReadContentAsString();
            reader.ReadEndElement();

            this[key] = value;

            reader.MoveToContent();
        }
        reader.ReadEndElement();
    }

    public void WriteXml (System.Xml.XmlWriter writer)
    {
        foreach (string key in this.Keys)
        {
            writer.WriteStartElement(key);
            string value = this[key];
            writer.WriteString(value);
            writer.WriteEndElement();
        }
    }
    #endregion
}