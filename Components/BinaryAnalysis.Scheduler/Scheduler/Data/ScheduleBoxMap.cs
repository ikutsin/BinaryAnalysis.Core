using System;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Settings;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BinaryAnalysis.Scheduler.Scheduler.Data
{
    [Serializable]
    [XmlType("ScheduleItem")]
    [DataContract]
    [JsonObject]
    public class ScheduleBoxMap : EntityBoxMap
    {
        [DataMember, XmlElement, JsonProperty]
        public virtual string TaskName { get; set; }

        [DataMember, XmlElement, JsonProperty]
        public virtual DateTime LastExecution { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public virtual DateTime NextExecution { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public virtual ScheduleMessageState MessageState { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public virtual ScheduleExecutionState ExecutionState { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public virtual ScheduleTaskParallelism Parallelism { get; set; }

        [DataMember, XmlElement, JsonProperty]
        public virtual SettingsBoxMap Settings { get; set; }
    }
}
