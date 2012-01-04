using BA.Examples.ServiceProcess.Services.Core;

namespace BinaryAnalysis.Tests.Common.ServiceModel
{
    class TestServiceEventsPublisher : PublishService<ITestServiceEvents>, ITestServiceEvents
    {
        public void OnEvent1(string text)
        {
            FireEvent(text);
        }

        public void OnEvent2(int number)
        {
            FireEvent(number);
        }

        public void OnEvent3(int number, string text)
        {
            FireEvent(number, text);
        }
    }
}
