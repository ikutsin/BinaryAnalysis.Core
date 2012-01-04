using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;

namespace BinaryAnalysis.Browsing.Windowless
{
    public interface IBrowsingSession
    {
        IList<IBrowsingSessionDecorator> Decorators { get; }
        void AddDecorator(IBrowsingSessionDecorator decorator);
        void RemoveDecorator(IBrowsingSessionDecorator decorator);
        void RemoveDecorators<T>() where T : IBrowsingSessionDecorator;

        Encoding ResponseEncoding { get; }
        NameValueCollection Headers { get; }
        CookieContainer CookieContainer { get; }
        IBrowsingProxy CurrentProxy { get; set; }

        string GetCurrentIp(Uri uri);
        bool TrySwitchProxy(Uri uri);
        uint Timeout { get; set; }

        IBrowsingResponse NavigateFile(Uri httpUrl, List<Tuple<string, string,Stream>> files, NameValueCollection postParamz);
        IBrowsingResponse NavigateFile(Uri httpUrl, List<Tuple<string, string>> filePaths, NameValueCollection postParamz);
        IBrowsingResponse NavigateGet(Uri httpUrl);
        IBrowsingResponse NavigatePost(Uri httpUrl, NameValueCollection postParamz);
        Uri MakeUrl(string httpUrl, NameValueCollection nvc);

        void ClearCookieContainer();
    }
}
