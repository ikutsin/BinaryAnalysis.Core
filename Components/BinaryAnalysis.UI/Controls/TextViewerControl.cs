using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BinaryAnalysis.UI.Controls
{
    public partial class TextViewerControl : UserControl
    {
        public TextViewerControl()
        {
            InitializeComponent();

            richTextBox2.ReadOnly = true;
            textBox1.TextChanged +=
                (s, e) =>
                    {
                        bool hasText = !String.IsNullOrWhiteSpace(textBox1.Text);
                        btnHl.Enabled = btnInfo.Enabled = hasText;
                    };
            btnHl.Enabled = btnInfo.Enabled = false;
            btnHl.Click +=
                (s, e) =>
                    {
                        Highlight(textBox1.Text);
                    };
            btnInfo.Click +=
                (s, e) =>
                {
                    ShowMatchInfo(textBox1.Text);
                };
        }

        protected Regex GetRegex(string regex)
        {
            var opts = RegexOptions.Compiled;
            if (chkIC.Checked) opts |= RegexOptions.IgnoreCase;
            if (chkML.Checked) opts |= RegexOptions.Multiline;
            try
            {
                return new Regex(regex, opts);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.ToString());
                return new Regex("");
            }
        }
        protected void ShowMatchInfo(string regexStr)
        {
            var regex = GetRegex(regexStr);
            StringBuilder result = new StringBuilder();

            var matches = regex.Matches(Content);

            foreach (Match match in matches)
            {

                result.Append("Match(" + match.Index + "):");
                result.Append(" Groups:");
                result.Append(match.Groups.Count);
                result.Append(" Captures:");
                result.Append(match.Captures.Count);
                result.AppendLine();
                result.AppendLine(match.Value);
            }

            result.AppendLine();
            result.Append("Matches: ");
            result.AppendLine(matches.Count.ToString());
            result.Append("GroupNames: ");
            result.AppendLine(String.Join(", ", regex.GetGroupNames()));

            richTextBox2.Text = result.ToString();
        }

        public void CleanHighlight()
        {
            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = Color.White;
            richTextBox1.SelectionColor = Color.Black;
        }

        public void Highlight(string regexStr)
        {
            CleanHighlight();
            var regex = GetRegex(regexStr);

            foreach(Match match in regex.Matches(Content))
            {
                richTextBox1.Select(match.Index, match.Length);
                richTextBox1.SelectionBackColor = Color.Violet;
            }
        }

        public string Content { get { return richTextBox1.Text; }set { richTextBox1.Text = value; } }

        private void btn_save_Click(object sender, EventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                Stream myStream;
                if ((myStream = saveDialog.OpenFile()) != null)
                {
                    using (var textWriter = new StreamWriter(myStream))
                    {
                        textWriter.Write(richTextBox1.Text);
                    }
                    myStream.Close();
                }
            }
            
        }
    }
}
