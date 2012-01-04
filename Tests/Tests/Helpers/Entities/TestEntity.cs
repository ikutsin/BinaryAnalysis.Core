using System;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Versioning;
using BinaryAnalysis.Data.Box;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BinaryAnalysis.Tests.Helpers.Entities
{
    public class TestEntityMap : EntityClassMap<TestEntity>
    {
        public TestEntityMap()
        {
            Map(x => x.Name).Not.Nullable();
            Map(x => x.Name2);
            //Map(x => x.Description).CustomType("StringClob");
        }
    }

    [Serializable]
    [XmlType("TestEntityBoxMap")]
    [DataContract]
    [JsonObject]
    public class TestEntityBoxMap
    {
        [DataMember, XmlElement, JsonProperty]
        public string Name { get; set; }

        [DataMember, XmlElement, JsonProperty]
        public virtual string Name2 { get; set; }
    }

    [BoxTo(typeof(TestEntityBoxMap))]
    public class TestEntity : Entity, ITrackable
    {
        [TrackUpdates]
        public virtual string Name { get; set; }

        [TrackUpdates]
        public virtual string Name2 { get; set; }

        public virtual string ObjectName
        {
            get { return "Test"; }
        }
    }
}
