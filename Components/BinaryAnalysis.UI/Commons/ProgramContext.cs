using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autofac;
using log4net;

namespace BinaryAnalysis.UI.Commons
{
    public delegate void BackChannelHandler(string name, object args);
    public static class ProgramContext
    {
        public static NotificationManager NotificationManager { get; private set; }
        private static readonly ILog log = LogManager.GetLogger(typeof(ProgramContext));

        public static void LogException(Exception ex)
        {
            MessageBox.Show(ex.Message + Environment.NewLine + "Stack:"+ ex.StackTrace, ex.Message);
            log.Error(ex.Message, ex);
        }
        public static IContainer Container { get { return Bootstrap.Instance.Container; }}

        public static void StartApplication<T>(bool bootstrap = true) where T : Form
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool inited = false;
            if (bootstrap)
            {
                using (var init = new BootstrapModal())
                {
                    Application.Run(init);
                    inited = init.IsInitialized;
                }
            }
            else
            {
                inited = true;
            }

            if (inited)
            {
                NotificationManager = new NotificationManager();
                NotificationManager.InitializeTray();
                Application.Run(Activator.CreateInstance(typeof(T)) as Form);
                NotificationManager.Dispose();
            }
            if (Bootstrap.Instance != null) Bootstrap.Instance.Dispose();
        }
    }
}
