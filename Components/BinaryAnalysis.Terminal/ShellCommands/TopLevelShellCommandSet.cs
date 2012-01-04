using BinaryAnalysis.Terminal.Commanding;

namespace BinaryAnalysis.Terminal.ShellCommands
{
    public abstract class TopLevelShellCommandSet : ShellCommandSet
    {
        public HelpCommands Help()
        {
            return new HelpCommands();
        }
        public void Exit()
        {

        }
    }
}
