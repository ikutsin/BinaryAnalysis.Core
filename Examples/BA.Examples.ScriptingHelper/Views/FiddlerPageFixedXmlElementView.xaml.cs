using System.Windows.Controls;
using System.Xml.Linq;
using BinaryAnalysis.Browsing.Extensions;

namespace BA.Examples.ScriptingHelper.Views
{
    /// <summary>
    /// Interaction logic for FiddlerPageDomView.xaml
    /// </summary>
    public partial class FiddlerPageFixedXmlElementView : UserControl
    {
        public FiddlerPageFixedXmlElementView()
        {
            InitializeComponent();
        }

        public void SetViewModel(XElement elem)
        {
            DataContext = elem;
            if (elem == null) return;
            tbxValue.Text = elem.ValueText();
            tbxInnerText.Text = elem.InnerText();
        }

        public XElement ViewModel
        {
            get { return DataContext as XElement; }
        }
    }
}
