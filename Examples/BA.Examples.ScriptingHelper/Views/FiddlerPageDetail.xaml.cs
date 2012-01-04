using System.Windows.Controls;
using BA.Examples.ScriptingHelper.ViewModels;

namespace BA.Examples.ScriptingHelper.Views
{
    /// <summary>
    /// Interaction logic for FiddlerPageDetail.xaml
    /// </summary>
    public partial class FiddlerPageDetail : UserControl
    {
        public FiddlerPageDetail()
        {
            InitializeComponent();
        }

        public FiddlerPageDetailVm ViewModel
        {
            get { return DataContext as FiddlerPageDetailVm; }
            set { DataContext = value; }
        }
    }
}
