using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core.SessionManagement;
using BinaryAnalysis.Data.Index;
using BinaryAnalysis.Data.Settings;
using FluentNHibernate.Cfg.Db;

namespace BinaryAnalysis.Modularity.Modules.Data
{
    public class DataAccessConfigModule : Module
    {
		public DataAccessConfigModule() {
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

            builder.RegisterType<ModuledDbContext>().As<IDbContext>()
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
