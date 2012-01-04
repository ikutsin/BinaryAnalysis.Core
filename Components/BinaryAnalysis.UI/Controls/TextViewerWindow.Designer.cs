namespace BinaryAnalysis.UI.Controls
{
    partial class TextViewerWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textViewerControl1 = new BinaryAnalysis.UI.Controls.TextViewerControl();
            this.SuspendLayout();
            // 
            // textViewerControl1
            // 
            this.textViewerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textViewerControl1.Location = new System.Drawing.Point(0, 0);
            this.textViewerControl1.Name = "textViewerControl1";
            this.textViewerControl1.Size = new System.Drawing.Size(618, 559);
            this.textViewerControl1.TabIndex = 0;
            // 
            // TextViewerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 559);
            this.Controls.Add(this.textViewerControl1);
            this.Name = "TextViewerWindow";
            this.Text = "TextViewerWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private TextViewerControl textViewerControl1;

    }
}