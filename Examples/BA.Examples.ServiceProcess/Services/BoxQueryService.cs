using System.ServiceModel;
using System.Xml.Linq;
using BinaryAnalysis.Data.Box;
using log4net;

namespace BA.Examples.ServiceProcess.Services
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerCall,
        IncludeExceptionDetailInFaults=true
         )]
    public class BoxQueryService : IBoxQueryService
    {
        ILog log;
        public BoxQueryService(ILog log)
        {
            this.log = log;
        }
        public object Evaluate(XElement elem)
        {
            var result = BoxedQueryRemoteExtensions.DefaultEvaluator.Invoke(elem);
            log.Debug("Evaluate invoke as "+result.GetType());
            return result;
        }
    }
}
