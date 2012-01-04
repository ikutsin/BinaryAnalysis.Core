using System;
using System.Xml;
using System.Xml.Linq;
using Autofac;
using Autofac.Integration.Wcf;
using BA.Examples.ServiceProcess;
using BA.Examples.ServiceProcess.Services;
using BinaryAnalysis.Data.Box;
using System.ServiceModel;

namespace BA.Examples.ServiceClient.Modules
{
    public class BoxQueryClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new ChannelFactory<IBoxQueryService>(
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
                new EndpointAddress("net.pipe://localhost/BoxQueryService")))
                .SingleInstance();

            builder.Register(c => c.Resolve<ChannelFactory<IBoxQueryService>>().CreateChannel()).UseWcfSafeRelease()
                .OnActivated(c =>
                                 {
                                     BoxedQueryRemoteExtensions.DefaultEvaluator = new Func<XElement, object>(
                                         x => c.Context.Resolve<IBoxQueryService>().Evaluate(x));
                                 });
            builder.RegisterGeneric(typeof(BoxQuery<>));


            //known types
            KnownTypeRegistry.RegisterKnownTypes();

            base.Load(builder);
        }
    }
}
