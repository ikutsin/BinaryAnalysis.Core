/*
 * Created by SharpDevelop.
 * User: Ikutsin
 * Date: 17.02.2011
 * Time: 20:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace BinaryAnalysis.Browsing.Windowless
{
	/// <summary>
	/// Description of BrowsingResponse.
	/// </summary>
	public interface IBrowsingResponse : IDisposable
	{
		HttpStatusCode StatusCode { get; }
		string ResponseContent { get; }
        Stream ResponseStream { get; }

        NameValueCollection Headers { get; }
        Uri ResponseUrl { get; }
        TimeSpan GenerationTime { get; }
	}
}
