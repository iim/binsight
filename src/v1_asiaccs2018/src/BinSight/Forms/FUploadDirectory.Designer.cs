namespace APKInsight.Forms
{
    partial class FUploadDirectory
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblFoundObjects = new System.Windows.Forms.Label();
            this.lblSelectedDirectory = new System.Windows.Forms.Label();
            this.btnSelectDirectory = new System.Windows.Forms.Button();
            this.dlgDirectorySelectionDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.grpUploadProgress = new System.Windows.Forms.GroupBox();
            this.cmbDataSets = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblProgress = new System.Windows.Forms.Label();
            this.prbUploadProgress = new System.Windows.Forms.ProgressBar();
            this.btnStartUploading = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.grpUploadProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblFoundObjects);
            this.groupBox1.Controls.Add(this.lblSelectedDirectory);
            this.groupBox1.Controls.Add(this.btnSelectDirectory);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1047, 72);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "STEP 1: Select Directory";
            // 
            // lblFoundObjects
            // 
            this.lblFoundObjects.AutoSize = true;
            this.lblFoundObjects.Location = new System.Drawing.Point(108, 39);
            this.lblFoundObjects.Name = "lblFoundObjects";
            this.lblFoundObjects.Size = new System.Drawing.Size(27, 13);
            this.lblFoundObjects.TabIndex = 2;
            this.lblFoundObjects.Text = "N/A";
            // 
            // lblSelectedDirectory
            // 
            this.lblSelectedDirectory.AutoSize = true;
            this.lblSelectedDirectory.Location = new System.Drawing.Point(108, 21);
            this.lblSelectedDirectory.Name = "lblSelectedDirectory";
            this.lblSelectedDirectory.Size = new System.Drawing.Size(69, 13);
            this.lblSelectedDirectory.TabIndex = 1;
            this.lblSelectedDirectory.Text = "Not Selected";
            // 
            // btnSelectDirectory
            // 
            this.btnSelectDirectory.Location = new System.Drawing.Point(18, 29);
            this.btnSelectDirectory.Name = "btnSelectDirectory";
            this.btnSelectDirectory.Size = new System.Drawing.Size(75, 23);
            this.btnSelectDirectory.TabIndex = 0;
            this.btnSelectDirectory.Text = "Select";
            this.btnSelectDirectory.UseVisualStyleBackColor = true;
            this.btnSelectDirectory.Click += new System.EventHandler(this.btnSelectDirectory_Click);
            // 
            // grpUploadProgress
            // 
            this.grpUploadProgress.Controls.Add(this.cmbDataSets);
            this.grpUploadProgress.Controls.Add(this.label2);
            this.grpUploadProgress.Controls.Add(this.lblProgress);
            this.grpUploadProgress.Controls.Add(this.prbUploadProgress);
            this.grpUploadProgress.Controls.Add(this.btnStartUploading);
            this.grpUploadProgress.Enabled = false;
            this.grpUploadProgress.Location = new System.Drawing.Point(13, 92);
            this.grpUploadProgress.Name = "grpUploadProgress";
            this.grpUploadProgress.Size = new System.Drawing.Size(1047, 230);
            this.grpUploadProgress.TabIndex = 1;
            this.grpUploadProgress.TabStop = false;
            this.grpUploadProgress.Text = "STEP 2: Uploading";
            // 
            // cmbDataSets
            // 
            this.cmbDataSets.FormattingEnabled = true;
            this.cmbDataSets.Location = new System.Drawing.Point(18, 153);
            this.cmbDataSets.Name = "cmbDataSets";
            this.cmbDataSets.Size = new System.Drawing.Size(259, 21);
            this.cmbDataSets.TabIndex = 7;
            this.cmbDataSets.SelectedIndexChanged += new System.EventHandler(this.cmbDataSets_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 136);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "DataSet to Upload To:";
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(15, 83);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 13);
            this.lblProgress.TabIndex = 5;
            // 
            // prbUploadProgress
            // 
            this.prbUploadProgress.Location = new System.Drawing.Point(18, 46);
            this.prbUploadProgress.Name = "prbUploadProgress";
            this.prbUploadProgress.Size = new System.Drawing.Size(1011, 23);
            this.prbUploadProgress.TabIndex = 4;
            // 
            // btnStartUploading
            // 
            this.btnStartUploading.Location = new System.Drawing.Point(305, 178);
            this.btnStartUploading.Name = "btnStartUploading";
            this.btnStartUploading.Size = new System.Drawing.Size(75, 23);
            this.btnStartUploading.TabIndex = 3;
            this.btnStartUploading.Text = "Upload";
            this.btnStartUploading.UseVisualStyleBackColor = true;
            this.btnStartUploading.Click += new System.EventHandler(this.btnStartUploading_Click);
            // 
            // FUploadDirectory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1072, 336);
            this.Controls.Add(this.grpUploadProgress);
            this.Controls.Add(this.groupBox1);
            this.Name = "FUploadDirectory";
            this.Text = "Uploading APKs from a Directory";
            this.Load += new System.EventHandler(this.FUploadDirectory_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpUploadProgress.ResumeLayout(false);
            this.grpUploadProgress.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSelectDirectory;
        private System.Windows.Forms.FolderBrowserDialog dlgDirectorySelectionDialog;
        private System.Windows.Forms.Label lblFoundObjects;
        private System.Windows.Forms.Label lblSelectedDirectory;
        private System.Windows.Forms.GroupBox grpUploadProgress;
        private System.Windows.Forms.Button btnStartUploading;
        private System.Windows.Forms.ProgressBar prbUploadProgress;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbDataSets;
    }
}