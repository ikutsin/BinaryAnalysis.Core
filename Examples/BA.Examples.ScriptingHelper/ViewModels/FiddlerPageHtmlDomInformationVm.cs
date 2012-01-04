using System;
using System.Linq;
using BA.Examples.ScriptingHelper.Logic;
using BA.Examples.ScriptingHelper.Models;
using BA.Examples.ScriptingHelper.ViewModels.Utils;
using BinaryAnalysis.Browsing.Extensions;

namespace BA.Examples.ScriptingHelper.ViewModels
{
    public class FiddlerPageHtmlDomInformationVm : AbstractPageHtmlDomInformationVm
    {
        public override void Bind(mshtml.HTMLDocument dom, string content)
        {
            Clean();
            var sess = Enumerable.First<FiddlerSessionHolder>(FiddlerHelper.GetSessionsStack());
            
            //TODO: diff
            SessionDocument = new HtmlNodeHierarchy(sess.BrowsingResponse.ResponseContent.AsHtmlDocument().DocumentNode);
            String domDocStr = ("" + (dom as dynamic).documentElement.OuterHtml + "");
            DomDocument = new HtmlNodeHierarchy(domDocStr.AsHtmlDocument().DocumentNode);
        }
    }
}
