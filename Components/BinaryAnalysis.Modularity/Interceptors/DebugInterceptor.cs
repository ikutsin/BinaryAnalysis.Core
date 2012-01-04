using Castle.DynamicProxy;
using log4net;

namespace BinaryAnalysis.Modularity.Interceptors
{
    public class DebugInterceptor : IInterceptor
    {
        public ILog Log { get; set; }
        public void Intercept(IInvocation invocation)
        {
            Log.Info("Before invocation");
            invocation.Proceed();
            Log.Info("After invocation");
        }

    }
}
