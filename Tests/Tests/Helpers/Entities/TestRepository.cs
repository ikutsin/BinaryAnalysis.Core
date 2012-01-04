using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Versioning;
using log4net;

namespace BinaryAnalysis.Tests.Helpers.Entities
{
    public class TestRepository : TrackableRepository<TestEntity>
    {
        public TestRepository(
            IDbContext context, 
            ILog log,
            RelationService relRepo, 
            TrackingRepository trackRepo) : base(context, log, relRepo, trackRepo)
        {
        }

        public virtual void Test()
        {
            log.Info("Test called");
        }
    }
}
