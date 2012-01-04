using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Data.Classification
{
    public interface IClassifiable
    {
        int Id { get; }
        string ObjectName { get; }
    }
}
