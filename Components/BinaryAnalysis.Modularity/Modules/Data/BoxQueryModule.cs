using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.Box.Transforations;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Settings;

namespace BinaryAnalysis.Modularity.Modules.Data
{
    public class BoxQueryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //config
            builder.RegisterType<NHBoxConfig>().As<INHPropertiesProvider>().SingleInstance();

            builder.RegisterType<RepositoryFinder>()
                .PropertiesAutowired()
                .InstancePerDependency();

            //transformations
            builder.RegisterGeneric(typeof (FileBoxTransformation<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof (NHibernateBoxTransformation<,>)).InstancePerDependency();
            builder.RegisterType<BackupsResolver>().InstancePerDependency();
            

            //initialize evaluator
            builder.RegisterType<NHibernateBoxQueryEvaluator>()
                .PropertiesAutowired()
                .SingleInstance();

            builder.RegisterGeneric(typeof(BoxQuery<>))
                .OnActivated(x =>
                {
                    if (BoxedQueryRemoteExtensions.DefaultEvaluator == null)
                        BoxedQueryRemoteExtensions.DefaultEvaluator = x.Context.Resolve<NHibernateBoxQueryEvaluator>().Evaluate;
                });

            base.Load(builder);
        }
    }
    public class NHBoxConfig : INHPropertiesProvider
    {
        public Dictionary<string, string> Properties
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "adonet.batch_size", "20" }
                };
            }
        }
    }
}
