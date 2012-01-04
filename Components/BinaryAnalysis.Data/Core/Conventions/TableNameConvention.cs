using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace BinaryAnalysis.Data.Core.Conventions
{
    public class TableNameConvention : IClassConvention
    {

        public void Apply(IClassInstance instance)
        {
            instance.Table(instance.EntityType.Name.Replace("Entity", ""));
        }
    }
}
