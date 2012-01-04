using System;
using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Versioning;

namespace BinaryAnalysis.Modularity.Modules.Data
{
    public class TrackingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Repository
            builder.RegisterType<TrackingRepository>().InstancePerDependency();

            //config
            builder.RegisterType<NHTrackingConfig>().As<INHMappingsProvider>().SingleInstance();

            base.Load(builder);
        }
    }
    public class NHTrackingConfig : INHMappingsProvider
    {
        public IList<Type> FluentMappings
        {
            get
            {
                return new List<Type>()
                {
                    typeof(TrackingEntityMap)
                };
            }
        }
    }
}
