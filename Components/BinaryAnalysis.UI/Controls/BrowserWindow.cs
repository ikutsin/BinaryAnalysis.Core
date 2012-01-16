using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BinaryAnalysis.UI.Controls
{
    public partial class BrowserWindow : Form
    {
        private readonly string _content;

        public BrowserWindow(string title, string content)
        {
            _content = content;
            InitializeComponent();
            this.Name = title;
        }

        protected override void OnLoad(EventArgs e)
        {
            browserControl1.DisplayContent(_content);
        }
    }
}
