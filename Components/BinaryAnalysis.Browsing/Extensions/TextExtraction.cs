using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BinaryAnalysis.Browsing.Windowless;
using System.Xml.Linq;

namespace BinaryAnalysis.Browsing.Extensions
{
    //http://gskinner.com/RegExr/

    public static class TextExtraction
    {
        private static List<Regex> linesToDictListDefault = new List<Regex>
        {
            new Regex(@"^([A-Za-z0-9 _]+): (.*)$", RegexOptions.Compiled | RegexOptions.Multiline),
            new Regex(@"^\[([A-Za-z0-9 _]+)\]= (.*)$", RegexOptions.Compiled | RegexOptions.Multiline),
        };
        private static List<Regex> skipListDefault = new List<Regex>
        {
            new Regex(@"^###", RegexOptions.Compiled),
            new Regex(@"^;", RegexOptions.Compiled),
        };

        public static string RemoveUnusedLines(this string text, List<Regex> skipList = null)
        {
            if (skipList == null) skipList = skipListDefault;
            string txt;
            using (StringReader reader = new StringReader(text))
            using (StringWriter writer = new StringWriter())
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var tline = line.Trim();
                    if (tline.Length > 0 && !skipListDefault.Any(x=>x.IsMatch(tline)))
                        writer.WriteLine(tline);
                }
                txt = writer.ToString();
            }
            return txt;
        }
        public static Dictionary<string, string> LinesToDictionary(this string text, List<Regex> linesToDictList = null)
        {
            if (linesToDictList == null) linesToDictList = linesToDictListDefault;
            var paramz = new Dictionary<string, string>();

            foreach (var nameValues in linesToDictList)
            {
                foreach (Match m in nameValues.Matches(text))
                {
                    if (!paramz.ContainsKey(m.Groups[1].Value))
                    {
                        paramz.Add(m.Groups[1].Value, m.Groups[2].Value
                            .Replace("<br/>", "")
                            .Replace("\r", ""));
                    }
                }
            }
            return paramz;
        }
        public static string ToCleanText(this XContainer data)
        {
            var text = "";
            foreach (var elem in data.Descendants()
                .Where(x => x.Name == "script" || x.Name == "head")
                .ToList())
            {
                elem.Remove();
            }

            foreach (var elem in data.Descendants().Where(x => x.Name == "br"))
            {
                elem.Value = Environment.NewLine;
            }
            foreach (var elem in data.Descendants().Where(x => x.Name == "p"))
            {
                elem.Value = Environment.NewLine + elem.Value + Environment.NewLine;
            }

            foreach (XText node in data.DescendantNodes().OfType<XText>())
            {
                text += node.Value + " ";
            }

            bool lexHasNewLine = false;
            bool lexHasSpace = false;
            var retBuilder = new StringBuilder();

            var cutSymbols = new List<char>() { ' ', '\t', '\r' };
            var skipSymbols = new List<char>() { '\u00A0', '\u20E7' };

            foreach (var c in text)
            {
                if (c == '\n') lexHasNewLine = true;
                else if (cutSymbols.Contains(c)) lexHasSpace = true;
                else if (skipSymbols.Contains(c)) { }
                else
                {
                    if (lexHasNewLine) retBuilder.Append(Environment.NewLine);
                    else if (lexHasSpace) retBuilder.Append(" ");

                    retBuilder.Append(c);
                    lexHasNewLine = lexHasSpace = false;
                }

            }
            return retBuilder.ToString();
        }

        static Regex proxyIpRegex = new Regex(@"(([0-2]?[0-9]?[0-9])\.([0-2]?[0-9]?[0-9])\.([0-2]?[0-9]?[0-9])\.([0-2]?[0-9]?[0-9]))([\s|:]*([0-9]{2,4}))?", RegexOptions.Compiled);
        static Regex proxyNameRegex = new Regex(@"([0-9a-z_\-\.]+\.[a-z]{2,5})([\s|:]*([0-9]{2,4}))?",
                                              RegexOptions.Compiled & RegexOptions.IgnoreCase);

        static Dictionary<string, string> Replacements = new Dictionary<string, string>()
        {
            //proxyfire
            { "\u0030", "0" },
            { "\u0031", "1" },
            { "\u0032", "2" },
            { "\u0033", "3" },
            { "\u0034", "4" },
            { "\u0035", "5" },
            { "\u0036", "6" },
            { "\u0037", "7" },
            { "\u0038", "8" },
            { "\u0039", "9" },

        };

        public static List<Uri> AsListOfProxies(this IBrowsingResponse response)
        {
            var text = response.AsFixedXML().ToCleanText();

            foreach (var i in Replacements) text = text.Replace(i.Key, i.Value);
            var ret = new List<Uri>();

            var matchesIp = proxyIpRegex.Matches(text);
            foreach (Match m in matchesIp)
            {
                var ip = m.Groups[1].Value;
                var port = String.IsNullOrEmpty(m.Groups[7].Value) ?
                    "3128" : m.Groups[7].Value;
                var uriStr = String.Format(@"http://{0}:{1}", ip, port);
                ret.Add(new Uri(uriStr));
            }

            /* - TODO Proxy resolver by name
            var matchesName = proxyNameRegex.Matches(text);
            foreach (Match m in matchesName)
            {
                var ip = m.Groups[1].Value;
                var port = String.IsNullOrEmpty(m.Groups[7].Value) ?
                    "3128" : m.Groups[3].Value;
                var uriStr = String.Format(@"http://{0}:{1}", ip, port);
                ret.Add(new Uri(uriStr));
            }
             */

            return ret;
        }
    }
}
