using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using BinaryAnalysis.Modularity.Modules.Extensions;
using log4net;

namespace BinaryAnalysis.Modularity.Modules
{
    public class RegisterAllExtensionsModule : Module
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RegisterAllExtensionsModule));

        public int StateStoringDurationSeconds { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            /*
        <Module type="BinaryAnalysis.Modularity.Modules.Extensions.FileStorageModule, BinaryAnalysis.Modularity" />
        <Module type="BinaryAnalysis.Modularity.Modules.Extensions.EvaluationModule, BinaryAnalysis.Modularity" />
        <Module type="BinaryAnalysis.Modularity.Modules.Extensions.BrowsingModule, BinaryAnalysis.Modularity" />
        <Module type="BinaryAnalysis.Modularity.Modules.Extensions.HttpProxyDataModule, BinaryAnalysis.Modularity" />
             */

            //NS:BinaryAnalysis.Scheduler and Extensions
            builder.RegisterModule(new FileStorageModule());
            builder.RegisterModule(new EvaluationModule());
            BrowsingModule browsingModule = new BrowsingModule();
            browsingModule.StateStoringDurationSeconds = StateStoringDurationSeconds;
            builder.RegisterModule(browsingModule);
            builder.RegisterModule(new HttpProxyDataModule());
            builder.RegisterModule(new HealthModule());
        }
    }
}
