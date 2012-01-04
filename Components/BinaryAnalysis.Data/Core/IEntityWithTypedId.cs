using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BinaryAnalysis.Data.Core
{
    public interface IEntityWithTypedId<IdT>
    {
        IdT Id { get; }
        bool IsTransient();
        IEnumerable<PropertyInfo> GetSignatureProperties();
    }
}
