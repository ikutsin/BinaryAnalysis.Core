using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Index;
using BinaryAnalysis.Data.Log;

namespace BinaryAnalysis.Modularity.Modules.Data
{
    class ActivityLogModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Repository
            builder.RegisterType<ActivityLogRepository>().InstancePerDependency();

            //service
            builder.RegisterType<ActivityLogService>().InstancePerDependency();

            //config
            builder.RegisterType<NHActivityLogConfig>().As<INHMappingsProvider>().SingleInstance();


            base.Load(builder);
        }
        public class NHActivityLogConfig : INHMappingsProvider
        {
            public IList<Type> FluentMappings
            {
                get
                {
                    return new List<Type>()
                {
                    typeof(ActivityLogEntityMap)
                };
                }
            }
        }
    }
}
