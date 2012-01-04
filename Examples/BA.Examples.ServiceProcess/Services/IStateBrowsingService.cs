using System.Collections.Generic;
using System.ServiceModel;

namespace BA.Examples.ServiceProcess.Services
{
    [ServiceContract]
    public interface IStateBrowsingService
    {
        [OperationContract]
        List<string> RequestContains(string str);

        [OperationContract]
        List<string> RequestStartsWith(string str);

        [OperationContract]
        string LoadStateResponse(string key);
        
        [OperationContract]
        string LoadFixedStateResponse(string key);
    }
}
