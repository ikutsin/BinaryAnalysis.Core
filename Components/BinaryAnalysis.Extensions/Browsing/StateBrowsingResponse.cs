using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using BinaryAnalysis.Browsing.Windowless;
using EmitMapper;

namespace BinaryAnalysis.Extensions.Browsing
{
    [DataContract, Serializable]
    public class StateBrowsingResponse : IBrowsingResponse
    {
        public static StateBrowsingResponse Create(IBrowsingResponse resp)
        {
            var map = ObjectMapperManager.DefaultInstance.GetMapper<IBrowsingResponse, StateBrowsingResponse>().Map(resp);
            return map;
        }

        [DataMember]
        public HttpStatusCode StatusCode { get; set; }
        [DataMember]
        public string ResponseContent { get; set; }
        [DataMember]
        public NameValueCollection Headers { get; set; }
        [DataMember]
        public Uri ResponseUrl { get; set; }
        [DataMember]
        public TimeSpan GenerationTime { get; set; }


        private MemoryStream responseStream;
        public System.IO.Stream ResponseStream
        {
            get
            {
                if (responseStream == null)
                {
                    byte[] byteArray = Encoding.ASCII.GetBytes(ResponseContent);
                    responseStream = new MemoryStream(byteArray);
                }
                responseStream.Position = 0;
                return responseStream;
            }
        }

        public void Dispose() { }
    }
}
