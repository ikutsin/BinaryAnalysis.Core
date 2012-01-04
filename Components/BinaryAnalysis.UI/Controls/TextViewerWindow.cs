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
    public partial class TextViewerWindow : Form
    {
        public TextViewerWindow() : this("Empty", "")
        {
        }

        public TextViewerWindow(string title, string content)
        {
            InitializeComponent();
            textViewerControl1.Content = content;
            this.Text = title;
        }
    }
}
