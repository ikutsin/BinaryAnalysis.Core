using System;
using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core;

namespace BinaryAnalysis.Modularity.Modules.Data
{
    public class TaxonomyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Repositories
            builder.RegisterType<RelationRepository>().InstancePerDependency();
            builder.RegisterType<TaxonRepository>().InstancePerDependency();

            //Services
            builder.RegisterType<RelationService>().SingleInstance();
            builder.RegisterType<TaxonomyTree>().SingleInstance();

            //config
            builder.RegisterType<NHTaxonomyConfig>().As<INHMappingsProvider>().SingleInstance();
            
            base.Load(builder);
        }
    }
    public class NHTaxonomyConfig : INHMappingsProvider
    {
        public IList<Type> FluentMappings
        {
            get
            {
                return new List<Type>()
                {
                    typeof(TaxonEntityMap), typeof(RelationEntityMap)
                };
            }
        }
    }
}
