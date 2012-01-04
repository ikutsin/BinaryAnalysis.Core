using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autofac;
using BinaryAnalysis.Scheduler;
using BinaryAnalysis.Scheduler.ScriptedCS;
using BinaryAnalysis.Scheduler.Task;
using BinaryAnalysis.Scheduler.Task.Settings;
using BinaryAnalysis.UI.Commons;

namespace BinaryAnalysis.Visuals.Scheduler
{
    public partial class TaskRunnerControl : UserControl
    {
        private SchedulerTask task;

        public TaskRunnerControl()
        {
            InitializeComponent();
            progressBar1.Maximum = ProgressStatus.PROGRESS_MAX+1;
        }

        public SchedulerTask CurrentTask { get { return task; } }
        public event EventHandler OnTaskComplete;

        public SchedulerTask RunTask(Type type, Dictionary<string, object> opts = null)
        {
            return RunTask(f =>
                        {
                            var tp = f.CreateSingleScriptTask<ScsRunnerScript>(opts);
                            return f.InitTaskFromParameters(tp);
                        });
        }

        public SchedulerTask RunTask(string name, Dictionary<string, object> opts = null)
        {
            return RunTask(f => { return f.InitTaskFromContainer(name, opts); });
        }

        public SchedulerTask RunTask(Func<TaskFactory, SchedulerTask> fn)
        {
            progressBar1.Value = 0;
            label1.Text = richTextBox1.Text = "";


            var factory = ProgramContext.Container.Resolve<TaskFactory>();
            task = fn(factory);
            task.TaskStarted += (t) => { };
            task.TaskFinished += (t) => { };
            task.ScriptStarted += (t, s) => { };
            task.ScriptFinished += (t, s) => { };
            task.ScriptCustomEvent += (t, s, o) => { };
            task.Progress.Update +=
                (p) => this.Invoke(new Action(
                                       () =>
                                       {
                                           label1.Text = p.Message;
                                           progressBar1.Value = p.Completeness;
                                           richTextBox1.Text += p.Message + Environment.NewLine;

                                           richTextBox1.SelectionStart = richTextBox1.Text.Length;
                                           richTextBox1.ScrollToCaret();
                                       }));

            var asyncRunner = new Action(
                () =>
                {
                    factory.RunTaskUntilFinished(task);
                    if (OnTaskComplete != null) OnTaskComplete(this, EventArgs.Empty);
                });
            asyncRunner.BeginInvoke(null, null);
            return task;
        }
    }
}
