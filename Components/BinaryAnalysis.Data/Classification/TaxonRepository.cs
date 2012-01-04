using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Settings;
using log4net;

namespace BinaryAnalysis.Data.Classification
{
    public class TaxonRepository : ClassifiableRepository<TaxonEntity>
    {
        private RelationRepository relRepo;

        public TaxonRepository(IDbContext context, ILog log, RelationRepository relRepo,
            RelationService relSvc)
            : base(context, log, relSvc)
        {
            this.relRepo = relRepo;
        }

        public override void Delete(TaxonEntity entity)
        {
            var taxonTypes = relRepo.GetRelationsByType(entity);
            taxonTypes.ToList().ForEach(x => relRepo.Delete(x));

            base.Delete(entity);
        }
    }
}
