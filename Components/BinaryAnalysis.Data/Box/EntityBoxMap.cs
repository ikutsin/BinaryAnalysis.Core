using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using BinaryAnalysis.Box;
using Newtonsoft.Json;

namespace BinaryAnalysis.Data.Box
{
    [Serializable,XmlType("EmptyEntity"),DataContract,JsonObject]
    public class EntityBoxMap
    {
        [DataMember,XmlAttribute,JsonProperty]
        public int Id { get; set; }
    }
}
