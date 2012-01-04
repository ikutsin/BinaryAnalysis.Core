using System.Windows.Controls;
using BA.Examples.ScriptingHelper.ViewModels;

namespace BA.Examples.ScriptingHelper.Views
{
    /// <summary>
    /// Interaction logic for SchedulesGridView.xaml
    /// </summary>
    public partial class ServiceSchedulesGridView : UserControl
    {
        public ServiceSchedulesGridView()
        {
            InitializeComponent();
        }

        public ServiceSchedulesGridVm ViewModel
        {
            get { return DataContext as ServiceSchedulesGridVm; }
        }
    }
}
