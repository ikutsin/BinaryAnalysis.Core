using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using BinaryAnalysis.Terminal.ShellCommands;

namespace BinaryAnalysis.Terminal.Commanding
{
    public class CommandInfo {
        public string Path { get; set; }
        public string MethodName { get; set; }
        public string Description { get; set; }

        public Type Clazz { get; set; }

        public override string ToString()
        {
            return Path;
        }
    }
    public class CommandMatchHelper
    {
        private readonly IComponentContext context;
        private readonly TextWriter writer;
        public List<CommandInfo> AutoComplete { get; protected set; }

        public CommandMatchHelper(IComponentContext context, Type clazz, TextWriter writer)
        {
            this.context = context;
            this.writer = writer;
            AutoComplete = GetCommandsFrom(clazz);
        }

        public void CallAction(string s)
        {
            var sarr = s.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (sarr.Length > 0)
            {
                bool showDescription = sarr[sarr.Length - 1] == "?";
                CommandInfo command = null;

                int i = sarr.Length - 1;
                for (; i >= 0; i--)
                {
                    var path = string.Join(" ", sarr.Take(i + 1));
                    command = AutoComplete.Where(x => x.Path == path).FirstOrDefault();
                    if (command != null) break;
                }
                if (command != null)
                {
                    if (showDescription) // " ?" at the end of the command
                    {
                        writer.WriteLine("Description for [{0}] :", command);
                        writer.WriteLine(command.Description);
                    }
                    else
                    {
                        //writer.WriteLine("Calling command ----> [{0}]", command);
                        try
                        {
                            //Instance creation
                            var obj = Activator.CreateInstance(command.Clazz) as ShellCommandSet;
                            obj.Context = context;
                            obj.Writer = writer;
                            if (obj is HelpCommands)
                            {
                                (obj as HelpCommands).CommandMatcher = this;
                            }

                            var method = command.Clazz.GetMethod(command.MethodName, BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                            
                            //split from params
                            var j = i;
                            while (0!=String.Compare(method.Name, sarr[j], true)) j--;

                            var param = String.Join(" ", sarr.Skip(j+1).ToArray());
                            
                            if (String.IsNullOrEmpty(param))
                            {
                                method.Invoke(obj, null);
                            }
                            else
                            {
                                method.Invoke(obj, new[] { (String.IsNullOrEmpty(param) ? null : param) });
                            }
                            //writer.WriteLine(String.Join(", ", sarr.Skip(i)));
                        }
                        catch (Exception ex)
                        {
                            if (ex is TargetInvocationException)
                            ex = ex.InnerException;
                            
                            writer.WriteLine("=> {0}", ex.Message);
                        }
                    }
                }
                else
                {
                    writer.WriteLine("Bad command.");
                    //writer.WriteLine("Bad command ----> [{0}]", s);
                }
            }
        }

        List<CommandInfo> GetCommandsFrom(Type clazz, string prefix = null)
        {
            var ret = new List<CommandInfo>();

            MethodInfo[] methods = clazz
                .GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
               .Where(m => m.DeclaringType != typeof(object))
               .Where(m => !m.Name.StartsWith("set_"))
               .Where(m => !m.Name.StartsWith("get_"))
               .ToArray();

            Array.ForEach(methods, method =>
            {
                var paramz = method.GetParameters();

                var path = (String.IsNullOrEmpty(prefix)?method.Name:
                        String.Join(" ", new string[] {prefix, method.Name })
                        ).ToLower();
                if (method.ReturnType == typeof(String[]))
                {

                    var obj = Activator.CreateInstance(clazz) as ShellCommandSet;
                    obj.Context = context;
                    obj.Writer = writer;

                    var options = (string[])method.Invoke(obj,
                        new object[] { null });

                    //add params from string[] ret
                    var descr = (CommandDescriptionAttribute)method.GetCustomAttributes(typeof(CommandDescriptionAttribute), true).FirstOrDefault();
                    //parameterless call
                    ret.Add(new CommandInfo()
                    {
                        Description = descr == null ? "" : descr.Description,
                        MethodName = method.Name, //str,
                        Path = path, //String.Join(" ", new string[] { path, "" }),
                        Clazz = clazz
                    });
                    //list from ret
                    Array.ForEach(options, str => ret.Add(
                        new CommandInfo()
                        {
                            Description = descr == null ? "" : descr.Description + "(Parameter " + str + ")",
                            MethodName = method.Name, //str,
                            Path = String.Join(" ", new string[] { path, str }),
                            Clazz = clazz
                        }));
                }
                else if (!method.ReturnType.IsValueType
                    && method.ReturnType != typeof(string)
                    && method.ReturnType != typeof(void))
                {
                    ret.AddRange(GetCommandsFrom(method.ReturnType, path));
                }
                else
                {
                    var descr = (CommandDescriptionAttribute)method.GetCustomAttributes(typeof(CommandDescriptionAttribute), true).FirstOrDefault();
                    ret.Add(new CommandInfo()
                    {
                        Description = descr == null ? paramz.Count().ToString() : descr.Description,
                        MethodName = method.Name,
                        Path = path,
                        Clazz = clazz
                    });
                }
            });

            return ret;
        }

    }
}
