using Autofac;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Core.SessionManagement;
using BinaryAnalysis.Extensions.Health.Data;

namespace BinaryAnalysis.Modularity.Modules.Extensions
{
    public class TrackableDataAccessConfigModule : Module
    {
        public TrackableDataAccessConfigModule()
        {
			DatabaseInitMethod = DatabaseInitMethod.DoNothing;
			SessionManager = "PerCall";
		}
        public string SessionManager { get; set; }
        public DatabaseInitMethod DatabaseInitMethod { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            /*
      <component type="BinaryAnalysis.Data.Core.SessionManagement.OnePerCallSessionManager, BinaryAnalysis.Data"
                 service="FluentNHibernate.Cfg.Db.IPersistenceConfigurer, FluentNHibernate" instance-scope="single-instance" />
             */
            builder.RegisterType<OnePerCallSessionManager>().Named<ISessionManager>("PerCall").SingleInstance();
            builder.RegisterType<OnePerRepoSessionManager>().Named<ISessionManager>("PerRepo").SingleInstance();
            builder.RegisterType<SingleSessionManager>().Named<ISessionManager>("Single").SingleInstance();

            builder.RegisterType<TrackedModuledDbContext>().As<IDbContext>()
                .PropertiesAutowired()
                .OnActivating(
                    x =>
                    {
                        x.Instance.Initialize(x.Context.ResolveNamed<ISessionManager>(SessionManager));
                        DbInitializer.Init(DatabaseInitMethod, x.Instance, x.Context);
                    })
                .SingleInstance();
            base.Load(builder);
        }
    }
    
}
