using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;

namespace BinaryAnalysis.Data.Core.Conventions
{
    class ContractStorageConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        public void Accept(FluentNHibernate.Conventions.AcceptanceCriteria.IAcceptanceCriteria<FluentNHibernate.Conventions.Inspections.IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Name=="Contract" && x.Type.Name=="Byte[]");
        }

        public void Apply(FluentNHibernate.Conventions.Instances.IPropertyInstance instance)
        {
            instance.Length(int.MaxValue);
            //instance.CustomSqlType("image");//.CustomType("BinaryBlob");
        }

    }
}
