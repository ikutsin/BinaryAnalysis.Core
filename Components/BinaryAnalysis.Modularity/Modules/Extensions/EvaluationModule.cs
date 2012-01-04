using Autofac;
using BinaryAnalysis.Extensions.JSEvaluator;

namespace BinaryAnalysis.Modularity.Modules.Extensions
{
    public class EvaluationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //services
            builder.RegisterType<JSEvalService>().SingleInstance();
            
            base.Load(builder);
        }
    }
}
