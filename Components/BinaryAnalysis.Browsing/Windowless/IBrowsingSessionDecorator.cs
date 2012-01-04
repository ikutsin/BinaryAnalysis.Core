using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Browsing.Windowless
{
    public interface IBrowsingSessionDecorator
    {
        bool IsEnabled { get; }
        void OnInit(IBrowsingSession session);
        void OnRemove(IBrowsingSession session);
        bool OnBeforeRequestStop(IBrowsingSession session, Uri uri);
        bool OnAfterRequestRerun(IBrowsingSession session, Uri uri, IBrowsingResponse response);

        bool OnSwitchProxy(IBrowsingSession browsingSession, Uri uri);
    }
}
