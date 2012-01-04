using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace BinaryAnalysis.Data
{
    [Serializable,XmlType("Taxon"),DataContract,JsonObject]
    public class TaxonomyNodeBoxMap
    {
        [DataMember, XmlElement, JsonProperty]
        public int Id { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public string Name { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public string Description { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public string Path { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public string ParentPath { get; set; }
    }
}
