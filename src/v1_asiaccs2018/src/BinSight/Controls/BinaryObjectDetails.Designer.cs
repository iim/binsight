namespace APKInsight.Controls
{
    partial class BinaryObjectDetails
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
            this.grpMainGroup = new System.Windows.Forms.GroupBox();
            this.lblFileName = new System.Windows.Forms.Label();
            this.lblFileHash = new System.Windows.Forms.Label();
            this.lblSize = new System.Windows.Forms.Label();
            this.lblFileNameValue = new System.Windows.Forms.Label();
            this.lblHashValue = new System.Windows.Forms.Label();
            this.lblSizeValue = new System.Windows.Forms.Label();
            this.grpMainGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMainGroup
            // 
            this.grpMainGroup.Controls.Add(this.lblSizeValue);
            this.grpMainGroup.Controls.Add(this.lblHashValue);
            this.grpMainGroup.Controls.Add(this.lblFileNameValue);
            this.grpMainGroup.Controls.Add(this.lblSize);
            this.grpMainGroup.Controls.Add(this.lblFileHash);
            this.grpMainGroup.Controls.Add(this.lblFileName);
            this.grpMainGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpMainGroup.Location = new System.Drawing.Point(0, 0);
            this.grpMainGroup.Name = "grpMainGroup";
            this.grpMainGroup.Size = new System.Drawing.Size(425, 130);
            this.grpMainGroup.TabIndex = 0;
            this.grpMainGroup.TabStop = false;
            this.grpMainGroup.Text = "Binary Details";
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Location = new System.Drawing.Point(15, 34);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(54, 13);
            this.lblFileName.TabIndex = 0;
            this.lblFileName.Text = "FileName:";
            // 
            // lblFileHash
            // 
            this.lblFileHash.AutoSize = true;
            this.lblFileHash.Location = new System.Drawing.Point(34, 58);
            this.lblFileHash.Name = "lblFileHash";
            this.lblFileHash.Size = new System.Drawing.Size(35, 13);
            this.lblFileHash.TabIndex = 1;
            this.lblFileHash.Text = "Hash:";
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(39, 82);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(30, 13);
            this.lblSize.TabIndex = 2;
            this.lblSize.Text = "Size:";
            // 
            // lblFileNameValue
            // 
            this.lblFileNameValue.AutoSize = true;
            this.lblFileNameValue.Location = new System.Drawing.Point(76, 34);
            this.lblFileNameValue.Name = "lblFileNameValue";
            this.lblFileNameValue.Size = new System.Drawing.Size(0, 13);
            this.lblFileNameValue.TabIndex = 3;
            // 
            // lblHashValue
            // 
            this.lblHashValue.AutoSize = true;
            this.lblHashValue.Location = new System.Drawing.Point(75, 58);
            this.lblHashValue.Name = "lblHashValue";
            this.lblHashValue.Size = new System.Drawing.Size(0, 13);
            this.lblHashValue.TabIndex = 4;
            // 
            // lblSizeValue
            // 
            this.lblSizeValue.AutoSize = true;
            this.lblSizeValue.Location = new System.Drawing.Point(76, 82);
            this.lblSizeValue.Name = "lblSizeValue";
            this.lblSizeValue.Size = new System.Drawing.Size(0, 13);
            this.lblSizeValue.TabIndex = 5;
            // 
            // BinaryObjectDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpMainGroup);
            this.Name = "BinaryObjectDetails";
            this.Size = new System.Drawing.Size(425, 130);
            this.grpMainGroup.ResumeLayout(false);
            this.grpMainGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpMainGroup;
        private System.Windows.Forms.Label lblFileHash;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Label lblSizeValue;
        private System.Windows.Forms.Label lblHashValue;
        private System.Windows.Forms.Label lblFileNameValue;
    }
}
