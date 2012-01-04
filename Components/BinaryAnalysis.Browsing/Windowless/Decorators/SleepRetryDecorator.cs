using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace BinaryAnalysis.Browsing.Windowless.Decorators
{
    public class SleepRetryDecorator : EmptyDecorator
    {
        public SleepRetryDecorator()
        {
            Attempts = 2;
            SleepInterval = 1000;
        }
        public int Attempts { get; set; }
        public int SleepInterval { get; set; }

        private int retries;
        public override bool OnBeforeRequestStop(IBrowsingSession session, Uri uri)
        {
            retries = 0;
            return base.OnBeforeRequestStop(session, uri);
        }
        public override bool OnAfterRequestRerun(IBrowsingSession session, Uri uri, IBrowsingResponse response)
        {
            if (retries < Attempts&&response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                retries++;
                Thread.Sleep(SleepInterval);
                return true;
            }
            return base.OnAfterRequestRerun(session, uri, response);
        }
    }
}
