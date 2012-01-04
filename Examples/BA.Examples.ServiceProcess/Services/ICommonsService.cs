using System.Collections.Generic;
using System.ServiceModel;
using BA.Examples.ServiceProcess.Services.Core;
using BinaryAnalysis.Data;

namespace BA.Examples.ServiceProcess.Services
{
    [ServiceContract(CallbackContract = typeof(ICommonsServiceEvents))]
    public interface ICommonsService : ISubscriptionService
    {
        [OperationContract]
        void BroadcastText(string text);

        #region Taxonomy

        [OperationContract]
        TaxonomyNodeBoxMap FindOne(string query);
        
        [OperationContract]
        TaxonomyNodeBoxMap GetRoot();

        [OperationContract]
        IEnumerable<TaxonomyNodeBoxMap> ChildrenOf(TaxonomyNodeBoxMap tax);

        [OperationContract]
        IEnumerable<string> GetRelationsByType(TaxonomyNodeBoxMap tax);

        [OperationContract]
        IEnumerable<string> GetClassified(TaxonomyNodeBoxMap tax);



        #endregion
    }
}
