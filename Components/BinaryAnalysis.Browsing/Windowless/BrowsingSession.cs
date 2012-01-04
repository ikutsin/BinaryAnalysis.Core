/*
 * Created by SharpDevelop.
 * User: Ikutsin
 * Date: 15.02.2011
 * Time: 20:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BinaryAnalysis.Browsing.Windowless;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Web;

namespace BinaryAnalysis.Browsing.Windowless
{
	/// <summary>
	/// Description of IBrowsingSession.
	/// </summary>
	public class BrowsingSession : IBrowsingSession
	{
		public BrowsingSession() 
		{
            Timeout = 10000;
		}

	    private List<IBrowsingSessionDecorator> decorators = new List<IBrowsingSessionDecorator>();
        public IList<IBrowsingSessionDecorator> Decorators
        {
            get { return decorators.AsReadOnly(); }
        }
        public void AddDecorator(IBrowsingSessionDecorator decorator)
        {
            decorator.OnInit(this);
            decorators.Add(decorator);
        }
        public void RemoveDecorator(IBrowsingSessionDecorator decorator)
        {
            var dd = decorators.Find(x => x == decorator);
            dd.OnRemove(this);
            decorators.Remove(dd);
        }

        public void RemoveDecorators<T>() where T : IBrowsingSessionDecorator
        {
            var dd = decorators.Where(x => x is T);
            foreach (IBrowsingSessionDecorator browsingSessionDecorator in dd)
            {
                browsingSessionDecorator.OnRemove(this);
            }
            decorators.RemoveAll(x => dd.Contains(x));
        }
		
		protected NameValueCollection headers = new NameValueCollection();
		protected CookieContainer cookieContainer = new CookieContainer();
	    protected Encoding responseEncoding = Encoding.Default;

        #region IBrowsingSessionInfo
        public Encoding ResponseEncoding
        {
            set { responseEncoding = value; }
            get { return responseEncoding; }
        }

        public NameValueCollection Headers
        {
            get { return headers; }
        }

        public CookieContainer CookieContainer
        {
            get { return cookieContainer; }
        }
        public uint Timeout { get; set; }
        #endregion

        public IBrowsingProxy CurrentProxy { get; set; }

        public Uri MakeUrl(string httpUrl, NameValueCollection nvc)
        {
            if (httpUrl.Contains("?")) throw new Exception("Invalid link for parameters");
            return
                new Uri(httpUrl + "?" +
                        string.Join("&",
                                    Array.ConvertAll(nvc.AllKeys,
                                                     key =>
                                                     string.Format("{0}={1}", HttpUtility.UrlEncode(key),
                                                                   HttpUtility.UrlEncode(nvc[key])))));
        }

        public IBrowsingResponse NavigateGet(Uri httpUrl)
        {
            if (decorators.FindAll(x => x.IsEnabled).Select(x => x.OnBeforeRequestStop(this, httpUrl))
                .Any(x => x)) return null;
            IBrowsingResponse browsingResponse = null;
            do
            {
                browsingResponse = CurrentProxy.GetResponse(httpUrl, this);
            } while (
                decorators.FindAll(x => x.IsEnabled).Select(x => x.OnAfterRequestRerun(this, httpUrl, browsingResponse))
                    .Any(x => x));
            return browsingResponse;
        }


	    public IBrowsingResponse NavigatePost(Uri httpUrl, NameValueCollection postParamz)
        {
            if (decorators.FindAll(x => x.IsEnabled).Select(x => x.OnBeforeRequestStop(this, httpUrl))
                .Any(x => x)) return null; 
             IBrowsingResponse browsingResponse = null;
            do
            {
                browsingResponse = CurrentProxy.PostResponse(httpUrl, postParamz, this);
            } while (
                decorators.FindAll(x => x.IsEnabled).Select(x => x.OnAfterRequestRerun(this, httpUrl, browsingResponse))
                    .Any(x => x));
            return browsingResponse;
		}
        public IBrowsingResponse NavigateFile(Uri httpUrl, List<Tuple<string, string>> filePaths, NameValueCollection postParamz)
        {
            IBrowsingResponse response = null;
            List<Tuple<string, string, Stream>> files = new List<Tuple<string, string, Stream>>();
            try
            {
                filePaths.ForEach((filepath) =>
                {
                    var file = File.OpenRead(filepath.Item2);
                    files.Add(
                        new Tuple<string, string, Stream>(filepath.Item1, Path.GetFileName(filepath.Item2), file));
                });
                response = NavigateFile(httpUrl, files, postParamz);
            }
            catch (Exception ex)
            {
                throw new Exception("Upload failed", ex);
            }
            finally
            {
                foreach (var tuple in files)
                {
                    tuple.Item3.Close();
                }
            }
            return response;
        }

        public IBrowsingResponse NavigateFile(Uri httpUrl, List<Tuple<string, string, Stream>> files, NameValueCollection postParamz)
        {
            if (decorators.FindAll(x => x.IsEnabled).Select(x => x.OnBeforeRequestStop(this, httpUrl))
                .Any(x => x)) return null;

            IBrowsingResponse browsingResponse = null;
            do
            {
                browsingResponse = CurrentProxy.FilePostResponse(httpUrl, postParamz, files, this);
            } while (
                decorators.FindAll(x => x.IsEnabled).Select(x => x.OnAfterRequestRerun(this, httpUrl, browsingResponse))
                    .Any(x => x));
            return browsingResponse;
		}

        public virtual bool TrySwitchProxy(Uri uri)
        {
            return decorators.FindAll(x => x.IsEnabled)
                .Any(x => x.OnSwitchProxy(this, uri));
        }

        public void ClearCookieContainer()
        {
            cookieContainer = new CookieContainer();
        }


        public string GetCurrentIp(Uri uri)
        {
            return CurrentProxy.GetCurrentIp(uri, this);
        }
    }
}
