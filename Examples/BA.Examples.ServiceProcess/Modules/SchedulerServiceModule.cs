using Autofac;
using BA.Examples.ServiceProcess.Services;

namespace BA.Examples.ServiceProcess.Modules
{
    class SchedulerServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //custom WCF services
            builder.RegisterType<SchedulerService>().As<ISchedulerService>();

            base.Load(builder);
        }
    }
}
