using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using BinaryAnalysis.Box;
using BinaryAnalysis.Data.ContractContainer;
using Newtonsoft.Json;
using BinaryAnalysis.Data.Box;
using System.Reflection;

namespace BinaryAnalysis.Data.Settings
{
    [Serializable]
    [XmlType("SettingsEntry")]
    [DataContract]
    [JsonObject]
    public class SettingsEntryBoxMap : ContractContainerBoxMap
    {
        [DataMember, XmlElement, JsonProperty]
        public string Name { get; set; }
    }

    [Serializable]
    [XmlType("Settings")]
    [DataContract]
    [JsonObject]
    public class SettingsBoxMap : EntityBoxMap
    {
        [DataMember, XmlElement, JsonProperty]
        public string Name { get; set; }

        [DataMember, XmlArray, JsonProperty]
        public Box<SettingsEntryBoxMap> Entries { get; set; }

        public override string ToString()
        {
            return Name + "(" + Entries.Count + ")";
        }
    }
}
