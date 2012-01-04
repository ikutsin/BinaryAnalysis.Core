using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BinaryAnalysis.Data.Box;

namespace BA.Examples.ScriptingHelper.ViewModels
{
    public class AbstractServiceGridVm<T> : AbstractVm  where T : class 
    {
        public ObservableCollection<T> Items { get; set; }

        public void LoadItems()
        {
            try
            {
                var query = new BoxQuery<T>();
                var list = query.Take(20).ToList();
                Items = new ObservableCollection<T>(list);

                NotifyPropertyChanged("Items");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error connecting to server");
            }
        }
    }
}
