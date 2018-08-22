namespace APKInsight.Forms
{
    partial class FDecodeApk
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
            this.btnDecodeAPKs = new System.Windows.Forms.Button();
            this.prgProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.nudThreads = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTempDir = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtApkToolPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblFilesUploaded = new System.Windows.Forms.Label();
            this.lblThreadsCount = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.nudFileUploaderThreads = new System.Windows.Forms.NumericUpDown();
            this.lblRunningUploadingThreads = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.prgFilesUploading = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFileUploaderThreads)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDecodeAPKs
            // 
            this.btnDecodeAPKs.Location = new System.Drawing.Point(13, 75);
            this.btnDecodeAPKs.Name = "btnDecodeAPKs";
            this.btnDecodeAPKs.Size = new System.Drawing.Size(75, 23);
            this.btnDecodeAPKs.TabIndex = 0;
            this.btnDecodeAPKs.Text = "Process";
            this.btnDecodeAPKs.UseVisualStyleBackColor = true;
            this.btnDecodeAPKs.Click += new System.EventHandler(this.btnDecodeAPKs_Click);
            // 
            // prgProgressBar
            // 
            this.prgProgressBar.Location = new System.Drawing.Point(13, 13);
            this.prgProgressBar.Name = "prgProgressBar";
            this.prgProgressBar.Size = new System.Drawing.Size(808, 23);
            this.prgProgressBar.TabIndex = 1;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(106, 80);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(48, 13);
            this.lblProgress.TabIndex = 3;
            this.lblProgress.Text = "Progress";
            // 
            // nudThreads
            // 
            this.nudThreads.Location = new System.Drawing.Point(199, 126);
            this.nudThreads.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudThreads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudThreads.Name = "nudThreads";
            this.nudThreads.Size = new System.Drawing.Size(63, 20);
            this.nudThreads.TabIndex = 4;
            this.nudThreads.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(106, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(195, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Threads To Use (Decoding/Uploading):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(106, 169);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Temp Dir Path:";
            // 
            // txtTempDir
            // 
            this.txtTempDir.Location = new System.Drawing.Point(199, 166);
            this.txtTempDir.Name = "txtTempDir";
            this.txtTempDir.Size = new System.Drawing.Size(622, 20);
            this.txtTempDir.TabIndex = 7;
            this.txtTempDir.Text = "T:\\";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(106, 201);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "ApkTool Path:";
            // 
            // txtApkToolPath
            // 
            this.txtApkToolPath.Location = new System.Drawing.Point(199, 198);
            this.txtApkToolPath.Name = "txtApkToolPath";
            this.txtApkToolPath.Size = new System.Drawing.Size(622, 20);
            this.txtApkToolPath.TabIndex = 9;
            this.txtApkToolPath.Text = "S:\\csnowcode\\apkinsight\\scripts\\apktool.bat";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(441, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Total Files Uploaded:";
            // 
            // lblFilesUploaded
            // 
            this.lblFilesUploaded.AutoSize = true;
            this.lblFilesUploaded.Location = new System.Drawing.Point(554, 80);
            this.lblFilesUploaded.Name = "lblFilesUploaded";
            this.lblFilesUploaded.Size = new System.Drawing.Size(13, 13);
            this.lblFilesUploaded.TabIndex = 11;
            this.lblFilesUploaded.Text = "0";
            // 
            // lblThreadsCount
            // 
            this.lblThreadsCount.AutoSize = true;
            this.lblThreadsCount.Location = new System.Drawing.Point(554, 108);
            this.lblThreadsCount.Name = "lblThreadsCount";
            this.lblThreadsCount.Size = new System.Drawing.Size(13, 13);
            this.lblThreadsCount.TabIndex = 13;
            this.lblThreadsCount.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(363, 108);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(185, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Currently Running Decoding Threads:";
            // 
            // nudFileUploaderThreads
            // 
            this.nudFileUploaderThreads.Location = new System.Drawing.Point(268, 126);
            this.nudFileUploaderThreads.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudFileUploaderThreads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFileUploaderThreads.Name = "nudFileUploaderThreads";
            this.nudFileUploaderThreads.Size = new System.Drawing.Size(63, 20);
            this.nudFileUploaderThreads.TabIndex = 14;
            this.nudFileUploaderThreads.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // lblRunningUploadingThreads
            // 
            this.lblRunningUploadingThreads.AutoSize = true;
            this.lblRunningUploadingThreads.Location = new System.Drawing.Point(554, 135);
            this.lblRunningUploadingThreads.Name = "lblRunningUploadingThreads";
            this.lblRunningUploadingThreads.Size = new System.Drawing.Size(13, 13);
            this.lblRunningUploadingThreads.TabIndex = 16;
            this.lblRunningUploadingThreads.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(361, 135);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(187, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Currently Running Uploading Threads:";
            // 
            // prgFilesUploading
            // 
            this.prgFilesUploading.Location = new System.Drawing.Point(13, 42);
            this.prgFilesUploading.Name = "prgFilesUploading";
            this.prgFilesUploading.Size = new System.Drawing.Size(808, 23);
            this.prgFilesUploading.TabIndex = 17;
            // 
            // FDecodeApk
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 232);
            this.Controls.Add(this.prgFilesUploading);
            this.Controls.Add(this.lblRunningUploadingThreads);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.nudFileUploaderThreads);
            this.Controls.Add(this.lblThreadsCount);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblFilesUploaded);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtApkToolPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTempDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudThreads);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.prgProgressBar);
            this.Controls.Add(this.btnDecodeAPKs);
            this.Name = "FDecodeApk";
            this.Text = "Decoding APK Files";
            this.Load += new System.EventHandler(this.FDecodeApk_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudThreads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFileUploaderThreads)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDecodeAPKs;
        private System.Windows.Forms.ProgressBar prgProgressBar;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.NumericUpDown nudThreads;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTempDir;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtApkToolPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblFilesUploaded;
        private System.Windows.Forms.Label lblThreadsCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudFileUploaderThreads;
        private System.Windows.Forms.Label lblRunningUploadingThreads;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ProgressBar prgFilesUploading;
    }
}