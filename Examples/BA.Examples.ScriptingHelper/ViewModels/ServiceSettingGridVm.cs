using System.Collections.ObjectModel;
using System.Linq;
using BA.Examples.ScriptingHelper.Models;
using BinaryAnalysis.Data.Settings;

namespace BA.Examples.ScriptingHelper.ViewModels
{
    public class ServiceSettingGridVm : AbstractVm
    {
        public ObservableCollection<ServiceSettingsEntryBoxMapModel> Items { get; set; }
        public SettingsBoxMap Item { get; set; }

        public void SetItem(SettingsBoxMap setting)
        {
            Item = setting;
            Items = new ObservableCollection<ServiceSettingsEntryBoxMapModel>(setting.Entries
                .Select(x=>new ServiceSettingsEntryBoxMapModel(x)));
            NotifyPropertyChanged("Item");
            NotifyPropertyChanged("Items");
        }
    }
}
