using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using BinaryAnalysis.Scheduler.Task;

namespace BinaryAnalysis.Visuals.Scheduler
{
    public partial class TaskRunnerForm : Form
    {
        private readonly string taskName;
        private readonly Dictionary<string, object> opts;
        private readonly bool autoclose;

        public TaskRunnerForm(string taskName, Dictionary<string, object> opts, bool autoclose = false)
        {
            this.taskName = taskName;
            this.opts = opts;
            this.autoclose = autoclose;
            InitializeComponent();
            button1.Enabled = buttonSettings.Enabled = false;
        }

        public SchedulerTask CurrentTask { get { return taskRunnerControl.CurrentTask; } } 

        protected override void OnLoad(EventArgs e)
        {
            Text = taskName + " runner";
            taskRunnerControl.OnTaskComplete +=
                (_, __) => Invoke(new Action(
                    ()=> {
                        buttonSettings.Enabled = 
                        button1.Enabled = true;

                        if(autoclose) this.Close();
                    }));
            taskRunnerControl.RunTask(taskName, opts);
            base.OnLoad(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            this.Close();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !button1.Enabled;
            base.OnClosing(e);
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}
