using System;
using System.Windows.Forms;
using BinaryAnalysis.UI.Commons;
using log4net;

namespace BA.Examples.GUI
{
    public partial class MainForm : Form
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainForm));

        public MainForm()
        {
            InitializeComponent();
            int cnt = 0;
            
            log.Debug("debug");
            log.Warn("Warning");
            log.Info("Info");
            log.Error("Error");
            //Observable.Interval(TimeSpan.FromSeconds(.1)).Subscribe((_) => { log.Debug("Tick-tack-"+cnt++);});
        }

        protected override void OnLoad(EventArgs ea)
        {
            try
            {
                //browser tab
                browserControl.DisplayTemplate("MainPage");

                //log tab
                tabLog.VisibleChanged += (s, e) =>
                                             {
                                                 logCount = 0;
                                                 UpdateLogTab();
                                             };
                logControl.OnNotifyNewLog += (s, e) =>
                                                 {
                                                     logCount++;
                                                     UpdateLogTab();
                                                 };
            }
            catch (Exception ex)
            {
                ProgramContext.LogException(ex);
            }
        }
        private int logCount = 0;
        private void UpdateLogTab()
        {
            tabLog.BeginInvoke(new Action(() => tabLog.Text = String.Format("Log ({0})", logCount)));
        }
    }
}
