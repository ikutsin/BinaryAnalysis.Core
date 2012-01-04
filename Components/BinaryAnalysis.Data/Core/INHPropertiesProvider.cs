using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Data.Core
{
    public interface INHPropertiesProvider
    {
        Dictionary<string, string> Properties { get; }
    }
}
