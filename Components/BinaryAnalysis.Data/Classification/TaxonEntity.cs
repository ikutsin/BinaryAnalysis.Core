using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Settings;
using NHibernate.Validator.Constraints;

namespace BinaryAnalysis.Data.Classification
{
    public class TaxonEntity : Entity, IClassifiable
    {
        public const string OBJECT_NAME = "Taxon";

        [NotNullNotEmpty, Length(120)]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual string ObjectName { get { return OBJECT_NAME; } }
    }
}
