using System.Collections.Generic;
using System.ServiceModel;
using BA.Examples.ServiceProcess.Services.Core;

namespace BA.Examples.ServiceProcess.Services
{
    [ServiceContract(CallbackContract = typeof(ISchedulerServiceEvents))]
    public interface ISchedulerService : ISubscriptionService
    {
        [OperationContract]
        List<string> GetAvailableNamedGoals();

        [OperationContract]
        bool ScheduleNamedGoal(string name);
    }
}
