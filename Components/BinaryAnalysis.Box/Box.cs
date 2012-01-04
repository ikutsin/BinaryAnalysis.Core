using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace BinaryAnalysis.Box
{
    [Serializable, XmlType("Box"), DataContract, JsonArray]
    public class Box<T> : List<T>, IBox<T>
    {
        public Box()
        {
            
        }
        public Box(T el)
        {
            Add(el);
        }
        public Box(IEnumerable list = null)
        {
            if (list != null)
            {
                foreach (var obj in list)
                {
                    Add((T) obj);
                }
            }
        }
    }
}
