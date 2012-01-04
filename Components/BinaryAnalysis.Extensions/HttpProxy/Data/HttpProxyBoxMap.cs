using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using BinaryAnalysis.Data.Box;
using Newtonsoft.Json;

namespace BinaryAnalysis.Extensions.HttpProxy.Data
{
    [DataContract]
    public enum HttpProxyType : int
    {
        [EnumMember]
        Undefined = 0,
        [EnumMember]
        Transparent,
        [EnumMember]
        Anonymous,
        [EnumMember]
        HighAnonymous
    }

    [Serializable]
    [XmlType("HttpProxy")]
    [DataContract]
    [JsonObject]
    [KnownType(typeof(HttpProxyType))]
    public class HttpProxyBoxMap : EntityBoxMap
    {
        [DataMember, XmlElement, JsonProperty]
        public DateTime FirstUpdate { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public DateTime LastLive { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public DateTime LastUpdate { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public string Country { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public string ResolverName { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public string ProviderName { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public HttpProxyType Type { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public TimeSpan PingTime { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public string Comment { get; set; }
        [DataMember, XmlElement, JsonProperty]
        public string IP { get; set; }
    }
}
