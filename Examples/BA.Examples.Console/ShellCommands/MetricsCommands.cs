using System.Linq;
using Autofac;
using BinaryAnalysis.Data;
using BinaryAnalysis.Extensions.Health;
using BinaryAnalysis.Terminal.Commanding;

namespace BA.Examples.Console.ShellCommands
{
    public class MetricsCommands : ShellCommandSet
    {
        public void Health()
        {
            var tree = Context.Resolve<TaxonomyTree>();
            var metricsSvc = Context.Resolve<MetricsService>();
            var healthNode = tree.GetOrCreatePath(MetricsHealthTrackingService.TAXON_HEALTH_ROOT, "General health metrices");

            foreach (var node in healthNode.Children)
            {
                Writer.WriteLine(node.Path+": "+node.Description);
                foreach (var m in metricsSvc.GetFor(node))
                {
                    var me = m.Entries.LastOrDefault();
                    if (me != null)
                    {
                        Writer.WriteLine("\t{0} = {2} ({1})", m.Name, me.RecordDate, me.Value);
                    }
                }
            }
        }
    }
}
