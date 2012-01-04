using System;
using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.State;
using BinaryAnalysis.Data.Core;

namespace BinaryAnalysis.Modularity.Modules.Data
{
    public class StateModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Repository
            builder.RegisterType<StateRepository>().InstancePerDependency();

            //service
            builder.RegisterType<StateService>().InstancePerDependency();

            //config
            builder.RegisterType<NHStateConfig>().As<INHMappingsProvider>().SingleInstance();

            
            base.Load(builder);
        }
    }
    public class NHStateConfig : INHMappingsProvider
    {
        public IList<Type> FluentMappings
        {
            get
            {
                return new List<Type>()
                {
                    typeof(StateEntityMap)
                };
            }
        }
    }
}
