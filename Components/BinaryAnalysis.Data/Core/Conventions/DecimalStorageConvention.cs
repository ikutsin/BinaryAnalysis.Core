using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;

namespace BinaryAnalysis.Data.Core.Conventions
{
    public class DecimalStorageConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        public void Apply(FluentNHibernate.Conventions.Instances.IPropertyInstance instance)
        {
            instance.CustomType("decimal(25,5)");
        }

        public void Accept(FluentNHibernate.Conventions.AcceptanceCriteria.IAcceptanceCriteria<FluentNHibernate.Conventions.Inspections.IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Type == typeof(decimal?));
        }
    }
}
