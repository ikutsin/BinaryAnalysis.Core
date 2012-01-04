using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BinaryAnalysis.Box
{
    public static class BoxDataContracts
    {
        static ICollection<Type> KnownTypes;

        static BoxDataContracts()
        {
            KnownTypes = KnownTypes ?? new List<Type>();
        }

        public static void RegisterDataContractType<T>()
        {
            if (!KnownTypes.Contains(typeof(T))) KnownTypes.Add(typeof(T));
        }

        public static IEnumerable<Type> GetDataContractTypes(ICustomAttributeProvider provider = null)
        {
            return new List<Type>(KnownTypes);
        }
    }
}
