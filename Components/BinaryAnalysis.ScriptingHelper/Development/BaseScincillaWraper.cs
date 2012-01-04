using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BinaryAnalysis.Scheduler.ScriptedCS;
using ScintillaNet;

namespace BinaryAnalysis.ScriptingHelper.Development
{
    public partial class BaseScincillaWraper : UserControl
    {
        public BaseScincillaWraper()
        {
            InitializeComponent();
            var ui = TaskScheduler.FromCurrentSynchronizationContext();

            if (Process.GetCurrentProcess().ProcessName != "devenv")
            {
                _sca = new Scintilla();
                Initialize(_sca);
                _sca.CharAdded += new EventHandler<CharAddedEventArgs>(CharAdded);
                splitContainer1.Panel1.Controls.Add(_sca);

                var deafultColor = _sca.BackColor;
                _sca.BackColor = Color.Gray;
                _sca.Enabled = false;
                Task.Factory
                    .StartNew(EvaluationHelper.InitEvaluator)
                    .ContinueWith(_ =>
                                      {
                                          _sca.Enabled = true;
                                          _sca.BackColor = deafultColor;
                                      }, ui);
            }
            else
            {
                BackColor = Color.White;
            }
        }

        public void WriteErrors()
        {
            this.Invoke(new Action(() =>
                                       {
                                           richTextBox1.Text = EvaluationHelper.Errors.ToString();
                                           EvaluationHelper.Errors.GetStringBuilder().Clear();
                                       }));
        }

        protected virtual void CharAdded(object sender, CharAddedEventArgs e)
        {
            if (e.Ch == '(')
            {
                this._sca.CallTip.Show("TODO: parameters",_sca.CurrentPos);
            }
            else
            {
                this._sca.CallTip.Cancel();
                try
                {
                    string entry = GetSentenceAt(_sca.CurrentPos);
                    this._sca.CallTip.Show(entry, _sca.CurrentPos);
                    var autocomplete = EvaluationHelper.GetCompletions(entry, _sca.Text);
                    if (autocomplete.Count > 0)
                    {
                        if (entry.Length == 1 || entry.Length > 2 && entry[entry.Length - 2] == '.')
                        {
                            _sca.AutoComplete.Show(_sca.GetWordFromPosition(_sca.CurrentPos).Length, autocomplete);
                        }
                        _sca.AutoComplete.List = autocomplete;
                    }

                }
                catch (Exception ex)
                {
                    this._sca.CallTip.Show(ex.Message, _sca.CurrentPos);
                }

            }
        }

        private string GetSentenceAt(int currentPos)
        {
            if (_sca.Text[currentPos - 1] == '.') return "";
            var result = new List<string>();
            var position = currentPos;
            do
            {
                var str = _sca.GetWordFromPosition(position);
                if (str.Length > 0) result.Add(str);
                position -= str.Length+1;
                if(position<=0||_sca.Text[position]!='.') break;
            } while (true);
            result.Reverse();
            string sentenceAt = String.Join(".", result);
            if (_sca.Text[currentPos - 1] == '.') sentenceAt += ".";
            return sentenceAt;
        }

        private Scintilla _sca;
        public Scintilla Scintilla
        {
            get { return _sca; }
        }

        protected virtual void Initialize(Scintilla sca)
        {
            sca.Dock = DockStyle.Fill;
            sca.Margins[0].Width = 10;
            sca.ConfigurationManager.Language = "cs";
            sca.AutoComplete.List.Clear();
            sca.AutoComplete.DropRestOfWord = true;
            sca.AutoComplete.AutoHide = false;            
        }
    }
}
