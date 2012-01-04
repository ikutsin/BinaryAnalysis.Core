using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.ContractContainer;
using BinaryAnalysis.Data.State;

namespace BinaryAnalysis.Data.Log
{
    public class ActivityLogEntityMap : ContractContainerClassMap<ActivityLogEntity>
    {
        public ActivityLogEntityMap()
        {
            Map(x => x.Creation);
            Map(x => x.ClassifiableId);
            Map(x => x.ClassifiableName);
            Map(x => x.Descriminator);
            Map(x => x.Message);
            Map(x => x.Level);
        }
    }
}
