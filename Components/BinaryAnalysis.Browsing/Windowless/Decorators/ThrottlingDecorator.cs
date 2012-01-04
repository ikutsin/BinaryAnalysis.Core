using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BinaryAnalysis.Browsing.Windowless.Decorators
{
    public class ThrottlingDecorator : EmptyDecorator
    {
        public int SleepTime { get; set; }
        public ThrottlingDecorator()
        {
            SleepTime = 1000;
        }
        public override bool OnBeforeRequestStop(IBrowsingSession session, Uri uri)
        {
            Thread.Sleep(SleepTime);
            return base.OnBeforeRequestStop(session, uri);
        }
    }
}
