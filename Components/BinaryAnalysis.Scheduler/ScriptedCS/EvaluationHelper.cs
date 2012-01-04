using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.CSharp;

namespace BinaryAnalysis.Scheduler.ScriptedCS
{
    public static class EvaluationHelper
    {
        private static bool isInitialized;
        private static StringWriter _messageOutput;

        public static StringWriter Errors { get { return _messageOutput;  } }

        public static void InitEvaluator()
        {
            if (isInitialized) return;



            lock (typeof(EvaluationHelper))
            {
                _messageOutput = new StringWriter();
                _messageOutput.NewLine = Environment.NewLine;
                Evaluator.MessageOutput = _messageOutput;

                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        Evaluator.ReferenceAssembly(assembly);
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("Unable to reference: "+assembly);
                    }
                }
                /*
                Evaluator.Run("using System.Collections.Generic;");
                Evaluator.Run("using System;");
                Evaluator.Run("using System.Linq;");
                Evaluator.Run("using Autofac;");
                Evaluator.Run("using BinaryAnalysis.Scheduler;");
                Evaluator.Run("using BinaryAnalysis.Scheduler.Task.Flow;");
                Evaluator.Run("using BinaryAnalysis.Scheduler.Task.Script;");
                Evaluator.Run("using BinaryAnalysis.Scheduler.Task.Settings;");
                 */

                var files = Evaluator.InitAndGetStartupFiles(new string[] { });
                Evaluator.SetInteractiveBaseClass(typeof(InteractiveBase));
                isInitialized = true;
            }
        }
        public static List<string> GetCompletions(string input, string code = null)
        {
            if (code != null)
            {
                RunUsingsGetCode(code);
            }
            if(String.IsNullOrWhiteSpace(input)) return new List<string>();
            lock (typeof(EvaluationHelper))
            {
                string prefix;
                InitEvaluator();
                var completions =
                    Evaluator.GetCompletions(input, out prefix).Distinct().Select(s => prefix + s).ToList();
                return completions;
            }
        }

        private static string RunUsingsGetCode(string code)
        {
            var codeLines = code.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            int index = -1;
            
            while (++index < codeLines.Length)
            {
                var usingConst = "using ";
                if (codeLines[index].StartsWith(usingConst))
                {
                    var usings = Evaluator.GetUsing();
                    if(!usings.Contains(codeLines[index]))
                    {
                        if (Evaluator.Run(codeLines[index]))
                        {

                        }
                    }
                }
                else
                {
                    break;
                }
            } 

            return String.Join(Environment.NewLine, codeLines.Skip(index));
        }

        public static object Evaluate(string codee)
        {
            var code = RunUsingsGetCode(codee);
            
            lock (typeof(EvaluationHelper))
            {
                InitEvaluator();
                //var vars = Evaluator.GetVars();
                //var usings = Evaluator.GetUsing();
                object result;
                bool rSet;
                var success = Evaluator.Evaluate(code, out result, out rSet);

                if (!rSet)
                {
                    result = _messageOutput.ToString();
                }
                return result;
            }
        }
    }
}
