using System.Windows.Controls;
using HtmlAgilityPack;

namespace BA.Examples.ScriptingHelper.Views
{
    /// <summary>
    /// Interaction logic for FiddlerPageDomView.xaml
    /// </summary>
    public partial class FiddlerPageHtmlDomElementView : UserControl
    {
        public FiddlerPageHtmlDomElementView()
        {
            InitializeComponent();
        }

        public void SetViewModel(HtmlNode elem)
        {
            DataContext = elem;
            if (elem == null) return;
            tbxValue.Text = elem.OuterHtml;
            tbxInnerText.Text = elem.InnerText;
            tbxXPath.Text = elem.XPath;
        }

        public HtmlNode ViewModel
        {
            get { return DataContext as HtmlNode; }
        }
    }
}
