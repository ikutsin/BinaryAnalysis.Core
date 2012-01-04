using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autofac;
using BinaryAnalysis.Visuals.Scheduler;
using BinaryAnalysis.Scheduler;
using BinaryAnalysis.Scheduler.ScriptedCS;
using BinaryAnalysis.UI.Commons;

namespace BinaryAnalysis.ScriptingHelper.Development
{
    public partial class StandaloneScripter : Form
    {
        public StandaloneScripter()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            tabPage2.Select();
            
            taskRunnerControl1.RunTask(typeof(ScsRunnerScript), 
                new Dictionary<string, object>
                    {
                        {ScsRunnerScript.SETTINGS_INPUT, baseScincillaWraper1.Scintilla.Text}
                    })
                .TaskFinished += new Scheduler.Task.TaskProgress(StandaloneScripter_TaskFinished);
        }

        void StandaloneScripter_TaskFinished(Scheduler.Task.SchedulerTask sender)
        {
            this.Invoke(new Action(() =>
                                       {
                                           button1.Enabled = true;
                                           taskRunnerControl1.CurrentTask.TaskFinished -=
                                               StandaloneScripter_TaskFinished;
                                           baseScincillaWraper1.WriteErrors();
                                       }));
        }

    }
}
