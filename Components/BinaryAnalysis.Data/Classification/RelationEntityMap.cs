using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core.Impl;
using FluentNHibernate.Mapping;

namespace BinaryAnalysis.Data.Classification
{
    public class RelationEntityMap : EntityClassMap<RelationEntity>
    {
        public RelationEntityMap()
        {
            References(x => x.Type).Nullable();//.Cascade.None();
            Map(x => x.Direction);
            Map(x => x.ObjectName).Length(30).Not.Nullable();
            Map(x => x.RelatedObjectName).Length(30).Not.Nullable();
            Map(x => x.ObjectID).Not.Nullable();
            Map(x => x.RelatedObjectID).Not.Nullable();
        }
    }
}
