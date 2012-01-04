using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Scheduler.Task.Script
{
    public class ScriptUtilityCommands
    {
        IEnumerable<Lazy<IScriptUtilityCommand, IScriptUtilityCommandMetadata>> utilityCommands { get; set; }

        public IList<string> CommandNames { get; internal set; }

        public ScriptUtilityCommands(
            IEnumerable<Lazy<IScriptUtilityCommand, IScriptUtilityCommandMetadata>> utilityCommands)
        {
            this.utilityCommands = utilityCommands;
            CommandNames = utilityCommands.Select(x => x.Metadata.CommandName).ToList();
        }

        public void Exec(ScriptUtility xx, string name, object input = null)
        {
            var command = utilityCommands.FirstOrDefault(x => x.Metadata.CommandName == name);
            if(command==null)
            {
                throw new Exception(String.Format("Command '{0}' not found", name));
            }
            if (command.Value.ReturnType != typeof(void))
            {
                throw new Exception(String.Format("Invalid returntype '{0}' was '{1}' expected '{2}'", name, "Void", command.Value.ReturnType));
            }
            if (input != null)
            {
                if (!command.Value.InputType.IsAssignableFrom(input.GetType()))
                {
                    throw new Exception(String.Format("Invalid input type '{0}' was '{1}' expected '{2}'", name,
                                                      input.GetType(), command.Value.InputType));
                }
            }
            else if (command.Value.InputType != typeof(void))
            {
                throw new Exception(String.Format("Input expected type '{1}' {0}", command.Value.InputType, name));
            }
            command.Value.Execute(xx, input);
        }
        public T Exec<T>(ScriptUtility xx, string name, object input = null)
        {
            var command = utilityCommands.FirstOrDefault(x => x.Metadata.CommandName == name);
            if (command == null)
            {
                throw new Exception(String.Format("Command '{0}' not found", name));
            }
            if (command.Value.ReturnType != typeof (T))
            {
                throw new Exception(String.Format("Invalid returntype '{0}' was '{1}' expected '{2}'", name, typeof (T),
                                                  command.Value.ReturnType));
            }
            if (input != null) {
                if (!command.Value.InputType.IsAssignableFrom(input.GetType()))
                {
                    throw new Exception(String.Format("Invalid input type '{0}' was '{1}' expected '{2}'", name,
                                                      input.GetType(), command.Value.InputType));
                }
            }
            else if (command.Value.InputType != typeof(void))
            {
                throw new Exception(String.Format("Input '{1}' expected type {0}", command.Value.InputType, name));
            }

            return (T)command.Value.Execute(xx, input);
        }
    }
}
