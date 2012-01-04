using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace BinaryAnalysis.UI.Modules
{
    public class FormsLogInjectionModule : Module
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FormsLogInjectionModule));
        private static MemoryAppender appender;

        static FormsLogInjectionModule()
        {
            //BasicConfigurator.Configure();
            XmlConfigurator.Configure();
            log.Debug("LOGGING Initialized");

            Hierarchy h = LogManager.GetLoggerRepository() as Hierarchy;
            appender = h.Root.GetAppender("MemoryAppender") as MemoryAppender;

            if(appender==null) log.Error("MemoryAppender not found, form logging will not work.");
        }

        public static IEnumerable<LoggingEvent> NextLoggingEvent()
        {
            while (true)
            {
                Queue<LoggingEvent> events = new Queue<LoggingEvent>(appender.GetEvents().ToList());
                if (events != null && events.Count > 0)
                {
                    appender.Clear();
                    while(events.Count>0) yield return events.Dequeue();
                }
                else
                {
                    Thread.Sleep(300);
                    yield return null;
                }
            }
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
