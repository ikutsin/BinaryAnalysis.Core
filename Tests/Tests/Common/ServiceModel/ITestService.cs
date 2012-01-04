using System.ServiceModel;
using BA.Examples.ServiceProcess.Services.Core;

namespace BinaryAnalysis.Tests.Common.ServiceModel
{
    [ServiceContract(CallbackContract = typeof(ITestServiceEvents))]
    public interface ITestService : ISubscriptionService
    {
        [OperationContract]
        void DoWork(string txt);
    }
}
