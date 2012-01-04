using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.Browsing.Windowless.Decorators
{
    public class EmptyDecorator : IBrowsingSessionDecorator
    {
        public EmptyDecorator()
        {
            IsEnabled = true;
        }
        public virtual bool IsEnabled { 
            get;
            set;
        }

        public virtual bool OnBeforeRequestStop(IBrowsingSession session, Uri uri)
        {
            return false;
        }

        public virtual bool OnAfterRequestRerun(IBrowsingSession session, Uri uri, IBrowsingResponse response)
        {
            return false;
        }


        public virtual bool OnSwitchProxy(IBrowsingSession browsingSession, Uri uri)
        {
            return false;
        }

        public virtual void OnInit(IBrowsingSession session) { }
        
        public virtual void OnRemove(IBrowsingSession session) {}
    }
}
