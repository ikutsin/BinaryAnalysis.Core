using System;

namespace BinaryAnalysis.Terminal.Commanding
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandDescriptionAttribute : Attribute 
    {
        public string Description { get; set; }
        public CommandDescriptionAttribute(string Description)
        {
            this.Description = Description;
        }
    }
}
