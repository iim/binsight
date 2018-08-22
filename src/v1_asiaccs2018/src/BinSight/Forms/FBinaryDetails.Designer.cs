namespace APKInsight.Forms
{
    partial class FBinaryDetails
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
            this.tbcTabControl = new System.Windows.Forms.TabControl();
            this.tbpBinaryView = new System.Windows.Forms.TabPage();
            this.tbpSmaliView = new System.Windows.Forms.TabPage();
            this.UBinaryObjectBinaryView = new APKInsight.Controls.BinaryObjectBinaryView();
            this.UBinaryObjectDetails = new APKInsight.Controls.BinaryObjectDetails();
            this.UBinaryObjectSmaliView = new APKInsight.Controls.BinaryObjectSmaliView();
            this.tbcTabControl.SuspendLayout();
            this.tbpBinaryView.SuspendLayout();
            this.tbpSmaliView.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbcTabControl
            // 
            this.tbcTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbcTabControl.Controls.Add(this.tbpBinaryView);
            this.tbcTabControl.Controls.Add(this.tbpSmaliView);
            this.tbcTabControl.Location = new System.Drawing.Point(12, 148);
            this.tbcTabControl.Name = "tbcTabControl";
            this.tbcTabControl.SelectedIndex = 0;
            this.tbcTabControl.Size = new System.Drawing.Size(804, 463);
            this.tbcTabControl.TabIndex = 1;
            // 
            // tbpBinaryView
            // 
            this.tbpBinaryView.Controls.Add(this.UBinaryObjectBinaryView);
            this.tbpBinaryView.Location = new System.Drawing.Point(4, 22);
            this.tbpBinaryView.Name = "tbpBinaryView";
            this.tbpBinaryView.Padding = new System.Windows.Forms.Padding(3);
            this.tbpBinaryView.Size = new System.Drawing.Size(796, 437);
            this.tbpBinaryView.TabIndex = 0;
            this.tbpBinaryView.Text = "Binary View";
            this.tbpBinaryView.UseVisualStyleBackColor = true;
            // 
            // tbpSmaliView
            // 
            this.tbpSmaliView.Controls.Add(this.UBinaryObjectSmaliView);
            this.tbpSmaliView.Location = new System.Drawing.Point(4, 22);
            this.tbpSmaliView.Name = "tbpSmaliView";
            this.tbpSmaliView.Padding = new System.Windows.Forms.Padding(3);
            this.tbpSmaliView.Size = new System.Drawing.Size(796, 437);
            this.tbpSmaliView.TabIndex = 1;
            this.tbpSmaliView.Text = "Smali View";
            this.tbpSmaliView.UseVisualStyleBackColor = true;
            // 
            // UBinaryObjectBinaryView
            // 
            this.UBinaryObjectBinaryView.BinaryObject = null;
            this.UBinaryObjectBinaryView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UBinaryObjectBinaryView.Location = new System.Drawing.Point(3, 3);
            this.UBinaryObjectBinaryView.Name = "UBinaryObjectBinaryView";
            this.UBinaryObjectBinaryView.Size = new System.Drawing.Size(790, 431);
            this.UBinaryObjectBinaryView.TabIndex = 0;
            // 
            // UBinaryObjectDetails
            // 
            this.UBinaryObjectDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UBinaryObjectDetails.BinaryObject = null;
            this.UBinaryObjectDetails.Location = new System.Drawing.Point(12, 12);
            this.UBinaryObjectDetails.Name = "UBinaryObjectDetails";
            this.UBinaryObjectDetails.Size = new System.Drawing.Size(804, 130);
            this.UBinaryObjectDetails.TabIndex = 0;
            // 
            // UBinaryObjectSmaliView
            // 
            this.UBinaryObjectSmaliView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UBinaryObjectSmaliView.Location = new System.Drawing.Point(3, 3);
            this.UBinaryObjectSmaliView.Name = "UBinaryObjectSmaliView";
            this.UBinaryObjectSmaliView.Size = new System.Drawing.Size(790, 431);
            this.UBinaryObjectSmaliView.TabIndex = 0;
            // 
            // FBinaryDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 623);
            this.Controls.Add(this.tbcTabControl);
            this.Controls.Add(this.UBinaryObjectDetails);
            this.Name = "FBinaryDetails";
            this.Text = "Binary Details";
            this.Load += new System.EventHandler(this.FBinaryDetails_Load);
            this.Closed += new System.EventHandler(this.FBinaryDetails_Closed);
            this.tbcTabControl.ResumeLayout(false);
            this.tbpBinaryView.ResumeLayout(false);
            this.tbpSmaliView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.BinaryObjectDetails UBinaryObjectDetails;
        private System.Windows.Forms.TabControl tbcTabControl;
        private System.Windows.Forms.TabPage tbpBinaryView;
        private Controls.BinaryObjectBinaryView UBinaryObjectBinaryView;
        private System.Windows.Forms.TabPage tbpSmaliView;
        private Controls.BinaryObjectSmaliView UBinaryObjectSmaliView;
    }
}