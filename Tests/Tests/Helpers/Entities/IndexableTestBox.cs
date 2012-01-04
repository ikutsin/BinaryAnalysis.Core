using System;
using System.Collections.Generic;
using BinaryAnalysis.Box;
using BinaryAnalysis.Data.Box;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using BinaryAnalysis.Data.Settings;

namespace BinaryAnalysis.Tests.Helpers.Entities
{
    [BoxTo(typeof(IndexableTestBoxBoxMap))]
    public class IndexableTestBox
    {
        public string Name { get; set; }
        public string Name2 { get; set; }
        public TestBoxChild Child { get; set; }
        public IList<TestBoxChild> Children { get; set; }
    }

    [Serializable]
    [XmlType("TestBox")]
    [DataContract]
    [JsonObject]
    public class IndexableTestBoxBoxMap
    {
        [DataMember, XmlElement, JsonProperty]
        public string Name { get; set; }

        [DataMember, XmlElement, JsonProperty]
        public TestBoxChildBoxMap Child { get; set; }
        
        [DataMember, XmlElement, JsonProperty]
        public IBox<SettingsEntryBoxMap> Children { get; set; }
    }
}
