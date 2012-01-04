using System;
using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Settings;
using BinaryAnalysis.Data.Core;

namespace BinaryAnalysis.Modularity.Modules.Data
{
    public class SettingsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Repository
            builder.RegisterType<SettingsEntryRepository>().InstancePerDependency();
            builder.RegisterType<SettingsRepository>().InstancePerDependency();

            //service
            builder.RegisterType<SettingsService>().SingleInstance();

            //config
            builder.RegisterType<NHSettingsConfig>().As<INHMappingsProvider>().SingleInstance();

            base.Load(builder);
        }
    }
    public class NHSettingsConfig : INHMappingsProvider
    {
        public IList<Type> FluentMappings
        {
            get
            {
                return new List<Type>()
                {
                    typeof(SettingsEntityMap), typeof(SettingsEntryEntityMap)
                };
            }
        }
    }
}
