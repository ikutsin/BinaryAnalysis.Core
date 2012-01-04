using System;
using System.Collections.Generic;
using System.Reflection;
using BinaryAnalysis.Box;
using BinaryAnalysis.Extensions.HttpProxy.Data;
using BinaryAnalysis.Scheduler.Scheduler.Data;

namespace BA.Examples.ServiceProcess
{
    public class KnownTypeRegistry
    {
        public static void RegisterKnownTypes()
        {
            //common
            RegisterType<List<string>>();

            //box query types
            RegisterType<HttpProxyBoxMap>();
            RegisterType<List<HttpProxyBoxMap>>();

            //scheduler service
            RegisterType<ScheduleBoxMap>();
            RegisterType<List<ScheduleBoxMap>>();
            RegisterType<IBox<ScheduleBoxMap>>();

            RegisterType<RecurrencyBoxMap>();
            RegisterType<List<RecurrencyBoxMap>>();
            RegisterType<IBox<RecurrencyBoxMap>>();
        }

        public static ICollection<Type> KnownTypes;

        static KnownTypeRegistry()
        {
            KnownTypes = KnownTypes ?? new List<Type>();
        }

        public static void RegisterType<T>()
        {
            if (!KnownTypes.Contains(typeof(T))) KnownTypes.Add(typeof(T));
        }

        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            return new List<Type>(KnownTypes);
        }
    }
}
