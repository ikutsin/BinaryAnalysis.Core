using BinaryAnalysis.Terminal.Commanding;
using BinaryAnalysis.Terminal.ShellCommands;

namespace BA.Examples.ConsoleClient.ShellCommands
{
    public class TopLevelCommands : TopLevelShellCommandSet
    {
        public QueryCommands Service()
        {
            return new QueryCommands();
        }
        public SchedulerCommands Scheduler()
        {
            return new SchedulerCommands();
        }
        public StateCommands State()
        {
            return new StateCommands();
        }
        public EventCommands Events()
        {
            return new EventCommands();
        }
        [CommandDescription("Unsubscribe all services alias")]
        public void UA()
        {
            new EventCommands().Unsubscribe("all");
        }

    }
}
