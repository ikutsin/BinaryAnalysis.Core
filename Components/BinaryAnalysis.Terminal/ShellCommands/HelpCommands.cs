using BinaryAnalysis.Terminal.Commanding;
using BinaryAnalysis.Terminal.Redist;
using System;
using System.Text;

namespace BinaryAnalysis.Terminal.ShellCommands
{
    public class HelpCommands : ShellCommandSet
    {
        public CommandMatchHelper CommandMatcher { get; set; }

        [CommandDescription("List all actions")]
        public void List()
        {
            CommandMatcher.AutoComplete.ForEach(x => {
                Writer.Write(x.Path);
                Writer.WriteLine(" - "+x.Description);
            });
        }

        public void Handlers()
        {
            Array.ForEach(LineEditor.handlers, h =>
            {
                var cki = h.CKI;
                var sb = new StringBuilder();
                String m4 = " (character '{0}').";

                if (cki.Modifiers != 0)
                {
                    if ((cki.Modifiers & ConsoleModifiers.Alt) != 0)
                        sb.Append("ALT+");
                    if ((cki.Modifiers & ConsoleModifiers.Shift) != 0)
                        sb.Append("SHIFT+");
                    if ((cki.Modifiers & ConsoleModifiers.Control) != 0)
                        sb.Append("CTL+");
                }
                sb.Append(cki.Key.ToString());
                sb.AppendFormat(m4, cki.KeyChar);
                Writer.WriteLine(sb.ToString());
                //Dumper.Dump(h, Program.Writer);
            });
        }


        [CommandDescription("Basic information")]
        public void Basic()
        {
            Writer.Write(
                String.Join(", ",new[]{"exit", "quit", "bye", "q", "e"}));
            Writer.WriteLine(" - Exit programm");

            Writer.WriteLine("[action] ? - ask details");
        }
    }
}
