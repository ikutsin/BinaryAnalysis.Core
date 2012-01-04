using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.ContractContainer;
using NHibernate.Validator.Constraints;

namespace BinaryAnalysis.Data.Log
{
    public enum ActivityLogLevel
    {
        Debug, Info, Warn, Error, Fatal
    }
    public class ActivityLogEntity : ContractContainerEntity, IClassifiable
    {
        public ActivityLogEntity()
        {
            Creation = DateTime.Now;
        }
        public virtual DateTime Creation { get; set; }
        [NotNullNotEmpty, Length(60)]
        public virtual string Descriminator { get; set; }

        [NotNullNotEmpty, Length(600)]
        public virtual string Message { get; set; }

        [Length(60)]
        public virtual string ClassifiableName { get; set; }
        public virtual int ClassifiableId { get; set; }

        public virtual ActivityLogLevel Level { get; set; }

        public virtual string ObjectName
        {
            get { return "ActivityLog"; }
        }
    }
}
