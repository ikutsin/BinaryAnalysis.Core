using System.Collections.ObjectModel;
using System.Linq;
using BA.Examples.ScriptingHelper.Models;
using BA.Examples.ScriptingHelper.ViewModels.Utils;
using Fiddler;
using mshtml;

namespace BA.Examples.ScriptingHelper.ViewModels
{
    public class FiddlerPageInformationVm : AbstractVm
    {
        public ObservableCollection<NameValueItem> RequestHeaders { get; set; }
        public ObservableCollection<NameValueItem> ResponseHeaders { get; set; }
        public ObservableCollection<NameValueItem> QueryParams { get; set; }
        public ObservableCollection<InputTagInfo> InputTags { get; set; }

        private string requestType;
        public string RequestType
        {
            get { return requestType; }
            set { requestType = value; NotifyPropertyChanged("RequestType"); }
        }
        private string requestUrl;
        public string RequestUrl
        {
            get { return requestUrl; }
            set { requestUrl = value; NotifyPropertyChanged("RequestUrl"); }
        }

        public FiddlerPageInformationVm()
        {
            Clean();
        }
        public void Clean()
        {
            InputTags = new ObservableCollection<InputTagInfo>();
            NotifyPropertyChanged("InputTags");
            RequestHeaders = new ObservableCollection<NameValueItem>();
            NotifyPropertyChanged("RequestHeaders");
            ResponseHeaders = new ObservableCollection<NameValueItem>();
            NotifyPropertyChanged("ResponseHeaders");
            QueryParams = new ObservableCollection<NameValueItem>();
            NotifyPropertyChanged("QueryParams");
        }
        public void Bind(HTMLDocument dom)
        {
            Clean();
            var pageInfo = this;

            //input fields
            foreach (dynamic item in dom.all)
            {
                if (item.tagName == "INPUT")
                {
                    pageInfo.InputTags.Add(new InputTagInfo
                    {
                        TagName = item.tagName,
                        Name = item.name,
                        Value = item.value,
                        Type = item.type
                    });
                    //FiddlerHelper.Log(String.Format("input '{2}' {0}={1}", item.name, item.value, item.type));
                }
                else if (item.tagName == "BUTTON")
                {
                    FiddlerHelper.Log("BUTTON:" + item);
                }
            }
            //headers
            var sess = FiddlerHelper.GetSessionsStack().First();
            foreach (HTTPHeaderItem h in sess.FiddlerSession.oResponse.headers)
            {
                pageInfo.ResponseHeaders.Add(new NameValueItem { Name = h.Name, Value = h.Value });
            }
            foreach (HTTPHeaderItem h in sess.FiddlerSession.oRequest.headers)
            {
                pageInfo.RequestHeaders.Add(new NameValueItem { Name = h.Name, Value = h.Value });
            }
            pageInfo.RequestType = sess.FiddlerSession.oRequest.headers.HTTPMethod;

            pageInfo.RequestUrl = sess.BrowsingResponse.ResponseUrl.ToString();


            //query string
            string query = "";
            switch (pageInfo.RequestType)
            {
                case "GET":
                    var q = sess.FiddlerSession.oRequest.headers.RequestPath;
                    var queryStringIndex = q.IndexOf("?");
                    if (queryStringIndex >= 0)
                    {
                        query = q.Substring(queryStringIndex + 1);
                    }
                    break;
                case "POST":
                    query = sess.FiddlerSession.GetRequestBodyAsString();
                    break;
            }
            var qarr = query.Split('&');
            foreach (string qq in qarr)
            {
                var q = qq.Split('=');
                if (q[0].Length > 0) pageInfo.QueryParams.Add(new NameValueItem { Name = q[0], Value = q[1] });
            }
        }
    }
}
