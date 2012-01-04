using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.ContractContainer;
using BinaryAnalysis.Data.Core.Impl;
using FluentNHibernate.Mapping;

namespace BinaryAnalysis.Data.State
{
    public class StateEntityMap : ContractContainerClassMap<StateEntity>
    {
        public StateEntityMap()
        {
            Map(x => x.Name).Not.Nullable().Unique();
            Map(x => x.Description);
            Map(x => x.DieDate);
        }
    }
}
