using System;
using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Metrics;

namespace BinaryAnalysis.Modularity.Modules.Data
{
    public class MetricsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Repository
            builder.RegisterType<MetricsRepository>().InstancePerDependency();
            builder.RegisterType<MetricsEntryRepository>().InstancePerDependency();

            //config
            builder.RegisterType<NHMetricsConfig>().As<INHMappingsProvider>().SingleInstance();

            //service
            builder.RegisterType<MetricsService>().SingleInstance();

            //actions
            builder.RegisterType<BulkMetricsImportAction>().InstancePerDependency();
        }
    }
    public class NHMetricsConfig : INHMappingsProvider
    {
        public IList<Type> FluentMappings
        {
            get
            {
                return new List<Type>()
                {
                    typeof(MetricsEntityMap), 
                    typeof(MetricsEntryEntityMap)
                };
            }
        }
    }
}
