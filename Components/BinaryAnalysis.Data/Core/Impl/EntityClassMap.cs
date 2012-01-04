using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace BinaryAnalysis.Data.Core.Impl
{
    public class EntityClassMap<T> : ClassMap<T> where T : Entity
    {
        public EntityClassMap()
        {
            Id(x => x.Id);
        }
    }
}
