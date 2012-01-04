/*
 * Created by SharpDevelop.
 * User: Ikutsin
 * Date: 15.02.2011
 * Time: 20:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using BinaryAnalysis.Browsing.Windowless;
using System.IO;
using System.Collections.Generic;

namespace BinaryAnalysis.Browsing.Windowless
{
	/// <summary>
	/// Description of IBrowsingProxy.
	/// </summary>
	public interface IBrowsingProxy
	{
        string GetCurrentIp(Uri httpUrl, IBrowsingSession info);
        IBrowsingResponse GetResponse(Uri httpUrl, IBrowsingSession info);
        IBrowsingResponse PostResponse(Uri httpUrl, System.Collections.Specialized.NameValueCollection postParamz, IBrowsingSession info);
        IBrowsingResponse FilePostResponse(Uri httpUrl, System.Collections.Specialized.NameValueCollection postParamz, List<Tuple<string, string, Stream>> files, IBrowsingSession info);
    }
}
