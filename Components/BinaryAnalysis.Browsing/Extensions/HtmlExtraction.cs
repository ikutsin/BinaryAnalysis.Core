using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BinaryAnalysis.Browsing.Windowless;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace BinaryAnalysis.Browsing.Extensions
{
    public static class HtmlExtraction
    {
        public static IEnumerable<HtmlNode> ExtractInputFields(this IBrowsingResponse response)
        {
            var doc = response.AsHtmlDocument();
            return ExtractInputFields(doc);            
        }

        public static IEnumerable<string> ExtractHyperlinks(this IBrowsingResponse response)
        {
            var doc = response.AsHtmlDocument();
            var ret = ExtractAnchorLinks(doc);
            return response.FixPathToAbsolute(ret);
        }

        /// <summary>
        /// Pretti buggy approach
        /// TODO: make a better approach
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string ContentWithFixedToAbsoluteLinks(this IBrowsingResponse response)
        {
            var linksR = ExtractAlllinks(response).ToArray();
            var linksA = FixPathToAbsolute(response, linksR).ToArray();
            var result = response.ResponseContent + "";

            for (int i = 0; i < linksR.Length; i++)
            {
                var linkR = "\"" + linksR[i] + "\"";
                var linkA = "\"" + linksA[i] + "\"";
                result = result.Replace(linkR, linkA);
            }
            return result;
        }

        /// <summary>
        /// Add domain to partial links
        /// </summary>
        /// <param name="response"></param>
        /// <param name="linkList"></param>
        /// <returns></returns>
        public static IEnumerable<string> FixPathToAbsolute(this IBrowsingResponse response, IEnumerable<string> linkList)
        {
            return linkList.Select(x => response.FixPathToAbsolute(x));
        }
        public static string FixPathToAbsolute(this IBrowsingResponse response, string link)
        {
            Uri uri = null;
            try
            {
                if (Uri.TryCreate(link, UriKind.Absolute, out uri)) return uri.ToString();
            }
            catch (Exception)
            {
            }

            var ret = "";
            
            //is relative
            if (link.StartsWith("/"))
            {
                ret = string.Format("{0}://{1}{2}", response.ResponseUrl.Scheme,
                                     response.ResponseUrl.Host, link);
            }
            else
            {
                var url = response.ResponseUrl.ToString();
                url = url.Substring(0, url.LastIndexOf("/"));
                ret = string.Format("{0}/{1}", url, link);
            }
            if (ret.IndexOf('#') > 0)
            {
                ret = ret.Substring(0, ret.IndexOf('#'));
            }
            return ret;
        }

        public static IEnumerable<string> DistinctHashAncors(this IEnumerable<string> linkList)
        {
            return linkList.Select(s=>
            {
                var hashIndex = s.IndexOf('#');
                return hashIndex <= 0 ? s : s.Substring(0, hashIndex);
            }).Distinct();
        }


        public static IEnumerable<string> ExtractAlllinks(this IBrowsingResponse response)
        {
            var doc = response.AsHtmlDocument();
            var ret = doc.ExtractAnchorLinks().ToList();
            ret.AddRange(doc.ExtractLinksNonAnchorLinks());
            return response.FixPathToAbsolute(ret);
        }

        public static IEnumerable<HtmlNode> ExtractInputFields(this HtmlDocument doc)
        {
            return doc.DocumentNode.SelectNodes("//input").ToList();
        }
        public static IEnumerable<string> ExtractLinksNonAnchorLinks(this HtmlDocument doc)
        {
            var ret = new List<string>();
            var linkList = doc.DocumentNode.SelectNodes("/html/head/link[@href]");
            if (linkList!=null)ret.AddRange(linkList.Select(link => link.Attributes["href"].Value));
            var scriptList = doc.DocumentNode.SelectNodes("/html/head/script[@src]");
            if (scriptList != null) ret.AddRange(scriptList.Select(link => link.Attributes["src"].Value));
            var frameList = doc.DocumentNode.SelectNodes("//iframe[@src]");
            if (frameList != null) ret.AddRange(frameList.Select(link => link.Attributes["src"].Value));
            var imageList = doc.DocumentNode.SelectNodes("//img[@src]");
            if (imageList != null) ret.AddRange(imageList.Select(link => link.Attributes["src"].Value));
            return ret;
        }
        public static IEnumerable<string> ExtractAnchorLinks(this HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//a[@href]");
            return nodes==null?new List<string>():nodes.Select(link => link.Attributes["href"].Value);
        }


        public static Uri AsUri(this string url)
        {
            return new Uri(url);
        }

    }

}
