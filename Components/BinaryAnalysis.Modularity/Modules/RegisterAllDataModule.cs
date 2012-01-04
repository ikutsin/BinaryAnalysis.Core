using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Modularity.Modules.Data;
using log4net;

namespace BinaryAnalysis.Modularity.Modules
{
    public class RegisterAllDataModule : Module
    {
        public string SessionManager { get; set; }
        public DatabaseInitMethod DatabaseInitMethod { get; set; }

        private static readonly ILog log = LogManager.GetLogger(typeof(RegisterAllDataModule));
        protected override void Load(ContainerBuilder builder)
        {
            //BA.Data modules
            //builder.RegisterModule(new DataAccessConfigModule() 
            //    { 
            //        SessionManager = SessionManager,
            //        DatabaseInitMethod = DatabaseInitMethod
            //    });
            builder.RegisterModule(new DefaultFluentConventionsModule());
            builder.RegisterModule(new IndexModule());
            builder.RegisterModule(new TaxonomyModule());
            builder.RegisterModule(new SettingsModule());
            builder.RegisterModule(new StateModule());
            builder.RegisterModule(new ActivityLogModule());
            builder.RegisterModule(new TrackingModule());
            builder.RegisterModule(new MetricsModule());
            builder.RegisterModule(new BoxQueryModule());
        }
    }
}
