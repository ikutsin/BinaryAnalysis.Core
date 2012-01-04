using System.Collections.Generic;
using System.Windows.Media;
using HtmlAgilityPack;

namespace BA.Examples.ScriptingHelper.Models
{
    public class HtmlNodeHierarchy : ObservableHierarchy<HtmlNode>
    {
        private Brush highlight;
        public Brush Highlight { get { return highlight; } set { highlight = value; NotifyPropertyChanged("Highlight"); } }

        public HtmlNodeHierarchy(HtmlNode current) : base(current)
        {
            Highlight = Brushes.Transparent;
        }

        protected override IEnumerable<HtmlNode> Expand()
        {
            return Current.ChildNodes;
        }

        protected override ObservableHierarchy<HtmlNode> Create(HtmlNode item)
        {
            return new HtmlNodeHierarchy(item);
        }
    }
}
