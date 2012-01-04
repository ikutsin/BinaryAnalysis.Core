using System;

namespace BA.Examples.ScriptingHelper
{
    public static class Extensions
    {
        public static void DispatchInUI(this object obj, Action<object> action, bool wait = false)
        {
            App.CurrentApp.InRenderAction(action, obj, wait);
        }
    }
}
