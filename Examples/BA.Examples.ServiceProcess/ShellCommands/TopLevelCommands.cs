using System;
using BinaryAnalysis.Terminal.ShellCommands;

namespace BA.Examples.ServiceProcess.ShellCommands
{
    public class TopLevelCommands : TopLevelShellCommandSet
    {
        public void Restart()
        {
            throw new NotImplementedException("Restart is not implemented");
        }
        public DbCommands DB()
        {
            return new DbCommands();
        }
        public TaskCommands Task()
        {
            return new TaskCommands();
        }
    }
}
