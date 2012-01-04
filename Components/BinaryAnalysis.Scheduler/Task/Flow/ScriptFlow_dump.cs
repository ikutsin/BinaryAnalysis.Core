using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace BinaryAnalysis.Scheduler.Task.Flow
{
    [DataContract]
    public class DumpMessage
    {
        public enum SerializationType
        {
            SerializableBinary
        }
        public DumpMessage() 
        {
        }

        [DataMember]
        public byte[] Data { get; set; }
        [DataMember]
        public SerializationType Type { get; set; }
        [DataMember]
        public string ClassName { get; set; }

        public void SetDumpObj<T>(T obj)
        {
            ClassName = obj.GetType().Name;
            IFormatter formatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                formatter.Serialize(memoryStream, obj);
                memoryStream.Position = 0;
                Data = memoryStream.ToArray();
                Type = SerializationType.SerializableBinary;
            }
        }
        public T GetDumpObj<T>()
        {
            switch(Type) {
                case SerializationType.SerializableBinary:
                    IFormatter formatter = new BinaryFormatter();
                    using (var memoryStream = new MemoryStream(Data))
                    {
                        return (T)formatter.Deserialize(memoryStream);
                    }
                default:
                    throw new InvalidOperationException("unknown type:" + Type);
            }
        }
    }
    public partial class ScriptFlow
    {
        public void Dump(object message)
        {
            var dumpMessage = new DumpMessage();
            try
            {
                dumpMessage.SetDumpObj(message);
            }
            catch (Exception ex)
            {
                dumpMessage.SetDumpObj("Failed to dump object of type " + message.GetType().Name);
            }
            Log.Debug(message);
            goal.Settings.Set(SchedulerTask.SETTING_DUMP, dumpMessage);
        }
    }
}
