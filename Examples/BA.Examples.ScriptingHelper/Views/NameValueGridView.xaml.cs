using System.Windows.Controls;
using BA.Examples.ScriptingHelper.Models;
using BA.Examples.ScriptingHelper.ViewModels;

namespace BA.Examples.ScriptingHelper.Views
{
    /// <summary>
    /// Interaction logic for SchedulesGridView.xaml
    /// </summary>
    public partial class NameValueGridView : UserControl
    {
        public NameValueGridView()
        {
            InitializeComponent();

            ListView.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
            {
                var item = ListView.SelectedItem as NameValueItem;
                if(item==null)return;
                ViewModel.SelectedName = item.Name;
                ViewModel.SelectedValue = item.Value;
            };
        }

        public NameValueGridVm ViewModel
        {
            get { return DataContext as NameValueGridVm; }
        }
    }
}
