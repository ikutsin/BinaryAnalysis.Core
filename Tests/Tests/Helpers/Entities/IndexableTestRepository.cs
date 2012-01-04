using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Index;
using log4net;

namespace BinaryAnalysis.Tests.Helpers.Entities
{
    public class IndexableTestRepository : IndexableRepository<IndexableTestEntity>
    {
        public IndexableTestRepository(IDbContext context, ILog log)
            : base(context, log) 
        {

        }

        public virtual void Test()
        {
            log.Info("Test called");
        }
    }
}
