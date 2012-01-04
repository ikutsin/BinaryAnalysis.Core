using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.Data.Index;
using BinaryAnalysis.Data.Core;
using NHibernate.Search.Store;
using Lucene.Net.Analysis.Standard;

namespace BinaryAnalysis.Modularity.Modules.Data
{
    public class IndexModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //NHibernateListeners
            builder.RegisterType<IndexNHListener>()
                .As<ITypedListener>()
                .PropertiesAutowired()
                .SingleInstance();

            //NHIbernate properties
            builder.RegisterType<NHIndexConfig>()
                .As<INHPropertiesProvider>()
                .SingleInstance();

            //manipulation service
            builder.RegisterType<IndexRepository>().InstancePerDependency();
            
            base.Load(builder);
        }
    }
    public class NHIndexConfig : INHPropertiesProvider
    {
        public Dictionary<string, string> Properties
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "hibernate.search.default.directory_provider", typeof(FSDirectoryProvider).AssemblyQualifiedName },
                    { NHibernate.Search.Environment.AnalyzerClass, typeof(StandardAnalyzer).AssemblyQualifiedName },
                    { "hibernate.search.default.indexBase", "~/Index" }
                };
            }
        }
    }
}
