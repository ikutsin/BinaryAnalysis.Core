using System;
using BinaryAnalysis.Terminal.ShellCommands;

namespace BA.Examples.Console.ShellCommands
{
    public class TopLevelCommands : TopLevelShellCommandSet
    {
        public void Restart()
        {
            throw new NotImplementedException("Restart is not implemented");
        }

        public void PrepareTests()
        {
            var backup = Backup();
            backup.Writer = Writer;
            backup.Context = Context;
            backup.Truncate("persons");
            backup.Truncate("mailServers");
            backup.Truncate("mailAccounts");

            backup.Restore("persons");
            backup.Restore("mailServers");
            backup.Restore("mailAccounts");

            backup.Count("persons");
            backup.Count("mailServers");
            backup.Count("mailAccounts");
        }

        public DbCommands DB()
        {
            return new DbCommands();
        }
        public MetricsCommands Metrics()
        {
            return new MetricsCommands();
        }
        public TaskCommands Task()
        {
            return new TaskCommands();
        }
        public StateCommands State()
        {
            return new StateCommands();
        }
        public BackupCommands Backup()
        {
            return new BackupCommands();
        }
        public QueryCommands Query()
        {
            return new QueryCommands();
        }
        public ProxyCommands Proxy()
        {
            return new ProxyCommands();
        }
    }
}
