using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BA.Examples.ScriptingHelper.Models;
using BA.Examples.ScriptingHelper.ViewModels;

namespace BA.Examples.ScriptingHelper.Views
{
    /// <summary>
    /// Interaction logic for FiddlerPageDomView.xaml
    /// </summary>
    public partial class FiddlerPageHtmlDomView : UserControl
    {
        public FiddlerPageHtmlDomView()
        {
            InitializeComponent();
            Loaded += (o, e) =>
                          {
                              TreeDomDocument.SelectedItemChanged += d_SelectedItemChanged;
                              TreeSessionDocument.SelectedItemChanged += d_SelectedItemChanged;
                          };
        }

        void d_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedEl = e.NewValue as HtmlNodeHierarchy;// XElement;
            if (selectedEl == null)
            {
                ViewModel.SelectedElement = null;
                elementView.SetViewModel(null);
            }
            else
            {
                ViewModel.SelectedElement = selectedEl.Current;
                elementView.SetViewModel(selectedEl.Current);
            }
        }

        //public FiddlerPageHtmlDomInformationVm ViewModel
        //{
        //    get { return DataContext as FiddlerPageHtmlDomInformationVm; }
        //}
        public AbstractPageHtmlDomInformationVm ViewModel
        {
            get { return DataContext as AbstractPageHtmlDomInformationVm; }
        }

        void WalkTree(ItemsControl item, Action<TreeViewItem> action)
        {
            foreach (object obj in item.Items)
            {
                ItemsControl childControl = item.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl != null)
                {
                    WalkTree(childControl, action);
                }
                TreeViewItem treeItem = childControl as TreeViewItem;
                if (treeItem != null)
                {
                    action(treeItem);
                }
            }        
        }

        private void Button_SessionDocumentExpandClick(object sender, RoutedEventArgs e)
        {
            WalkTree(TreeSessionDocument, (x) => x.IsExpanded = true);
        }

        private void Button_SessionDocumentCollapseClick(object sender, RoutedEventArgs e)
        {
            WalkTree(TreeSessionDocument, (x) => x.IsExpanded = false);
        }

        private void Button_DomDocumentExpandClick(object sender, RoutedEventArgs e)
        {
            WalkTree(TreeDomDocument, (x) => x.IsExpanded = true);
        }

        private void Button_DomDocumentCollapseClick(object sender, RoutedEventArgs e)
        {
            WalkTree(TreeDomDocument, (x) => x.IsExpanded = false);
        }

        void SelectAndExpand(TreeViewItem x)
        {
            x.IsSelected = true;
            var y = x;
            do
            {
                y.IsExpanded = true;
                y = GetSelectedTreeViewItemParent(y) as TreeViewItem;
            } while (y != null);
        }
        public ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as ItemsControl;
        }

        private void Button_ExpandElement(object sender, RoutedEventArgs e)
        {
            try
            {
                var xpath = tbxXPathInput.Text;
                var node = ViewModel.SessionDocument.Current.SelectSingleNode(xpath);
                if (node != null)
                {
                    WalkTree(TreeSessionDocument, (x) => 
                    { if ((x.DataContext as HtmlNodeHierarchy).Current == node)SelectAndExpand(x); });
                }
                node = ViewModel.DomDocument.Current.SelectSingleNode(xpath);
                if (node != null)
                {
                    WalkTree(TreeDomDocument, (x) => 
                    { if ((x.DataContext as HtmlNodeHierarchy).Current == node) SelectAndExpand(x);});
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
        }
    }
}
