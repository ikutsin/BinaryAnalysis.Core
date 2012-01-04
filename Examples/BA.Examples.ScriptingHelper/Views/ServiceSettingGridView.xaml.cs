using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using BA.Examples.ScriptingHelper.Models;
using BA.Examples.ScriptingHelper.ViewModels;
using BinaryAnalysis.Scheduler;

namespace BA.Examples.ScriptingHelper.Views
{
    /// <summary>
    /// Interaction logic for SchedulesGridView.xaml
    /// </summary>
    public partial class ServiceSettingGridView : UserControl
    {
        public ServiceSettingGridView()
        {
            InitializeComponent();
        }

        public ServiceSettingGridVm ViewModel
        {
            get { return DataContext as ServiceSettingGridVm; }
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ListView.SelectedItem as ServiceSettingsEntryBoxMapModel;
            if (item != null && item.Model.IsContract)
            {
                var val = item.Model.GetValue();
                if (val is IEnumerable<ScriptAssertionMessage>)
                {
                    var msgItems = (val as IEnumerable<ScriptAssertionMessage>)
                        .Select(x => new NameValueItem {Name = x.Date.ToString(), Value = "" + x.Type + ": " + x.Message});
                    new NameValueListWindow(msgItems, item.Name).ShowDialog();
                }
                else if (val is IEnumerable<BrowsingGoalScriptSchedule>)
                {
                    var schItems = (val as IEnumerable<BrowsingGoalScriptSchedule>)
                        .Select(x => new NameValueItem {Name = x.Date.ToString(), Value = x.ScriptName});
                    new NameValueListWindow(schItems, item.Name).ShowDialog();
                }
                else
                {
                    new SimpleTextWindow(item.SimpleValue, item.Name).ShowDialog();
                }
            }
        }
    }
}
