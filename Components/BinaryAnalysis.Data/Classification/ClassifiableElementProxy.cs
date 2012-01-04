using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Data.Classification
{
    public class ClassifiableElementProxy : IClassifiable
    {
        public ClassifiableElementProxy(string objName, int id)
        {
            ObjectName = objName;
            Id = id;
        }

        #region Implementation of IClassifiable

        public int Id { get; set; }

        public string ObjectName { get; set; }

        #endregion
    }
}
