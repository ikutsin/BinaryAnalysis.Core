using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Data.Versioning
{
    public class ChangeSet
    {
        internal ChangeSet() { }
        public DateTime TrackingTime { get; internal set; }
        protected Dictionary<string, object> properties = new Dictionary<string, object>();
        public Dictionary<string, object> Properties { get { return properties; } }
    }
}
