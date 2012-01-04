using System.Threading;
using Autofac;
using log4net;

namespace BinaryAnalysis.UI.Modules
{
    public class RegisterAllUIModule : Module
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RegisterAllUIModule));
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new FormsLogInjectionModule());
            builder.RegisterModule(new CoreVisualsModule());
        }
    }
}
