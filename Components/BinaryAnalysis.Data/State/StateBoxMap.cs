using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using BinaryAnalysis.Data.Box;
using Newtonsoft.Json;

namespace BinaryAnalysis.Data.State
{
    [Serializable, XmlType("State"), DataContract, JsonObject]
    public class StateBoxMap : EntityBoxMap
    {
        [DataMember, XmlElement, JsonProperty]
        public virtual DateTime DieDate { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public virtual string Name { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public virtual string Description { get; set; }
    }
}
