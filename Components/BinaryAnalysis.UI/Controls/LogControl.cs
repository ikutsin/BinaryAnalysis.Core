using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using BinaryAnalysis.UI.Modules;
using log4net.Core;

namespace BinaryAnalysis.UI.Controls
{
    public partial class LogControl : UserControl
    {
        private const int LOGS_MAX = 50;

        private List<CheckBox> chkLevels;
        private Label lblCount;
        private Button btnClean;

        Queue<LoggingEvent> logInformation = new Queue<LoggingEvent>(LOGS_MAX);

        public LogControl()
        {
            InitializeComponent();


            this.SuspendLayout();
            var levels = new Level[] { Level.Debug, Level.Info, Level.Warn, Level.Error, Level.Fatal };
            chkLevels = new List<CheckBox>(levels.Length);
            foreach (var level in levels)
            {
                var chkLevel = new CheckBox();
                chkLevel.Checked = level != Level.Debug;
                chkLevel.Text = level.ToString();
                chkLevel.Tag = level;
                chkLevel.CheckedChanged += new EventHandler(chkLevel_CheckedChanged);
                pnlFilter.Controls.Add(chkLevel);
                chkLevels.Add(chkLevel);
            }
            btnClean = new Button();
            btnClean.Margin = chkLevels.First().Margin;
            btnClean.Text = "clean";
            btnClean.Click += new EventHandler(btnClean_Click);
            pnlFilter.Controls.Add(btnClean);

            lblCount = new Label();
            lblCount.Margin = chkLevels.First().Margin;
            lblCount.Text = "0";
            pnlFilter.Controls.Add(lblCount);
            this.ResumeLayout(true);
        }

        void btnClean_Click(object sender, EventArgs e)
        {
            logInformation.Clear();
            lblCount.Text = "";
            WriteLog();
        }

        void chkLevel_CheckedChanged(object sender, EventArgs e)
        {
            WriteLog();
        }

        void WriteLog()
        {
            var levels = chkLevels.Where(c => c.Checked).Select(c => c.Tag).Cast<Level>();
            richTextBox1.BeginInvoke(new Action(
                () =>
                {
                    lblCount.Text = "Records: " + logInformation.Count;
                    richTextBox1.Text = "";
                    foreach (var loggingEvent in logInformation.Where(l => levels.Contains(l.Level)).Reverse())
                    {
                        var currentIndex = richTextBox1.Text.Length;
                        var message = loggingEvent.MessageObject + Environment.NewLine;
                        //write text
                        //richTextBox1.Text += message;

                        //richTextBox1.Select(currentIndex, message.Length-2);
                        richTextBox1.SelectionFont = new Font("Arial", 9.75f, FontStyle.Bold);
                        richTextBox1.SelectionColor = Color.Black;
                        richTextBox1.SelectionBackColor = Color.White;

                        if(loggingEvent.Level==Level.Debug)
                        {
                            richTextBox1.SelectionColor = Color.Gray;
                            richTextBox1.SelectionBackColor = Color.Black;
                        }
                        else if (loggingEvent.Level == Level.Warn)
                        {
                            richTextBox1.SelectionBackColor = Color.Red;
                        }
                        else if (loggingEvent.Level == Level.Error)
                        {
                            richTextBox1.SelectionColor = Color.Red;
                            richTextBox1.SelectionBackColor= Color.Black;
                        }
                        richTextBox1.AppendText(message);
                    }
                }));            
        }

        private IDisposable logsSubscriber;
        private IDisposable logsViewer;
        protected override void OnLoad(EventArgs e)
        {
            if (!DesignMode)
            {
                logsSubscriber = FormsLogInjectionModule.NextLoggingEvent()
                    .ToObservable(Scheduler.NewThread)
                    .Where(log => log != null)
                    .Subscribe(AddLogEventAsync);

                logsViewer = Observable
                    .FromEventPattern<EventArgs>(this, "OnNotifyNewLog")
                    .Buffer(TimeSpan.FromSeconds(1))
                    .Subscribe((a) => { if (a.Count > 0) WriteLog(); });

                Disposed +=
                    (s, ee) =>
                        {
                            logsSubscriber.Dispose();
                            logsViewer.Dispose();
                        };
            }
            WriteLog();
            base.OnLoad(e);
        }

        public event EventHandler OnNotifyNewLog;
        private void AddLogEventAsync(LoggingEvent log)
        {
            logInformation.Enqueue(log);
            while (logInformation.Count>LOGS_MAX)
            {
                logInformation.Dequeue();
            }
            if (OnNotifyNewLog != null) OnNotifyNewLog(this, EventArgs.Empty);
        }
    }
}
