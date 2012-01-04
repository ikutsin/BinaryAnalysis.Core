using System;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Versioning;
using NHibernate.Search.Attributes;
using BinaryAnalysis.Data.Box;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BinaryAnalysis.Tests.Helpers.Entities
{
    public class IndexableTestEntityMap : EntityClassMap<IndexableTestEntity>
    {
        public IndexableTestEntityMap()
        {
            Map(x => x.Name).Not.Nullable();
            Map(x => x.Name2);
            //Map(x => x.Description).CustomType("StringClob");
        }
    }

    [Serializable]
    [XmlType("IndexableTestEntityBoxMap")]
    [DataContract]
    [JsonObject]
    public class IndexableTestEntityBoxMap
    {
        [DataMember, XmlElement, JsonProperty]
        public string Name { get; set; }

        [DataMember, XmlElement, JsonProperty]
        public virtual string Name2 { get; set; }
    }

    [Indexed]
    [BoxTo(typeof(IndexableTestEntityBoxMap))]
    public class IndexableTestEntity : Entity, ITrackable
    {
        [Field(Index.Tokenized)]
        public virtual string Name { get; set; }

        [Field(Index.UnTokenized)]
        public virtual string Name2 { get; set; }

        public virtual string ObjectName
        {
            get { return "IndexableTest"; }
        }
    }
}
