using Autofac;

namespace BA.Examples.ServiceProcess.Modules
{
    public class RegisterAllModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new CommonsServiceModule());
            builder.RegisterModule(new BoxQueryServiceModule());
            builder.RegisterModule(new SchedulerServiceModule());
            builder.RegisterModule(new StateBrowserServiceModule());
        }
    }
}
