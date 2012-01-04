using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BA.Examples.ScriptingHelper.Logic;
using BA.Examples.ScriptingHelper.Models;
using BA.Examples.ScriptingHelper.ViewModels.Utils;
using BinaryAnalysis.Browsing.Extensions;
using HtmlAgilityPack;
using mshtml;

namespace BA.Examples.ScriptingHelper.ViewModels
{
    public class FiddlerPageDetailVm : AbstractVm
    {
        private HtmlDocument html;

        public ObservableCollection<CodeGeneration> GeneratedCode { get; set; }

        public FiddlerPageDetailVm()
        {
            GeneratedCode = new ObservableCollection<CodeGeneration>();
        }

        public void Bind(HTMLDocument dom)
        {
            var sess = Enumerable.First<FiddlerSessionHolder>(FiddlerHelper.GetSessionsStack());
            html = sess.BrowsingResponse.ResponseContent.AsHtmlDocument();

            GeneratedCode.Clear();
            GeneratedCode.Add(GetRequestCode(sess, html));
            foreach (var codeGeneration in GetFormsCode(sess, html))
            {
                GeneratedCode.Add(codeGeneration);
            }
            foreach (var codeGeneration in GetAllSelectCode(sess, html))
            {
                GeneratedCode.Add(codeGeneration);
            }
            GeneratedCode.Add(GetAllInputCode(sess, html));
            NotifyPropertyChanged("GeneratedCode");
        }

        private List<CodeGeneration> GetAllSelectCode(FiddlerSessionHolder sess, HtmlDocument htmlDocument)
        {
            var result = new List<CodeGeneration>();
            foreach(var selectNode in htmlDocument.DocumentNode.SelectNodes(".//select"))
            {
                var selectNodeName = selectNode.Attributes.Contains("name") ? selectNode.Attributes["name"].Value : "!!!";
                var select = new CodeGeneration("Select");
                select.Header = "Options for "+selectNodeName;
                var paramz = new StringBuilder();

                var options = selectNode.SelectNodes(".//option");
                foreach (var optioNode in options)
                {
                    paramz.AppendLine(String.Format("\t{{ \"{0}\",\"{1}\" }}, //{2} {3}", selectNodeName,
                        optioNode.Attributes.Contains("value") ? optioNode.Attributes["value"].Value : "",
                        optioNode.NextSibling.InnerText,
                        optioNode.Attributes.Contains("selected") ? "<-default" : ""));
                }
                select.Text = paramz.ToString();
                result.Add(select);
            }
            return result;
        }

        #region Helpers
        private int index;
        protected string GetIndex()
        {
            return (index++).ToString();
        }
        protected string StrMakeNavigateGet(string url, IEnumerable<Tuple<string, string, string>> p)
        {
            var index = GetIndex();
            var paramz = new StringBuilder();
            paramz.AppendLine(String.Format("var getUrl{1} = x.Exec<IBrowsingSession>(\"GetBrowsingSession\").MakeUrl(\"{0}\", new NameValueCollection", url, index));
            paramz.AppendLine("{");
            foreach (var kvp in p)
            {
                paramz.AppendLine(String.Format("\t{{ \"{0}\",\"{1}\" }}, //{2}", kvp.Item1, kvp.Item2, kvp.Item3));
            }
            paramz.AppendLine("});");
            paramz.AppendLine(String.Format("var response{1} = x.Exec<IBrowsingSession>(\"GetBrowsingSession\").NavigateGet(getUrl{1});", url, index));

            return paramz.ToString();
        }
        protected string StrMakeNavigatePost(string url, IEnumerable<Tuple<string, string, string>> p)
        {
            var index = GetIndex();
            var paramz = new StringBuilder();
            paramz.AppendLine(String.Format("var postParams{1} = new NameValueCollection", url, index));
            paramz.AppendLine("{");
            foreach (var kvp in p)
            {
                paramz.AppendLine(String.Format("\t{{ \"{0}\",\"{1}\" }}, //{2}", kvp.Item1, kvp.Item2, kvp.Item3));
            }
            paramz.AppendLine("});");
            paramz.AppendLine(String.Format("var response{1} = x.Exec<IBrowsingSession>(\"GetBrowsingSession\").NavigatePost(new Uri(\"{0}\"), postParams{1});", url, index));

            return paramz.ToString();
        }

        protected string StrNavigateGet(string url)
        {
            return String.Format("var response{1} = x.Exec<IBrowsingSession>(\"GetBrowsingSession\").NavigateGet(new Uri(@\"{0}\"));", url, GetIndex());
        }
        #endregion

        #region CodeGen
        private List<CodeGeneration> GetFormsCode(FiddlerSessionHolder sess, HtmlDocument htmlDocument)
        {
            var result = new List<CodeGeneration>();
            HtmlNodeCollection nodes = html.DocumentNode.SelectNodes(".//form");
            if (nodes == null) return result;

            foreach (var formNode in nodes)
            {
                var method = "GET";
                var methodAttr = formNode.Attributes.FirstOrDefault(x => x.Name.ToLower() == "method");
                if (methodAttr != null) method = methodAttr.Value.ToUpper();

                var action = sess.FiddlerSession.fullUrl;
                var actionAttr = formNode.Attributes.FirstOrDefault(x => x.Name.ToLower() == "action");
                if (actionAttr != null) action = sess.BrowsingResponse.FixPathToAbsolute(actionAttr.Value);

                var inputNodes = formNode.SelectNodes(".//input");
                if(inputNodes==null) continue;

                var p = inputNodes
                    .Where(k => k.Attributes.FirstOrDefault(n => n.Name.ToLower() == "name") != null)
                    .Select(k =>
                                {
                                    var nameAttr = k.Attributes.FirstOrDefault(n => n.Name.ToLower() == "name");
                                    var valueAttr = k.Attributes.FirstOrDefault(n => n.Name.ToLower() == "value");
                                    return new Tuple<string, string, string>(
                                        nameAttr == null ? null : nameAttr.Value,
                                        valueAttr == null ? "" : valueAttr.Value,
                                        k.XPath);
                                }).Where(k => k.Item1 != null);

                if (method == "GET")
                {
                    result.Add(new CodeGeneration("Form: " + formNode.XPath)
                                   {
                                       Text = StrMakeNavigateGet(action, p),
                                       Header = action + " (" + method + ")"
                                   });
                }
                else
                {
                    result.Add(new CodeGeneration("Form: " + formNode.XPath)
                    {
                        Text = StrMakeNavigatePost(action, p),
                        Header = action + " (" + method + ")"
                    });                    
                }
            }
            return result;
        }
        private CodeGeneration GetRequestCode(FiddlerSessionHolder sess, HtmlDocument htmlDocument)
        {
            var requestCode = new CodeGeneration("Request");
            var requestType = sess.FiddlerSession.oRequest.headers.HTTPMethod;
            requestCode.Header = sess.FiddlerSession.url + " (" + requestType + ")";

            switch (requestType)
            {
                case "GET":
                    requestCode.Text = StrNavigateGet(sess.FiddlerSession.fullUrl);
                    break;
                case "POST":
                    requestCode.Text = "todo " + GetIndex();
                    break;
                default:
                    requestCode.Text = "Invalid requestType " + requestType;
                    break;
            }
            return requestCode;
        }
        protected CodeGeneration GetAllInputCode(FiddlerSessionHolder sess, HtmlDocument htmlDocument)
        {
            var allInput = new CodeGeneration("Request");
            allInput.Header = "Code";

            var inputNodes = htmlDocument.DocumentNode.SelectNodes(".//input");
            if (inputNodes != null)
            {
                var p = inputNodes
                    .Where(k => k.Attributes.FirstOrDefault(n => n.Name.ToLower() == "name") != null)
                    .Select(k =>
                    {
                        var nameAttr = k.Attributes.FirstOrDefault(n => n.Name.ToLower() == "name");
                        var valueAttr = k.Attributes.FirstOrDefault(n => n.Name.ToLower() == "value");
                        var typeAttr = k.Attributes.FirstOrDefault(n => n.Name.ToLower() == "type");
                        return new Tuple<string, string, string>(
                            nameAttr == null ? null : nameAttr.Value,
                            valueAttr == null ? "" : valueAttr.Value,
                            typeAttr == null ? "default" : typeAttr.Value
                            );
                    }).Where(k => k.Item1 != null);

                var paramz = new StringBuilder();
                paramz.AppendLine(String.Format("var allParams = new NameValueCollection"));
                paramz.AppendLine("{");
                foreach (var kvp in p)
                {
                    paramz.AppendLine(String.Format("\t{{ \"{0}\",\"{1}\" }}, //Type: {2}", kvp.Item1, kvp.Item2, kvp.Item3));
                }
                paramz.AppendLine("});");
                allInput.Text = paramz.ToString();
            }
            return allInput;

        }
        #endregion
    }
}
