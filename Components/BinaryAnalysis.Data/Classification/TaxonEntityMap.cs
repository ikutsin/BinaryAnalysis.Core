using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core.Impl;
using FluentNHibernate.Mapping;

namespace BinaryAnalysis.Data.Classification
{
    public class TaxonEntityMap : EntityClassMap<TaxonEntity>
    {
        public TaxonEntityMap()
        {
            Map(x => x.Name).Not.Nullable();
            Map(x => x.Description).CustomType("StringClob");
        }
    }
}
