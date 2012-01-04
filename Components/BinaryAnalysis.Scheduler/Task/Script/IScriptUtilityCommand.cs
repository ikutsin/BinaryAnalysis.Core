using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Scheduler.Task.Script
{
    public interface IScriptUtilityCommand
    {
        Type InputType { get; }
        Type ReturnType { get; }
        object Execute(ScriptUtility x, object input);
    }
    public interface IScriptUtilityCommandMetadata
    {
        string CommandName { get; }
    }
}
