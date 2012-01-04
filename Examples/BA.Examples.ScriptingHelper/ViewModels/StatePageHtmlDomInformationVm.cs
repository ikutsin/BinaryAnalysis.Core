using System;
using BA.Examples.ScriptingHelper.Models;
using BinaryAnalysis.Browsing.Extensions;

namespace BA.Examples.ScriptingHelper.ViewModels
{
    public class StatePageHtmlDomInformationVm : AbstractPageHtmlDomInformationVm
    {
        public override void Bind(mshtml.HTMLDocument dom, string rawContent)
        {
            Clean();
            SessionDocument = new HtmlNodeHierarchy(rawContent.AsHtmlDocument().DocumentNode);
            String domDocStr = ("" + (dom as dynamic).documentElement.OuterHtml + "");
            DomDocument = new HtmlNodeHierarchy(domDocStr.AsHtmlDocument().DocumentNode);
        }
    }
}
