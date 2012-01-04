using System;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core.Impl;

namespace BinaryAnalysis.Extensions.HttpProxy.Data
{
    public class HttpProxyEntityMap : EntityClassMap<HttpProxyEntity>
    {
        public HttpProxyEntityMap()
        {
            Map(x => x.Country);
            Map(x => x.PingTime);
            Map(x => x.Comment);
            Map(x => x.IP).Not.Nullable();

            Map(x => x.FirstUpdate);
            Map(x => x.LastLive);
            Map(x => x.LastUpdate);

            Map(x => x.ResolverName);
            Map(x => x.ProviderName);
            Map(x => x.Type);
        }
    }
    [BoxTo(typeof(HttpProxyBoxMap))]
    public class HttpProxyEntity : Entity, IClassifiable
    {
        public const string OBJECT_NAME = "Proxy_Http";
        public virtual string ObjectName
        {
            get { return OBJECT_NAME; }
        }

        public virtual DateTime FirstUpdate { get; set; }
        public virtual DateTime LastLive { get; set; }
        public virtual DateTime LastUpdate { get; set; }

        public virtual string Country { get; set; }
        public virtual string ResolverName { get; set; }
        public virtual string ProviderName { get; set; }
        public virtual HttpProxyType Type { get; set; }

        public virtual TimeSpan PingTime { get; set; }
        public virtual string Comment { get; set; }
        public virtual string IP { get; set; }

        public override string ToString()
        {
            return Comment + " " + IP;
        }
    }
}
