using Autofac;
using BA.Examples.ServiceProcess.Services;

namespace BA.Examples.ServiceProcess.Modules
{
    class CommonsServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //custom WCF services
            builder.RegisterType<CommonsService>().As<ICommonsService>();

            base.Load(builder);
        }
    }
}
