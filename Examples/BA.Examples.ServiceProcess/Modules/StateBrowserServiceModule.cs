using Autofac;
using BA.Examples.ServiceProcess.Services;

namespace BA.Examples.ServiceProcess.Modules
{
    class StateBrowserServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //custom WCF services
            builder.RegisterType<StateBrowsingService>().As<IStateBrowsingService>();

            base.Load(builder);
        }
    }
}
