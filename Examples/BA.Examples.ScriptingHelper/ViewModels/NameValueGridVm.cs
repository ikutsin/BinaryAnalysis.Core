using System.Collections.ObjectModel;
using BA.Examples.ScriptingHelper.Models;

namespace BA.Examples.ScriptingHelper.ViewModels
{
    public class NameValueGridVm : AbstractVm
    {
        public ObservableCollection<NameValueItem> Items { get; set; }
        public NameValueGridVm()
        {
            Items = new ObservableCollection<NameValueItem>();
        }

        private string selectedName;
        public string SelectedName { get { return selectedName; } set { selectedName = value; NotifyPropertyChanged("SelectedName"); } }

        private string selectedValue;
        public string SelectedValue { get { return selectedValue; } set { selectedValue = value; NotifyPropertyChanged("SelectedValue"); } }


    }
}
