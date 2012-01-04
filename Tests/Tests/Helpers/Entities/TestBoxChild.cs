using System;
using BinaryAnalysis.Data.Box;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace BinaryAnalysis.Tests.Helpers.Entities
{
    [BoxTo(typeof(TestBoxChildBoxMap))]
    public class TestBoxChild
    {
        public string Name { get; set; }
        public TestBox Parent { get; set; }
    }

    [Serializable]
    [XmlType("TestBoxChild")]
    [DataContract]
    [JsonObject]
    public class TestBoxChildBoxMap
    {
        [DataMember, XmlElement, JsonProperty]
        public string Name { get; set; }
        //[DataMember, XmlElement, JsonProperty]
        //public TestBoxBoxMap Parent { get; set; }
    }
}
