using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using BinaryAnalysis.Box;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core.Impl;
using Newtonsoft.Json;

namespace BinaryAnalysis.Data.Metrics
{
    [Serializable,XmlType("Metrics"),DataContract,JsonObject]
    public class MetricsBoxMap : EntityBoxMap
    {
        [DataMember, XmlElement, JsonProperty]
        public string Name { get; set; }

        [DataMember, XmlElement, JsonProperty]
        public string Description { get; set; }

        [DataMember, XmlElement, JsonProperty]
        public int EntriesMax { get; set; }   
        
        [DataMember][XmlArray][JsonProperty]
        public Box<MetricsEntryBoxMap> Entries { get; set; }

        public override string ToString()
        {
            return Name +" '"+Description+"'" +"(" + Entries.Count + ")";
        }
    }

}
