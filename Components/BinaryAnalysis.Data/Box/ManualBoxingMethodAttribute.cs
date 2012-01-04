using System;

namespace BinaryAnalysis.Data.Box
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ManualBoxingMethodAttribute : ManualBoxingAttribute
    {
        public ManualBoxingMethodAttribute(string boxMethodName, string unboxMethodName)
        {
            BoxMethodName = boxMethodName;
            UnboxMethodName = unboxMethodName;
        }

        public string BoxMethodName { get; set; }
        public string UnboxMethodName { get; set; }
    }
}