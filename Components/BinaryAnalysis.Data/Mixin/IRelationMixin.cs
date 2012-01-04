using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Data.Mixin
{
    public interface IRelationMixin
    {
        int Id { get; set; }
        string Name { get; set; }
    }
}
