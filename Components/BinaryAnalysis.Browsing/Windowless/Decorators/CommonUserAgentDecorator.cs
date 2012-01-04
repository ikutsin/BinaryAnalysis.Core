using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Browsing.Windowless.Decorators
{
    /// <summary>
    /// One of: http://www.user-agents.org/index.shtml?moz
    /// </summary>
    public class CommonUserAgentDecorator : EmptyDecorator
    {
        public override void OnInit(IBrowsingSession session)
        {
            session.Headers["User-Agent"] = @"Mozilla/5.0 (X11; U; Linux i686; de-AT; rv:1.8.0.2) Gecko/20060309 SeaMonkey/1.0";
            session.Headers["Accept"] = @"image/png,image/*;q=0.8,*/*;q=0.5";
            session.Headers["Accept-Language"] = @"en-us,en;q=0.5";
            //session.Headers["Accept-Encoding"] = @"gzip,deflate";
            session.Headers["Accept-Charset"] = @"utf-8;q=0.7,*;q=0.7";
            session.Headers["Keep-Alive"] = @"115";
        }
    }
}
