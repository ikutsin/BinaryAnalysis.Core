using System.Windows;
using System.Windows.Controls;
using BA.Examples.ScriptingHelper.Models;
using BA.Examples.ScriptingHelper.ViewModels;

namespace BA.Examples.ScriptingHelper.Views
{
    /// <summary>
    /// Interaction logic for TaxonomyTreeView.xaml
    /// </summary>
    public partial class TaxonomyTreeView : UserControl
    {
        public TaxonomyTreeView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(TaxonomyTreeView_Loaded);
        }

        public TaxonomyTreeVm ViewModel
        {
            get { return DataContext as TaxonomyTreeVm; }
        }

        void TaxonomyTreeView_Loaded(object sender, RoutedEventArgs e)
        {
            treeTaxonomy.SelectedItemChanged +=
                (s, ea) =>
                    {
                        var selectedItem = ea.NewValue as TaxonomyHierarchy;
                        ViewModel.LoadRelations(selectedItem.Current);
                    };
        }
    }
}
