using System.ServiceModel;

namespace BA.Examples.ServiceProcess.Services.Core
{
    [ServiceContract]
    public interface ISubscriptionService
    {
        [OperationContract]
        void Subscribe(string eventOperation);

        [OperationContract]
        void Unsubscribe(string eventOperation);
    }
}
