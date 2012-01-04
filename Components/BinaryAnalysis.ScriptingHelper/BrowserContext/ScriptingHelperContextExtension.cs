using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BinaryAnalysis.ScriptingHelper.Development;
using BinaryAnalysis.UI.BrowserContext;

namespace BinaryAnalysis.ScriptingHelper.BrowserContext
{
    [ComVisible(true)]
    public class ScriptingHelperContextExtension : IBrowserContextExtension
    {
        public void startStandalone()
        {
            var form = new StandaloneScripter();
            form.Show();
        }
    }
}
