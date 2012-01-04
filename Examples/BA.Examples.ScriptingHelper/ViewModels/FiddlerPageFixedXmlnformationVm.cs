using System;
using System.Linq;
using System.Xml.Linq;
using BA.Examples.ScriptingHelper.Logic;
using BA.Examples.ScriptingHelper.ViewModels.Utils;
using BinaryAnalysis.Browsing.Extensions;

namespace BA.Examples.ScriptingHelper.ViewModels
{
    public class FiddlerPageFixedXmlnformationVm : AbstractVm
    {
        private XDocument sessionDocument;
        public XDocument SessionDocument
        {
            get { return sessionDocument; }
            set { sessionDocument = value; NotifyPropertyChanged("SessionDocument"); }
        }
        private XDocument domDocument;
        public XDocument DomDocument
        {
            get { return domDocument; }
            set { domDocument = value; NotifyPropertyChanged("DomDocument"); }
        }

        private XElement selectedElement;
        public XElement SelectedElement
        {
            get { return selectedElement; }
            set { selectedElement = value; NotifyPropertyChanged("SelectedElement"); }
        }

        public FiddlerPageFixedXmlnformationVm()
        {
            Clean();
        }
        public void Clean()
        {
            SessionDocument = new XDocument();
            DomDocument = new XDocument();
        }
        public void Bind(mshtml.HTMLDocument dom)
        {
            Clean();
            var sess = Enumerable.First<FiddlerSessionHolder>(FiddlerHelper.GetSessionsStack());
            //TODO: diff
            SessionDocument = sess.BrowsingResponse.ResponseContent.AsFixedXML();
            String domDocStr = ("" + (dom as dynamic).documentElement.OuterHtml + "");
            DomDocument = domDocStr.AsFixedXML();
        }

    }
}
