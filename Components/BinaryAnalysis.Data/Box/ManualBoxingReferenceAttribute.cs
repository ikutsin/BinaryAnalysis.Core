using System;

namespace BinaryAnalysis.Data.Box
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ManualBoxingReferenceAttribute : ManualBoxingAttribute
    {
        public ManualBoxingReferenceAttribute()
        {
        }
    }
}