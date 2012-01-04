using Autofac;
using Autofac.Integration.Wcf;
using System.ServiceModel;
using BA.Examples.ServiceProcess.Services;

namespace BA.Examples.ServiceClient.Modules
{
    public class CommonsClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CommonsServiceEventsDefaultHandler>().SingleInstance();

            builder.Register(c => new DuplexChannelFactory<ICommonsService>(
                    c.Resolve<CommonsServiceEventsDefaultHandler>(),
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
                new EndpointAddress("net.pipe://localhost/CommonsService")))
                .SingleInstance();

            builder.Register(c => c.Resolve<DuplexChannelFactory<ICommonsService>>().CreateChannel()).UseWcfSafeRelease();
            
            base.Load(builder);
        }
    }
}
