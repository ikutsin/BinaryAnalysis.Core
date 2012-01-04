using System.Runtime.InteropServices;
using log4net;

namespace BinaryAnalysis.UI.BrowserContext
{
    [ComVisible(true)]
    public class LoggingContextExtensions : IBrowserContextExtension
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoggingContextExtensions));
        public void info(object obj)
        {
            log.Info(obj);
        }
        public void debug(object obj)
        {
            log.Debug(obj);
        }
        public void error(object obj)
        {
            log.Error(obj);
        }
        public void warn(object obj)
        {
            log.Warn(obj);
        }
    }
}
