using System.Collections;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Customization;
using log4net;

namespace BA.Examples.Console.Data
{
    public class HQLQueryAction : SimplePersistentAction<IList>
    {
        public HQLQueryAction(IDbContext context, RepositoryFinder repoFinder, ILog log)
            : base(context, repoFinder, log)
        {
        }
        public string Query { get; set; }

        #region Overrides of SimplePersistentAction<object>


        public override IList ActionCommand()
        {
            IList rawLookup;
            using (DbWorkUnit wu = GetSessionFor(DbWorkUnitType.Write))
            {
                var query = wu.Session.CreateQuery(Query);
                return query.List();
            }
        }

        #endregion
    }
}
