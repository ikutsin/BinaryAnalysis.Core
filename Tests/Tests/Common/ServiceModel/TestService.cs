using System.ServiceModel;
using BA.Examples.ServiceProcess.Services.Core;

namespace BinaryAnalysis.Tests.Common.ServiceModel
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class TestService : SubscriptionManager<ITestServiceEvents>, ITestService
    {
        TestServiceEventsPublisher thisPublisher = new TestServiceEventsPublisher();
        public void DoWork(string txt)
        {
            thisPublisher.OnEvent1(txt);
        }
    }
}
