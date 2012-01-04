using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Extensions.Health
{
    public interface IHealthTrackingService
    {
        void Track(TimeSpan value, string metrixName, string name=null);
        void Track(int value, string metrixName, string name = null);
        void Track(decimal value, string metrixName, string name = null);
    }
}
