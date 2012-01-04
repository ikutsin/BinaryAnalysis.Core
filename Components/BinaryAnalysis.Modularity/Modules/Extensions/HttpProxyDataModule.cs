using System;
using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.Browsing.Windowless;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Extensions.HttpProxy;
using BinaryAnalysis.Extensions.HttpProxy.Control;
using BinaryAnalysis.Extensions.HttpProxy.Data;

namespace BinaryAnalysis.Modularity.Modules.Extensions
{
    public class HttpProxyDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //session decorators
            //builder.RegisterType<HttpProxyDecorator>()
            builder.RegisterType<ParallelHttpProxyDecorator>()
                .Named<IBrowsingSessionDecorator>("AutoHttpProxy").InstancePerDependency();

            //http proxy browsing session
            builder.RegisterType<BrowsingSession>().InstancePerDependency()
                .Named<IBrowsingSession>("HttpProxy")
                .OnActivating(x => x.Instance.AddDecorator(x.Context.ResolveNamed<IBrowsingSessionDecorator>("AutoHttpProxy")));

            //Repository
            builder.RegisterType<HttpProxyRepository>().InstancePerDependency();

            //config
            builder.RegisterType<NHHttpProxyConfig>().SingleInstance()
                .As<INHPropertiesProvider>()
                .As<INHMappingsProvider>();

            //backup
            builder.RegisterType<HttpProxyFileBackup>().InstancePerDependency();

        }
    }
    public class NHHttpProxyConfig : INHMappingsProvider, INHPropertiesProvider
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
        public IList<Type> FluentMappings
        {
            get
            {
                return new List<Type>()
                {
                    typeof(HttpProxyEntityMap),
                };
            }
        }
    }
}
