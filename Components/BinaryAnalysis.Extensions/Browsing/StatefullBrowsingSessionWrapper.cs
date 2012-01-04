using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using BinaryAnalysis.Browsing.Windowless;
using BinaryAnalysis.Browsing.Windowless.Proxies;
using BinaryAnalysis.Data;

namespace BinaryAnalysis.Extensions.Browsing
{
    [Flags]
    public enum StateCaching
    {
        All, OnlyValid, Never, LikeBrowser
    }
    public enum StateStoringFlags
    {
        None, Get = 1, Post, File
    }
    //Too many copypaste. Dreaming to make it through the interceptor. 
    public class StatefullBrowsingSessionWrapper : IBrowsingSession
    {
        public const string STATE_TRIGGER_PATH = "/BinaryAnalysis.Core/StatefullBrowsingTrigger";
        const string message = "Trigger to flush '{0}' cache.";

        StateService stateService;
        TaxonomyTree tree;
        TaxonomyNode RootBrowsingStateTriggers;

        public StateStoringFlags StoringFlags { get; set; }
        public TimeSpan StoringDuration { get; set; }
        public StateCaching StateCaching { get; set; }

        private IBrowsingSession parent;
        public IBrowsingSession Parent { 
            get { return parent; } 
            set { 
                if(value.GetType()==typeof(StatefullBrowsingSessionWrapper)) throw new InvalidOperationException("Double BrowsingSession wrapping");
                parent = value;
            }
        }

        public StatefullBrowsingSessionWrapper(
            StateService stateService,
            TaxonomyTree tree)
        {
            StoringFlags = StateStoringFlags.Get;
            StateCaching = StateCaching.LikeBrowser;
            StoringDuration = TimeSpan.FromHours(3); //default storing duration 3 hours //one day

            this.stateService = stateService;
            this.tree = tree;

            RootBrowsingStateTriggers =
                tree.GetOrCreatePath(STATE_TRIGGER_PATH, "Trigger to flush all Browsing cache");
        }

        public Encoding ResponseEncoding
        {
            get { return Parent.ResponseEncoding; }
        }

        public NameValueCollection Headers
        {
            get { return Parent.Headers; }
        }

        public CookieContainer CookieContainer
        {
            get { return Parent.CookieContainer; }
        }

        public uint Timeout
        {
            get { return Parent.Timeout; }
            set { Parent.Timeout = value; }
        }

        private string CalculateHash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public IBrowsingResponse NavigateFile(Uri httpUrl, List<Tuple<string, string,Stream>> files, NameValueCollection postParamz)
        {
            if ((StoringFlags & StateStoringFlags.File) == StateStoringFlags.File)
            {
                var txtStr = httpUrl + ":"+String.Join(",", files.Select(d => d.Item2));
                var key = "file_" + CalculateHash(txtStr);
                var hostTrigger = RootBrowsingStateTriggers.GetOrAddChild(httpUrl.Host,
                    String.Format(message, httpUrl.Host));

                var state = stateService.Get<StateBrowsingResponse>(key);
                if (state != null) return state;

                var response = Parent.NavigateFile(httpUrl, files, postParamz);

                if (ShouldCache(response))
                {
                    stateService.Put<StateBrowsingResponse>(key, StateBrowsingResponse.Create(response),
                        hostTrigger, (long)StoringDuration.TotalSeconds, txtStr);
                }

                return response;
            }
            return Parent.NavigateFile(httpUrl, files, postParamz);
        }

        public IBrowsingResponse NavigateFile(Uri httpUrl, List<Tuple<string, string>> filePaths, NameValueCollection postParamz)
        {
            if ((StoringFlags & StateStoringFlags.File) == StateStoringFlags.File)
            {
                var txtStr = httpUrl + ":"+String.Join(",", filePaths);
                var key = "file_" + CalculateHash(txtStr);
                var hostTrigger = RootBrowsingStateTriggers.GetOrAddChild(httpUrl.Host,
                                                                          String.Format(message, httpUrl.Host));
                var state = stateService.Get<StateBrowsingResponse>(key);
                if (state != null) return state;

                var response = Parent.NavigateFile(httpUrl, filePaths, postParamz);

                if (ShouldCache(response))
                {
                    stateService.Put<StateBrowsingResponse>(key, StateBrowsingResponse.Create(response),
                                                        hostTrigger, (long)StoringDuration.TotalSeconds, txtStr);
                }
                return response;
            }
            return Parent.NavigateFile(httpUrl, filePaths, postParamz);
        }

        public IBrowsingResponse NavigateGet(Uri httpUrl)
        {
            if ((StoringFlags & StateStoringFlags.Get) == StateStoringFlags.Get)
            {
                var key = "get_" + httpUrl;
                var hostTrigger = RootBrowsingStateTriggers.GetOrAddChild(httpUrl.Host,
                                                                          String.Format(message, httpUrl.Host));
                var state = stateService.Get<StateBrowsingResponse>(key);
                if (state != null) return state;

                var response = Parent.NavigateGet(httpUrl);
                if (ShouldCache(response))
                {
                    stateService.Put<StateBrowsingResponse>(key, StateBrowsingResponse.Create(response),
                                                        hostTrigger, (long)StoringDuration.TotalSeconds, httpUrl.ToString());
                }
                return response;
            }
            return Parent.NavigateGet(httpUrl); ;
        }

        public IBrowsingResponse NavigatePost(Uri httpUrl, NameValueCollection postParamz)
        {
            if ((StoringFlags & StateStoringFlags.Post) == StateStoringFlags.Post)
            {
                var txt = new StringBuilder();
                txt.Append(httpUrl + ":");
                foreach (string kk in postParamz)
                {
                    txt.Append(kk + "=" + postParamz[kk] + "&");
                }
                var txtStr = txt.ToString();

                var key = "post_" + CalculateHash(txtStr);
                var hostTrigger = RootBrowsingStateTriggers.GetOrAddChild(httpUrl.Host,
                                String.Format(message, httpUrl.Host));

                var state = stateService.Get<StateBrowsingResponse>(key);
                if (state != null) return state;

                var response = Parent.NavigatePost(httpUrl, postParamz);

                if (ShouldCache(response))
                {
                    stateService.Put<StateBrowsingResponse>(key, StateBrowsingResponse.Create(response),
                                hostTrigger, (long)StoringDuration.TotalSeconds, txtStr);
                }
                return response;
                
            }
            return Parent.NavigatePost(httpUrl, postParamz);
        }

        private bool ShouldCache(IBrowsingResponse response)
        {
            switch (StateCaching)
            {
                case StateCaching.All:
                    return true;
                case StateCaching.OnlyValid:
                    return !(response is ErrorBrowsingResponse);
                case StateCaching.Never:
                    return false;
                case StateCaching.LikeBrowser:
                    var doCache = !(response is ErrorBrowsingResponse);
                    if(doCache && !String.IsNullOrEmpty(response.Headers["Cache-Control"]))
                    {
                        doCache =  !(response.Headers["Cache-Control"].Contains("no-cache")
                                      || response.Headers["Cache-Control"].Contains("no-store")
                                      || response.Headers["Cache-Control"].Contains("max-age=0,")
                                      || response.Headers["Cache-Control"].Contains("must-revalidate"));
                    }
                    if(doCache && !String.IsNullOrEmpty(response.Headers["Pragma"]))
                    {
                        doCache = !(response.Headers["Pragma"].Contains("no-cache"));
                    }
                    return doCache;
                default:
                    throw new InvalidOperationException("Unknown StateCaching: " + StateCaching);
            }
        }

        public Uri MakeUrl(string httpUrl, NameValueCollection nvc)
        {
            return Parent.MakeUrl(httpUrl, nvc);
        }
        /// <summary>
        /// remove resource domain
        /// </summary>
        /// <param name="httpUrl"></param>
        public void ClearDomainCacheFor(Uri httpUrl)
        {
            if (RootBrowsingStateTriggers[httpUrl.Host] != null)
            {
                stateService.Trigger(RootBrowsingStateTriggers[httpUrl.Host], false);
            }
        }
        /// <summary>
        /// only this resource
        /// </summary>
        /// <param name="httpUrl"></param>
        public void ClearCacheEntry(Uri httpUrl)
        {
            stateService.Remove<StateBrowsingResponse>("post_" + httpUrl);
            stateService.Remove<StateBrowsingResponse>("file_" + httpUrl);
            stateService.Remove<StateBrowsingResponse>("get_" + httpUrl);
        }
        public void ClearAllCache()
        {
            //remove cache
            stateService.Trigger(tree.Find(STATE_TRIGGER_PATH).First(), true);
        }



        public IBrowsingProxy CurrentProxy
        {
            get { return Parent.CurrentProxy; }
            set { Parent.CurrentProxy = value; }
        }

        public IList<IBrowsingSessionDecorator> Decorators
        {
            get {return Parent.Decorators; }
        }


        public void AddDecorator(IBrowsingSessionDecorator decorator)
        {
            Parent.AddDecorator(decorator);
        }

        public void RemoveDecorator(IBrowsingSessionDecorator decorator)
        {
            Parent.RemoveDecorator(decorator);
        }


        public void RemoveDecorators<T>() where T : IBrowsingSessionDecorator
        {
            Parent.RemoveDecorators<T>();
        }


        public bool TrySwitchProxy(Uri uri)
        {
            return Parent.TrySwitchProxy(uri);
        }


        public void ClearCookieContainer()
        {
            Parent.ClearCookieContainer();
        }


        public string GetCurrentIp(Uri uri)
        {
            return Parent.GetCurrentIp(uri);
        }
    }
}
