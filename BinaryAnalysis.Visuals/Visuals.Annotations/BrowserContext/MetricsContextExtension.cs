using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Metrics;
using BinaryAnalysis.Extensions.Health;
using BinaryAnalysis.UI.BrowserContext;
using Newtonsoft.Json;

namespace BinaryAnalysis.Visuals.Annotations.BrowserContext
{
    [ComVisible(true)]
    public class MetricsContextExtension : IBrowserContextExtension
    {
        private readonly MetricsService _metricsService;
        private readonly TaxonomyTree _tree;
        private readonly NHibernateBoxTransformation<MetricsEntryBoxMap, MetricsEntryEntity> entryTransformation;


        public MetricsContextExtension(MetricsService metricsService,
            TaxonomyTree tree, NHibernateBoxTransformation<MetricsEntryBoxMap, MetricsEntryEntity> entryTransformation)
        {
            _metricsService = metricsService;
            _tree = tree;
            this.entryTransformation = entryTransformation;
        }

        #region Health
        public string getHealthItems()
        {
            var node = _tree.FindOne(MetricsHealthTrackingService.TAXON_HEALTH_ROOT);
            if (node == null) return null;

            var result = node.Children.Concat(new[] { node }).Select(
                taxonomyNode => new
                                    {
                                        Arr = GetListEntriesViewModel(taxonomyNode),
                                        taxonomyNode.Id,
                                        taxonomyNode.Name
                                    });
            return JsonConvert.SerializeObject(result);
        }
        #endregion

        #region Grid
        public string getMetricsEntries(string typename, int id, string metricsName)
        {
            var entries = GetMetricsEntries(typename, id, metricsName);
            if (entries == null) return null;
            return JsonConvert.SerializeObject(entries);
        }
        public string getEntriesListColNames()
        {
            return JsonConvert.SerializeObject(new[] { "Value", "Date" });
        }

        public string getEntriesListColModel()
        {
            return JsonConvert.SerializeObject(
                new[]
                    {
                        QjGridColumnFormat.Create("Value", typeof (object)),
                        QjGridColumnFormat.Create("RecordDate", typeof (DateTime))
                    });
        }

        public string getMetricsColNames()
        {
            return JsonConvert.SerializeObject(new[] { "Name", "Value", "Changes", "Sparkle", "LastChange" });
        }

        public string getMetricsColModel()
        {
            return JsonConvert.SerializeObject(
                new[]
                    {
                        QjGridColumnFormat.Create("Name", typeof (string)),
                        QjGridColumnFormat.Create("Value", typeof (object)),
                        QjGridColumnFormat.Create("Changes", typeof (int)),
                        QjGridColumnFormat.Create("Values", typeof (int[])),
                        QjGridColumnFormat.Create("LastChange", typeof (DateTime))
                    });
        }

        public string getListEntries(string typename, int id)
        {
            var viewmodel = GetListEntriesViewModel(new ClassifiableElementProxy(typename, id));
            return JsonConvert.SerializeObject(viewmodel);
        }
        #endregion

        private IList<MetricsEntryBoxMap> GetMetricsEntries(string typename, int id, string metricsName)
        {
            var classifiable = new ClassifiableElementProxy(typename, id);
            var metrics = _metricsService.GetFor(classifiable);
            var entries = metrics.FirstOrDefault(m => m.Name == metricsName);
            if (entries == null) return null;

            entryTransformation.Entries = entries.Entries;
            return entryTransformation.ToBox();
        }
        private IEnumerable<object> GetListEntriesViewModel(IClassifiable classifiable)
        {
            var metrics = _metricsService.GetFor(classifiable);
            return metrics.Select(
                d => new
                {
                    Name = d.Name,
                    Value = d.GetLastValueOrDefault(),
                    Changes = d.Entries.Count,
                    Values = SparkleValues(d),
                    LastChange = d.GetLastChange()
                }).Where(d => d.Changes > 0);
        }

        private decimal[] SparkleValues(MetricsEntity d)
        {
            return d.Entries.Skip(Math.Max(0, d.Entries.Count - 20))
                .Select(e => e.Value).ToArray();
        }
    }
}
