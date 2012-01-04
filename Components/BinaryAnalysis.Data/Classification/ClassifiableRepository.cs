using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Core.Impl;
using log4net;

namespace BinaryAnalysis.Data.Classification
{
    public class ClassifiableRepository<T> : Repository<T> where T:Entity, IClassifiable
    {
        private readonly RelationService _relationService;

        public ClassifiableRepository(IDbContext context, ILog log, 
            RelationService relationService):base(context,log)
        {
            _relationService = relationService;
        }

        public override void Delete(T entity)
        {
            _relationService.RemoveRelations(entity);
            base.Delete(entity);
        }

    }
}
