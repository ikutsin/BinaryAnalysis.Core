using System.ServiceModel;
using System.Xml;
using Autofac;
using Autofac.Integration.Wcf;
using BA.Examples.ServiceProcess.Services;

namespace BA.Examples.ServiceClient.Modules
{
    class StateBrowserClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new ChannelFactory<IStateBrowsingService>(
                                      new NetNamedPipeBinding
                                          {
                                              //CloseTimeout = new TimeSpan(0, 5, 0),
                                              //OpenTimeout = new TimeSpan(0, 5, 0),
                                              //ReceiveTimeout = new TimeSpan(0, 10, 0),
                                              //SendTimeout = new TimeSpan(0, 10, 0),
                                              //HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                                              //MaxBufferSize = int.MaxValue,
                                              //MaxBufferPoolSize = int.MaxValue,
                                              MaxReceivedMessageSize = int.MaxValue,
                                              //TransferMode = TransferMode.Buffered,
                                              ReaderQuotas = new XmlDictionaryReaderQuotas
                                                                 {
                                                                     MaxArrayLength = int.MaxValue,
                                                                     MaxBytesPerRead = int.MaxValue,
                                                                     MaxDepth = int.MaxValue,
                                                                     MaxNameTableCharCount = int.MaxValue,
                                                                     MaxStringContentLength = int.MaxValue,
                                                                 }
                                          },
                                      new EndpointAddress("net.pipe://localhost/StateBrowsingService")))
                .SingleInstance();

            builder.Register(c => c.Resolve<ChannelFactory<IStateBrowsingService>>().CreateChannel()).UseWcfSafeRelease();
            base.Load(builder);
        }
    }
}
