using Autofac;
using BA.Examples.ServiceProcess.Services;
using BinaryAnalysis.Data.Box;

namespace BA.Examples.ServiceProcess.Modules
{
    public class BoxQueryServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //register service
            builder.RegisterType<BoxQueryService>().As<IBoxQueryService>()
                .OnActivated(x =>
                {
                    if (BoxedQueryRemoteExtensions.DefaultEvaluator == null)
                        BoxedQueryRemoteExtensions.DefaultEvaluator = x.Context.Resolve<NHibernateBoxQueryEvaluator>().Evaluate;
                });

            //known types
            KnownTypeRegistry.RegisterKnownTypes();

            base.Load(builder);
        }
    }
}
