using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using BA.Examples.ScriptingHelper.Models;
using BA.Examples.ScriptingHelper.ViewModels.Commands;
using HtmlAgilityPack;
using System.Windows;

namespace BA.Examples.ScriptingHelper.ViewModels
{
    public abstract class AbstractPageHtmlDomInformationVm : AbstractVm
    {
        public const string CUSTOM_ATTR_PREFIX = "ba_";
        public const string CUSTOM_ATTR_HL = CUSTOM_ATTR_PREFIX+"hl";

        private HtmlNodeHierarchy sessionDocument;
        public HtmlNodeHierarchy SessionDocument
        {
            get { return sessionDocument; }
            set { sessionDocument = value; NotifyPropertyChanged("SessionDocument"); }
        }
        private HtmlNodeHierarchy domDocument;
        public HtmlNodeHierarchy DomDocument
        {
            get { return domDocument; }
            set { domDocument = value; NotifyPropertyChanged("DomDocument"); }
        }

        private HtmlNode selectedElement;
        public HtmlNode SelectedElement
        {
            get { return selectedElement; }
            set { selectedElement = value; NotifyPropertyChanged("SelectedElement"); }
        }

        public ICommand HightlightHtmlElements
        {
            get
            {
                return new RelayCommand((xpatho) =>
                {
                    Highlight(xpatho.ToString());
                }, (xpath) => !String.IsNullOrWhiteSpace(xpath.ToString()) );
            }
        }
        public ICommand HightlightSingleElement
        {
            get
            {
                return new RelayCommand((xpatho) =>
                {
                    HighlightSingle(xpatho.ToString());
                }, (xpath) => !String.IsNullOrWhiteSpace(xpath.ToString()));
            }
        }

        private void HighlightSingle(string xpath)
        {
            try
            {
                var hnode = SessionDocument.Current.SelectSingleNode(xpath);
                var node = SessionDocument.Descendants().Where(x => x.Current == hnode)
                    .FirstOrDefault() as HtmlNodeHierarchy;
                if(node!=null) node.Highlight = Brushes.Red;


                hnode = DomDocument.Current.SelectSingleNode(xpath);
                node = DomDocument.Descendants().Where(x => x.Current == hnode)
                    .FirstOrDefault() as HtmlNodeHierarchy;
                if (node != null) node.Highlight = Brushes.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }

            NotifyPropertyChanged("SessionDocument");
            NotifyPropertyChanged("DomDocument");
        }

        private void Highlight(string xpath)
        {
            try
            {
                bool nodefound = false;
                var sessNodes = SessionDocument.Current.SelectNodes(xpath);
                if (sessNodes != null)
                {
                    foreach (HtmlNodeHierarchy node in SessionDocument.Descendants())
                    {
                        node.Highlight = sessNodes.Contains(node.Current) ? Brushes.Yellow : Brushes.Transparent;
                    }
                    nodefound = true;
                }

                var domNodes = DomDocument.Current.SelectNodes(xpath);
                if (domNodes != null)
                {
                    foreach (HtmlNodeHierarchy node in DomDocument.Descendants())
                    {
                        node.Highlight = domNodes.Contains(node.Current) ? Brushes.Yellow : Brushes.Transparent;
                    }
                    nodefound = true;
                }
                if(!nodefound)MessageBox.Show("null nodes");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }

            NotifyPropertyChanged("SessionDocument");
            NotifyPropertyChanged("DomDocument");
        }


        public AbstractPageHtmlDomInformationVm()
        {
            Clean();
        }
        public void Clean()
        {
            SessionDocument = new HtmlNodeHierarchy(null);
            DomDocument = new HtmlNodeHierarchy(null);
        }

        public abstract void Bind(mshtml.HTMLDocument dom, string rawContent);
    }
}
