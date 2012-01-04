using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Data.ContractContainer
{
    public interface IContractContainer
    {
        string StringValue { get; }
        decimal? NumericValue { get; }
        byte[] Contract { get; }
    }
}
