using Autofac;

namespace BA.Examples.ServiceClient.Modules
{
    class AllServiceClientModulesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new CommonsClientModule());
            builder.RegisterModule(new SchedulerClientModule());
            builder.RegisterModule(new BoxQueryClientModule());
            builder.RegisterModule(new StateBrowserClientModule());
        }
    }
}
