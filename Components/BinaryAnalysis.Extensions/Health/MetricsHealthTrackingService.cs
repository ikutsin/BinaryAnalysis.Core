using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Metrics;

namespace BinaryAnalysis.Extensions.Health
{
    public class MetricsHealthTrackingService : IHealthTrackingService
    {
        private readonly MetricsService metrixSvc;
        private readonly TaxonomyTree tree;

        public const string TAXON_HEALTH_ROOT = @"BinaryAnalysis.Extensions/Health";

        public MetricsHealthTrackingService(MetricsService metrixSvc, TaxonomyTree tree)
        {
            this.metrixSvc = metrixSvc;
            this.tree = tree;
            tree.GetOrCreatePath(TAXON_HEALTH_ROOT, "Root for health metrices");
        }

        private Dictionary<string, Tuple<TaxonomyNode, MetricsEntity>> resolveMetrics = new Dictionary<string, Tuple<TaxonomyNode, MetricsEntity>>();
        protected Tuple<TaxonomyNode,MetricsEntity> ResolveMetrics(string metrixName, string name)
        {
            var key = metrixName + name;
            if (!resolveMetrics.ContainsKey(key))
            {
                TaxonomyNode metricsNode =
                    tree.GetOrCreatePath(metrixName.StartsWith("/") ? metrixName : TAXON_HEALTH_ROOT + "/" + metrixName);
                resolveMetrics.Add(key,
                                   new Tuple<TaxonomyNode, MetricsEntity>(metricsNode,
                                                                      metrixSvc.GetOrCreateFor(metricsNode, name)));
            }
            return resolveMetrics[key];
        }

        public void Track(TimeSpan value, string metrixName, string name)
        {
            Track((decimal)value.TotalMilliseconds, metrixName, name);
        }

        public void Track(int value, string metrixName, string name)
        {
            Track((decimal) value, metrixName, name);
        }

        public void Track(decimal value, string metrixName, string name)
        {
            var nodeMetrics = ResolveMetrics(metrixName, name);
            if (nodeMetrics.Item2.IsChanging(value))
            {
                nodeMetrics.Item2.AddEntry(value);
                metrixSvc.UpdateMetrics(nodeMetrics.Item2);
            }
        }
    }
}
