using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using BA.Examples.ScriptingHelper.ViewModels;

namespace BA.Examples.ScriptingHelper.Views
{
    /// <summary>
    /// Interaction logic for FiddlerPageDomView.xaml
    /// </summary>
    public partial class FiddlerPageFixedXmlView : UserControl
    {
        public FiddlerPageFixedXmlView()
        {
            InitializeComponent();
            Loaded += (o, e) =>
                          {
                              TreeDomDocument.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(TreeDomDocument_SelectedItemChanged);
                              TreeSessionDocument.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(TreeSessionDocument_SelectedItemChanged);
                          };
        }

        void TreeSessionDocument_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedEl = e.NewValue as XElement;
            ViewModel.SelectedElement = selectedEl;
            elementView.SetViewModel(selectedEl);
        }

        void TreeDomDocument_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedEl = e.NewValue as XElement;
            ViewModel.SelectedElement = selectedEl;
            elementView.SetViewModel(selectedEl);
        }

        //public FiddlerPageHtmlDomInformationVm ViewModel
        //{
        //    get { return DataContext as FiddlerPageHtmlDomInformationVm; }
        //}
        public FiddlerPageFixedXmlnformationVm ViewModel
        {
            get { return DataContext as FiddlerPageFixedXmlnformationVm; }
        }

    }
}
