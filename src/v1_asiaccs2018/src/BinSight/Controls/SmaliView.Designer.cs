namespace APKInsight.Controls
{
    public partial class SmaliView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rtxtSmaliCode = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtxtSmaliCode
            // 
            this.rtxtSmaliCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtSmaliCode.Location = new System.Drawing.Point(0, 0);
            this.rtxtSmaliCode.Name = "rtxtSmaliCode";
            this.rtxtSmaliCode.ReadOnly = true;
            this.rtxtSmaliCode.Size = new System.Drawing.Size(544, 432);
            this.rtxtSmaliCode.TabIndex = 1;
            this.rtxtSmaliCode.Text = "";
            this.rtxtSmaliCode.MouseClick += new System.Windows.Forms.MouseEventHandler(this.rtxtSmaliCode_MouseClick);
            this.rtxtSmaliCode.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rtxtSmaliCode_MouseMove);
            // 
            // SmaliView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtxtSmaliCode);
            this.Name = "SmaliView";
            this.Size = new System.Drawing.Size(544, 432);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtxtSmaliCode;
    }
}
