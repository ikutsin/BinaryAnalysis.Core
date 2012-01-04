using System;
using System.Collections.Generic;
using System.Linq;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Core.Impl;
using log4net;

namespace BinaryAnalysis.Extensions.HttpProxy.Data
{
    public class HttpProxyRepository : ClassifiableRepository<HttpProxyEntity>
    {
        public HttpProxyRepository(IDbContext context, ILog log, RelationService relRepo)
            : base(context, log, relRepo)
        {
        }

        public IList<HttpProxyEntity> GetNewForTesting()
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                return AsQueryable(wu.Session)
                    .Where(e=>e.FirstUpdate==DateTime.MinValue)
                    .ToList();
            }
        }
        public IList<HttpProxyEntity> GetInvalid()
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                return AsQueryable(wu.Session)
                    .Where(e => e.LastLive < e.LastUpdate)
                    .ToList();
            }
        }
        public IList<HttpProxyEntity> GetWorking()
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                return AsQueryable(wu.Session)
                    .Where(e => e.LastLive >= e.LastUpdate)
                    .OrderBy(e=>e.PingTime)
                    .ToList();
            }
        }

        public HttpProxyEntity GetByUri(Uri proxyAddress)
        {
            return FindOne(new Dictionary<string, object>()
            {
                { "IP", proxyAddress.ToString() }
            });
        }

        public int AddIfNew(IList<Uri> proxyAddresses, string provider)
        {
            //TODO: optimize
            return proxyAddresses.Where(p=>AddIfNew(p, provider)).Count();
        }

        public bool AddIfNew(Uri proxyAddress, string provider)
        {
            if (GetByUri(proxyAddress) != null) return false;

            var proxy = new HttpProxyEntity()
            {
                IP = proxyAddress.ToString(),
                ProviderName = provider
            };
            Save(proxy);
            return true;
        }
    }
}
