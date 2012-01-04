using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core.Impl;
using Newtonsoft.Json;

namespace BinaryAnalysis.Data.Metrics
{
    [Serializable,XmlType("MetricsEntry"),DataContract,JsonObject]
    public class MetricsEntryBoxMap : EntityBoxMap
    {
        [DataMember, XmlElement, JsonProperty]
        public virtual decimal Value { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public virtual DateTime RecordDate { get; set; }
    }

}
