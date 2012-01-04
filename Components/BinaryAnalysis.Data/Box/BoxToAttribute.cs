using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BinaryAnalysis.Data.Box
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class BoxToAttribute : Attribute
    {
        public BoxToAttribute(Type boxingType)
        {
            this.BoxingType = boxingType;
        }
        public Type BoxingType { get; set;}
    }
}
