using System;
using Autofac;
using Autofac.Configuration;
using log4net;

namespace BA.Examples.ScriptingHelper
{
    public class Bootstrap : IServiceProvider, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Bootstrap));

        public IContainer Container { get; protected set; }
        public Bootstrap()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
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
