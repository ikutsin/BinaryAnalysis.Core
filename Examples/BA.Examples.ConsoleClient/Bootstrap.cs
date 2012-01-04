using System;
using Autofac;
using Autofac.Configuration;

namespace BA.Examples.ConsoleClient
{
    public class Bootstrap : IServiceProvider, IDisposable
    {
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
