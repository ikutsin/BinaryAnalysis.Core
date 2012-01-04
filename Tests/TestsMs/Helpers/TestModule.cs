using Autofac;

namespace BinaryAnalysis.TestsMs.Helpers
{
    public class TestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<SessionSyncGoal>();

            base.Load(builder);
        }
    }
}
