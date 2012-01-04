using System.Linq;
using Autofac;
using BinaryAnalysis.Data.Core.Conventions;
using FluentNHibernate.Conventions;

namespace BinaryAnalysis.Modularity.Modules.Data
{
    public class DefaultFluentConventionsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Fluent conventions
            builder.RegisterAssemblyTypes(typeof(DateTimeStorageConvention).Assembly)
                .Where(c => c.GetInterfaces().Contains(typeof(IConvention)))
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .SingleInstance();

            builder.Register(c => FluentNHibernate.Conventions.Helpers.DefaultLazy.Never()).AsImplementedInterfaces();

            base.Load(builder);
        }
    }
}
