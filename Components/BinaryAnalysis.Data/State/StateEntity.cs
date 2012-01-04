using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.ContractContainer;
using BinaryAnalysis.Data.Core.Impl;
using System.Runtime.Serialization;
using System.IO;
using BinaryAnalysis.Data.Classification;
using NHibernate.Validator.Constraints;

namespace BinaryAnalysis.Data.State
{
    [BoxTo(typeof(StateBoxMap))]
    public class StateEntity : ContractContainerEntity, IClassifiable
    {
        public const string OBJECT_NAME = "State";

        public virtual DateTime DieDate { get; set; }

        [NotNullNotEmpty, Length(300)]
        public virtual string Name { get; set; }

        [Length(2000)]
        public virtual string Description { get; set; }

        public virtual string ObjectName
        {
            get { return OBJECT_NAME; }
        }
    }
}
