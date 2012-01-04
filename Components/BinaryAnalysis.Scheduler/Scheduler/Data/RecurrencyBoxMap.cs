using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using BinaryAnalysis.Box;
using Newtonsoft.Json;

namespace BinaryAnalysis.Scheduler.Scheduler.Data
{
    [Serializable]
    [XmlType("RecurrencySchedule")]
    [DataContract]
    [JsonObject]
    public class RecurrencyBoxMap
    {
        [DataMember, XmlElement, JsonProperty]
        public virtual string Cron { get; set; }

        [DataMember, XmlElement, JsonProperty]
        public virtual string TaskName { get; set; }
        
        [DataMember, XmlElement, JsonProperty]
        public virtual ScheduleTaskParallelism Parallelism { get; set; }
        
        [DataMember, XmlElement, JsonProperty]
        public virtual DateTime LastSchedule { get; set; }
    }
}
