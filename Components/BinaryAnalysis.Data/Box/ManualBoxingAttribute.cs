using System;

namespace BinaryAnalysis.Data.Box
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ManualBoxingAttribute : Attribute
    {
        protected ManualBoxingAttribute()
        {
        }
    }
}