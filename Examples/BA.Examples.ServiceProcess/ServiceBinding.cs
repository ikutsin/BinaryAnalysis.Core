using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Integration.Wcf;
using System.ServiceModel;
using BA.Examples.ServiceProcess.Services;

namespace BA.Examples.ServiceProcess
{
    //Port sharing: http://msdn.microsoft.com/en-us/library/ms731810.aspx
    public class ServiceBinding
    {
        IContainer container;
        List<ServiceHost> hosts = new List<ServiceHost>();

        public ServiceBinding(IContainer container)
        {
            this.container = container;
        }
        public void Start()
        {
            //SchedulerService
            var schedulerServiceHost = new ServiceHost(typeof(SchedulerService), 
                new Uri("net.pipe://localhost/SchedulerService"));
            schedulerServiceHost.AddServiceEndpoint(typeof(ISchedulerService), new NetNamedPipeBinding(), string.Empty);
            schedulerServiceHost.AddDependencyInjectionBehavior<ISchedulerService>(container);
            hosts.Add(schedulerServiceHost);

            //BoxQueryService
            var boxQueryService = new ServiceHost(typeof(BoxQueryService),
                new Uri("net.pipe://localhost/BoxQueryService"));
            boxQueryService.AddServiceEndpoint(typeof(IBoxQueryService), new NetNamedPipeBinding(), string.Empty);
            boxQueryService.AddDependencyInjectionBehavior<IBoxQueryService>(container);
            hosts.Add(boxQueryService);
            
            //BoxQueryService
            var commonsService = new ServiceHost(typeof(CommonsService),
                new Uri("net.pipe://localhost/CommonsService"));
            commonsService.AddServiceEndpoint(typeof(ICommonsService), new NetNamedPipeBinding(), string.Empty);
            commonsService.AddDependencyInjectionBehavior<ICommonsService>(container);
            hosts.Add(commonsService);

            //StateBrowsingService
            var stateService = new ServiceHost(typeof(StateBrowsingService),
                new Uri("net.pipe://localhost/StateBrowsingService"));
            stateService.AddServiceEndpoint(typeof(IStateBrowsingService), new NetNamedPipeBinding(), string.Empty);
            stateService.AddDependencyInjectionBehavior<IStateBrowsingService>(container);
            hosts.Add(stateService);

            hosts.ForEach(host => host.Open());
        }

        public void Stop()
        {
            hosts.ForEach(host => host.Close());
        }
    }
}
