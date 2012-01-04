using System;
using System.Collections.Generic;
using System.Linq;

namespace BinaryAnalysis.Extensions.JSEvaluator
{
    public class JsValue
    {
        public string Name { get; internal set; }
        public string Value { get; internal set; }
        public List<JsValue> Values { get; internal set; }

        public JsValue this[string key]
        {
            get
            {
                if (Values == null) return null;
                return Values.FirstOrDefault(x => x.Name == key);
            }
        }

        public override string ToString()
        {
            return Name + (Value == null ? "" : "=" + Value)
                  + (Values == null ? "" : "={" + String.Join(",", Values)+"}");
        }
    }
}
