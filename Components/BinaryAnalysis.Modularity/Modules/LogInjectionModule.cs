using System.Linq;
using Autofac;
using log4net;
using log4net.Config;
using Autofac.Core;

namespace BinaryAnalysis.Modularity.Modules
{
    public class LogInjectionModule : Module
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LogInjectionModule));

        static LogInjectionModule()
        {
            //BasicConfigurator.Configure();
            XmlConfigurator.Configure();
            log.Debug("LOGGING Initialized");
        }
        protected override void AttachToComponentRegistration(IComponentRegistry registry, IComponentRegistration registration)
        {
            registration.Preparing += OnComponentPreparing;
            registration.Activated += OnComponentActivated;
        }

        void OnComponentActivated(object sender, ActivatedEventArgs<object> e)
        {
            //var loggingAware = e.Instance as ILoggingAware;
            //if (loggingAware != null)
            //{
            //    var t = e.Component.Activator.LimitType;
            //    loggingAware.Log = LogManager.GetLogger(t);
            //}
        }

        static void OnComponentPreparing(object sender, PreparingEventArgs e)
        {
            var t = e.Component.Activator.LimitType;
            e.Parameters = e.Parameters.Union(new[]
            {
                new ResolvedParameter((p, i) => p.ParameterType == typeof(ILog), (p, i) => LogManager.GetLogger(t))
            });
        }
    }
}
