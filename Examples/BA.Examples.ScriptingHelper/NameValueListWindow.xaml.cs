using System.Collections.Generic;
using BA.Examples.ScriptingHelper.Models;

namespace BA.Examples.ScriptingHelper
{
    /// <summary>
    /// Interaction logic for NameValueListWindow.xaml
    /// </summary>
    public partial class NameValueListWindow : System.Windows.Window
    {
        public NameValueListWindow(IEnumerable<NameValueItem> items, string name = "Name value list")
        {
            InitializeComponent();
            //listView.ViewModel.Items.Clear();

            foreach (var nameValueItem in items)
            {
                listView.ViewModel.Items.Add(nameValueItem);
            }
            this.Title = name;
            Title += " (" + listView.ViewModel.Items.Count + ")";


        }
    }
}
