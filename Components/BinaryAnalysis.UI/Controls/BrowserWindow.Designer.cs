namespace BinaryAnalysis.UI.Controls
{
    partial class BrowserWindow
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
            System.Dynamic.ExpandoObject expandoObject1 = new System.Dynamic.ExpandoObject();
            this.browserControl1 = new BinaryAnalysis.UI.Controls.BrowserControl();
            this.SuspendLayout();
            // 
            // browserControl1
            // 
            this.browserControl1.BaseModel = expandoObject1;
            this.browserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserControl1.Location = new System.Drawing.Point(0, 0);
            this.browserControl1.Name = "browserControl1";
            this.browserControl1.Size = new System.Drawing.Size(686, 552);
            this.browserControl1.TabIndex = 0;
            // 
            // BrowserWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 552);
            this.Controls.Add(this.browserControl1);
            this.Name = "BrowserWindow";
            this.Text = "BrowserWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private BrowserControl browserControl1;
    }
}