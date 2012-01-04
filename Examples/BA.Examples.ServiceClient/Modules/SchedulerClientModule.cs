using Autofac;
using Autofac.Integration.Wcf;
using System.ServiceModel;
using BA.Examples.ServiceProcess.Services;

namespace BA.Examples.ServiceClient.Modules
{
    public class SchedulerClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new ChannelFactory<ISchedulerService>(
                new NetNamedPipeBinding()
                    {
                        //CloseTimeout = new TimeSpan(0, 5, 0),
                        //OpenTimeout = new TimeSpan(0, 5, 0),
                        //ReceiveTimeout = new TimeSpan(0, 10, 0),
                        //SendTimeout = new TimeSpan(0, 10, 0),
                        //HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                        //MaxBufferSize = int.MaxValue,
                        //MaxBufferPoolSize = int.MaxValue,
                        //MaxReceivedMessageSize = int.MaxValue,
                        //TransferMode = TransferMode.Buffered,
                    },
                new EndpointAddress("net.pipe://localhost/SchedulerService")))
                .SingleInstance();

            builder.Register(c => c.Resolve<ChannelFactory<ISchedulerService>>().CreateChannel()).UseWcfSafeRelease();
            
            base.Load(builder);
        }
    }
}
