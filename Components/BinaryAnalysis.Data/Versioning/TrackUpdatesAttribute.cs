using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Data.Versioning
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TrackUpdatesAttribute : Attribute
    {
        public string Name;
    }
}
