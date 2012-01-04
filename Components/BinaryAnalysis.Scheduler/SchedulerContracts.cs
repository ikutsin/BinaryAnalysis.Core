using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BinaryAnalysis.Scheduler
{
    [DataContract, Serializable]
    public class BrowsingGoalScriptSchedule
    {
        [DataMember]
        public string ScriptName { get; set; }
        [DataMember]
        public DateTime Date { get; set; }

        public override string ToString()
        {
            return String.Format("{0} at {1}", ScriptName, Date);
        }
    }

    [DataContract]
    public enum ScheduleTaskParallelism : int
    {
        [EnumMember]
        Standalone = 0,
        [EnumMember]
        AllowOthers,
        [EnumMember]
        AllowAll
    }
    [DataContract]
    public enum ScheduleMessageState : int
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        Debug,
        [EnumMember]
        Info,
        [EnumMember]
        Warn,
        [EnumMember]
        Error,
        [EnumMember]
        Fatal
    }
    [DataContract]
    public enum ScheduleExecutionState : int
    {
        [EnumMember]
        Idle = 0,
        [EnumMember]
        Running,
        [EnumMember]
        Finished
    }
    [DataContract,Serializable]
    public class ScriptAssertionMessage
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public ScheduleMessageState Type { get; set; }
        [DataMember]
        public DateTime Date { get; set; }

        public override string ToString()
        {
            return String.Format("{0}: ({1}) {2}", Date, Type, Message);
        }
    }
}
