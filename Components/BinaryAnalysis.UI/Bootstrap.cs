using System;
using Autofac;
using Autofac.Configuration;
using BinaryAnalysis.UI.Modules;
using log4net;

namespace BinaryAnalysis.UI
{
    public class Bootstrap : IServiceProvider, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Bootstrap));
        public static Bootstrap Instance;

        internal IContainer Container { get; private set; }

        public void Init()
        {
            if(Instance!=null) throw new InvalidOperationException("Bootstrap already initialized");
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
            Container = builder.Build();
            Instance = this;            
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
