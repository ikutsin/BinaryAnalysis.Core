using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.ContractContainer;
using BinaryAnalysis.Data.Core.Impl;
using NHibernate.Validator.Constraints;
using System.Runtime.Serialization;
using System.IO;

namespace BinaryAnalysis.Data.Versioning
{
    public class TrackingEntity : ContractContainerEntity
    {
        [NotNullNotEmpty, Length(30)]
        public virtual string ObjectName { get; set; }
        [Min(1)]
        public virtual int ObjectID { get; set; }
        [NotNullNotEmpty, Length(30)]
        public virtual string PropertyName { get; set; }
        public virtual DateTime TrackingTime { get; set; }
    }
}
