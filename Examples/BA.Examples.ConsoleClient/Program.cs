using System.Collections.Generic;
using BA.Examples.ConsoleClient.ShellCommands;
using BinaryAnalysis.Box;
using BinaryAnalysis.Box.Presentations;
using BinaryAnalysis.Terminal;

namespace BA.Examples.ConsoleClient
{
    class Program
    {
        static Shell shell = new Shell();
        public static void DumpBoxed<T>(IList<T> boxed)
        {
            var box = new Box<T>(boxed);
            var presenter = new DumperBoxPresentation();
            shell.Writer.WriteLine(presenter.AsString(box));
        }

        static void Main(string[] args)
        {
            using (var bootstrap = new Bootstrap())
            {
                shell.ProcessArguments(args, bootstrap.Container, typeof(TopLevelCommands));
                if (shell.IsShellEnabled) shell.StartLineEditor();
            }
        }
    }
}
