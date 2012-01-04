using System;
using NUnit.Framework;
using System.ServiceModel;
using System.Threading;

namespace BinaryAnalysis.Tests.Common.ServiceModel
{
    [TestFixture]
    public class WcfTests
    {
        [Test]
        public void TestServiceEventModel()
        {
            var testHost = new ServiceHost(typeof(TestService), new Uri("net.pipe://localhost/TestService"));
            testHost.AddServiceEndpoint(typeof(ITestService), new NetNamedPipeBinding(), string.Empty);
            testHost.Open();

            var callback = new TestServiceEventsHandler(); //new object();// DynamicObject();


            var client2 = DuplexChannelFactory<ITestService>.CreateChannel(callback,
                new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/TestService"));

            client2.Subscribe("OnEvent1");
            client2.Subscribe("OnEvent2");
            client2.Subscribe("OnEvent3");

            var client1 = DuplexChannelFactory<ITestService>.CreateChannel(callback,
                new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/TestService"));

            client1.Subscribe("OnEvent1");
            client1.Subscribe("OnEvent2");
            client1.Subscribe("OnEvent3");


            TestServiceEventsPublisher thisPublisher = new TestServiceEventsPublisher();
            thisPublisher.OnEvent2(52);

            client1.DoWork("clien1");
            client2.DoWork("client2");

            Thread.Sleep(1000);

            

            testHost.Close();
        }
    }
}
