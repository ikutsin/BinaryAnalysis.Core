using NUnit.Framework;

namespace BinaryAnalysis.Tests.Common.Schedulder
{
    [TestFixture]
    public class BasicTests
    {
        //[Test]
        //public void DoalDefSerializationTest()
        //{
        //    var tp = new TaskParameters(
        //        new Dictionary<string, ISchedulerTaskScript>
        //        {
        //            {"sleep", new SleepIntervalScript()}
        //        }, 
        //        new Dictionary<string, object>
        //        {
        //            {SleepIntervalScript.SETTING_SLEEP_INTERVAL, 5000}
        //        });

        //    var doc = TaskParametersSerializer.Serialize(tp);
        //    var tpp = TaskParametersSerializer.Deserialize(doc);

        //    Assert.AreEqual(tp.Scripts.Count, tpp.Scripts.Count);
        //    Assert.AreEqual(tp.Settings.Count, tpp.Settings.Count);
        //}
        //public static class TaskParametersSerializer
        //{
        //    public static TaskParameters Deserialize(string def)
        //    {
        //        IFormatter formatter = new BinaryFormatter();
        //        var deff = Convert.FromBase64String(def);
        //        using (var memoryStream = new MemoryStream(deff))
        //        {
        //            return (TaskParameters)formatter.Deserialize(memoryStream);
        //        }
        //    }
        //    public static string Serialize(TaskParameters def)
        //    {
        //        IFormatter formatter = new BinaryFormatter();
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            formatter.Serialize(memoryStream, def);
        //            return Convert.ToBase64String(memoryStream.ToArray());
        //        }
        //    }
        //}
    }
}
