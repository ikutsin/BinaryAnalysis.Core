using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using BinaryAnalysis.Browsing.Windowless;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Core.SessionManagement;
using BinaryAnalysis.Extensions.Browsing;
using BinaryAnalysis.Extensions.Health;

namespace BinaryAnalysis.Modularity.Modules.Extensions
{
    class HealthModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<LogHealthTrackingService>()
            //    .As<IHealthTrackingService>()
            //    .SingleInstance();

            builder.RegisterType<MetricsHealthTrackingService>()
                .As<IHealthTrackingService>()
                .SingleInstance();

            builder.RegisterType<FrequencyTrackHelper>().InstancePerDependency();
            builder.RegisterType<DurationTrackHelper>().InstancePerDependency();

            builder.RegisterType<SchedulerServiceTracker>().SingleInstance();
            builder.RegisterType<PerformanceCountersTracker>().SingleInstance();

            builder.RegisterType<RandomTracker>().SingleInstance();

            builder.RegisterType<BandwidthTrackerDecorator>()
                .Named<IBrowsingSessionDecorator>("BandwidthTracker").SingleInstance();

            builder.RegisterCallback(
                (c) =>
                    {
                        var activityLogRegistration =
                            c.Registrations.Where(r => r.Activator.LimitType == typeof(ActivityLogService))
                                .FirstOrDefault();
                        activityLogRegistration.Activated +=
                            (s, x) =>
                                {
                                    var logFreq = x.Context.Resolve<FrequencyTrackHelper>();
                                    logFreq.Start(TimeSpan.FromMinutes(1), "logFreq");
                                    (x.Instance as ActivityLogService)
                                        .OnNewLog += (_) => logFreq.Notify();
                                };

                        var browsingSessionRegistration =
                            c.Registrations.Where(r => r.Activator.LimitType == typeof (StatefullBrowsingSessionWrapper))
                                .FirstOrDefault();
                        browsingSessionRegistration.Activating +=
                            (s,x) =>
                                {
                                    (x.Instance as IBrowsingSession)
                                        .AddDecorator(x.Context.ResolveNamed<IBrowsingSessionDecorator>("BandwidthTracker"));
                                };
                    });
        }
    }
}
