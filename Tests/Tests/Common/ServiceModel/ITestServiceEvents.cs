using System.ServiceModel;

namespace BinaryAnalysis.Tests.Common.ServiceModel
{
    public interface ITestServiceEvents
    {
        [OperationContract(IsOneWay = true)]
        void OnEvent1(string text);

        [OperationContract(IsOneWay = true)]
        void OnEvent2(int number);

        [OperationContract(IsOneWay = true)]
        void OnEvent3(int number, string text);
    }
}
