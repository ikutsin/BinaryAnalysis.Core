using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Core;
using BinaryAnalysis.Terminal.Commanding;
using BinaryAnalysis.Terminal.Redist;
using BinaryAnalysis.Terminal.ShellCommands;

namespace BinaryAnalysis.Terminal
{
    public class Shell
    {
        public TextWriter Writer { get { return Console.Out; } }
        public CommandMatchHelper CommandMatcher { get; protected set; }

        private IComponentContext context;
        public bool IsShellEnabled { get; set; }

        /// <summary>
        /// throws OptionException
        /// </summary>
        /// <param name="args"></param>
        /// <param name="context"></param>
        /// <param name="clazz"></param>
        /// <returns>Unparsed parameters</returns>
        public string[] ProcessArguments(string[] args, IComponentContext context, Type clazz)
        {
            if (clazz.IsAssignableFrom(typeof(TopLevelShellCommandSet)))
            {
                throw new Exception("clazz type should be derived from TopLevelShellCommandSet");
            }
            this.context = context;
            CommandMatcher = new CommandMatchHelper(context, clazz, Writer);

            bool show_help = false;
            bool list_action = false;
            bool list_action_all = false;
            string call_action = null;
            var p = new OptionSet()
                {
                    { "h|?|help",  "show help",  (string v) => show_help = v != null },
                    { "sl|slist|simplelist",  "list registered actions",  (v) =>list_action = v!=null },
                    { "l|list",  "list actions using help command",  (v) =>list_action_all = v!=null },
                    { "s|shell",  "start shell",  (v) =>IsShellEnabled = v != null },
                    { "a|action:",  "call action",  (v) =>call_action = v },
                };

            try
            {
                var propList = p.Parse(args);
                if (show_help) ShowHelp(p);
                if (list_action) Writer.WriteLine(String.Join(",\n", CommandMatcher.AutoComplete));
                if (list_action_all) CommandMatcher.CallAction("help list");
                if (!string.IsNullOrEmpty(call_action)) CommandMatcher.CallAction(call_action);
                return propList.ToArray();
            }
            catch (Exception ex)
            {
                Writer.WriteLine("Error: "+ex.Message);
                return args;
            }
        }

        public void StartLineEditor()
        {
            LineEditor le = new LineEditor("LineEditor", 50);
            le.AutoCompleteEvent = (string text, int pos) =>
            {
                //text = text.ToLower();
                string token = null;
                for (int i = pos - 1; i >= 0; i--)
                {
                    if (Char.IsWhiteSpace(text[i]))
                    {
                        token = text.Substring(i + 1, pos - i - 1);
                        break;
                    }
                    else if (i == 0)
                        token = text.Substring(0, pos);
                }
                List<string> results = new List<string>();
                if (token == null)
                {
                    token = string.Empty;
                    results.AddRange(CommandMatcher.AutoComplete.Select(x => x.Path).ToArray());
                }
                else
                {
                    string[] completePaths = CommandMatcher.AutoComplete
                        .Where(x => x.Path.Length > pos)
                        .Select(x => x.Path.Substring(pos - token.Length))
                        .ToArray();
                    for (int i = 0; i < completePaths.Length; i++)
                    {
                        if (completePaths[i].StartsWith(token))
                        {
                            string result = completePaths[i];
                            results.Add(result.Substring(token.Length, result.Length - token.Length));
                        }
                    }

                }
                return new LineEditor.Completion(token, results.ToArray());
            };
            string s;
            while ((s = le.Edit("> ", "")) != null)
            {
                if (new[] { "exit", "quit", "bye", "q", "e" }.Contains(s)) break;
                CommandMatcher.CallAction(s);
            }
        }

        void ShowHelp(OptionSet p)
        {
            Writer.WriteLine("Usage: [OPTIONS]+ message");
            Writer.WriteLine();
            Writer.WriteLine("Options:");
            p.WriteOptionDescriptions(Writer);
        }
    }
}
