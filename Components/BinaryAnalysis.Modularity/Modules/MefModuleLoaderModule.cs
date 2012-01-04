using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using Autofac;
using Autofac.Core;
using log4net;

namespace BinaryAnalysis.Modularity.Modules
{
    public class MefModuleLoaderModule : Module
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MefModuleLoaderModule));

        [ImportMany]
        private IEnumerable<IModule> RegisteredModules;

        private DirectoryCatalog catalog;
        /// <summary>
        /// Autofac property for setting
        /// </summary>
        public string ExtensionsFolder { get; set; }

        public MefModuleLoaderModule()
        {
            ExtensionsFolder = "extensions";
        }

        public string WorkingFolder
        {
            get
            {
                return (Path.Combine(Environment.CurrentDirectory, ExtensionsFolder));
            }
        }

        private void ImportModules()
        {
            //AppDomain currentDomain = AppDomain.CurrentDomain;
            //currentDomain.AppendPrivatePath(WorkingFolder);

            try
            {
                catalog = new DirectoryCatalog(ExtensionsFolder); // WorkingFolder);
                var container = new CompositionContainer(catalog);

                var batch = new CompositionBatch();
                batch.AddPart(this);
                container.Compose(batch);
            }
            catch(Exception ex)
            {
                log.Error("MEF module loader failed", ex);
            }
        }

        protected override void Load(ContainerBuilder builder)
        {
            ImportModules();
            if (RegisteredModules != null)
            {
                foreach (var module in RegisteredModules)
                {
                    //AppDomain.CurrentDomain.Load(Path.Combine(WorkingFolder, module.GetType().Assembly.GetName()));

                    log.Debug("MEF registered module: " + module);
                    builder.RegisterModule(module);
                }
            }
            base.Load(builder);
        }
    }
}
