using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Configuration;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Index;
using log4net;

namespace BinaryAnalysis.Modularity
{
    public class Bootstrap : IServiceProvider, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Bootstrap));
        public IContainer Container { get; protected set; }
		
        public Bootstrap(Action<ContainerBuilder> beforeBuild = null)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
			if(beforeBuild!=null)beforeBuild(builder);
            Container = builder.Build();
        }

        public object GetService(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}

