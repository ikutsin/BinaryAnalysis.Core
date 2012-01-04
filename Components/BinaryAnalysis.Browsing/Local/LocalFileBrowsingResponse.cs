using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Browsing.Windowless;
using System.Net;
using System.IO;
using System.Collections.Specialized;

namespace BinaryAnalysis.Browsing.Local
{
    public class LocalFileBrowsingResponse : IBrowsingResponse
    {
        private string filePath;

        public LocalFileBrowsingResponse(string filePath)
        {
            this.filePath = filePath;
        }
        public HttpStatusCode StatusCode
        {
            get { return File.Exists(filePath) ? HttpStatusCode.OK : HttpStatusCode.NotFound; }
        }

        public string ResponseContent
        {
            get 
            {
                if(!File.Exists(filePath)) return "";
                return File.ReadAllText(filePath);
            }
        }

        Stream _ResponseStream;
        MemoryStream _memStream;
        public Stream ResponseStream
        {
            get {
                if (File.Exists(filePath))
                {
                    if (_ResponseStream == null)
                    {
                        _ResponseStream = File.OpenRead(filePath);
                    }
                    _ResponseStream.Position = 0;
                    return _ResponseStream;
                }
                else
                {
                    if (_memStream == null)
                    {
                        _memStream = new MemoryStream(0);
                    }
                    return _memStream;
                }
            }
        }
        public void Dispose()
        {
            if (_ResponseStream != null) _ResponseStream.Dispose();
            if (_memStream != null) _memStream.Dispose();
        }

        public virtual NameValueCollection Headers
        {
            get { throw new NotImplementedException(); }
        }

        public virtual Uri ResponseUrl
        {
            get
            {
                FileInfo info = new FileInfo(filePath);
                return new Uri("file://" + Path.GetFullPath(filePath));
            }
        }




        public TimeSpan GenerationTime
        {
            get { return TimeSpan.Zero; }
        }
    }
}
