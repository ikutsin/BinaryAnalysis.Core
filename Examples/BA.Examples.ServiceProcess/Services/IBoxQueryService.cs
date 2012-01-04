using System.ServiceModel;
using System.Xml.Linq;

namespace BA.Examples.ServiceProcess.Services
{
    //http://davybrion.com/blog/2008/07/the-known-type-provider/ or
    //http://www.xavierdecoster.com/post/2010/05/07/Automate-WCF-KnownTypes-in-a-3-Tier-Silverlight-application.aspx
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypeRegistry))]
    public interface IBoxQueryService
    {
        [OperationContract]
        object Evaluate(XElement elem);
    }
}
