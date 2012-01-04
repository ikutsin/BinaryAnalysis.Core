using Autofac;
using BinaryAnalysis.Scheduler.Task.Script;

namespace BinaryAnalysis.Scheduler.ScriptedCS
{
    public class InteractiveBase
    {
        public static IComponentContext context { get; set; }
        public static ScriptUtility x { get; set; }
    }
}