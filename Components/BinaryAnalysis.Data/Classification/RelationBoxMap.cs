using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core.Impl;
using Newtonsoft.Json;
using NHibernate.Validator.Constraints;

namespace BinaryAnalysis.Data.Classification
{
    [DataContract]
    public enum RelationDirection : int
    {
        [EnumMember]
        Undefined = 0,
        [EnumMember]
        Forward,
        [EnumMember]
        Back,
        [EnumMember]
        Both
    }
    [Serializable, XmlType("Relation"), DataContract, JsonObject]
    public class RelationBoxMap : EntityBoxMap
    {
        [DataMember, XmlElement, JsonProperty]
        public virtual RelationDirection Direction { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public virtual string ObjectName { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public virtual int ObjectID { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public virtual string RelatedObjectName { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public virtual int RelatedObjectID { get; set; }
        [DataMember, XmlElement, JsonProperty]
        [ManualBoxingMethod("BoxDescription", null)]
        public virtual string Description { get; set; }
    }
}
