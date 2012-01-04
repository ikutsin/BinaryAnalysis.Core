using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Browsing.Windowless.Decorators
{
    public class AutoRefererDecorator : EmptyDecorator
    {
        private const string HEADER_REFERRER = "Referer";
        private Uri LastRequest = null;
        public override bool OnBeforeRequestStop(IBrowsingSession session, Uri uri)
        {
            if(LastRequest!=null)
            {
                session.Headers[HEADER_REFERRER] = LastRequest.ToString();
                //String.Format(@"{0}://{1}/", uri.Scheme, uri.Host);
            }
            return base.OnBeforeRequestStop(session, uri);
        }
        public override bool OnAfterRequestRerun(IBrowsingSession session, Uri uri, IBrowsingResponse response)
        {
            LastRequest = uri;
            return base.OnAfterRequestRerun(session, uri, response);
        }
    }
}
