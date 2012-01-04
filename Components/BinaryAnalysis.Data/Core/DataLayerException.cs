using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Data.Core
{
    [Serializable]
    public class DataLayerException : Exception
    {
        public DataLayerException(string message) : base(message) {}
        public DataLayerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
