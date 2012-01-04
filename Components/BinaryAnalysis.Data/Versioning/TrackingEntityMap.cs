using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.ContractContainer;
using BinaryAnalysis.Data.Core.Impl;
using FluentNHibernate.Mapping;

namespace BinaryAnalysis.Data.Versioning
{
    public class TrackingEntityMap : ContractContainerClassMap<TrackingEntity>
    {
        public TrackingEntityMap()
        {
            Map(x => x.ObjectName).Length(30).Not.Nullable();
            Map(x => x.ObjectID).Not.Nullable();

            Map(x => x.TrackingTime);
            Map(x => x.PropertyName).Length(30).Not.Nullable();
        }
    }
}
