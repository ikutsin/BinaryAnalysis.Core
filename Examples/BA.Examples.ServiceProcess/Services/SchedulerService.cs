using System;
using System.Collections.Generic;
using System.ServiceModel;
using BA.Examples.ServiceProcess.Services.Core;
using BinaryAnalysis.Scheduler.Scheduler;
using log4net;

namespace BA.Examples.ServiceProcess.Services
{
    class SchedulerServiceEventsPublisher : PublishService<ISchedulerServiceEvents>, ISchedulerServiceEvents
    {
    }

    [ServiceKnownType("GetKnownTypes", typeof(KnownTypeRegistry))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class SchedulerService : SubscriptionManager<ISchedulerServiceEvents>, ISchedulerService
    {
        private ILog log;

        public SchedulerService(ILog log)
        {
            this.log = log;
        }

        public bool ScheduleNamedGoal(string name)
        {
            return SchedulerInstance.Instance.Schedule(name);
        }

        public List<string> GetAvailableNamedGoals()
        {
            throw new Exception("TODO");
            //return SchedulerInstance.Instance.GetAvailableNamedGoals();
        }
    }
}
