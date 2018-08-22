namespace APKInsight.Forms
{
    partial class FDecodeAndUploadApks
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
            this.grpDirectorySelection = new System.Windows.Forms.GroupBox();
            this.btnResetFilter = new System.Windows.Forms.Button();
            this.btnLoadFilter = new System.Windows.Forms.Button();
            this.lblFoundObjects = new System.Windows.Forms.Label();
            this.lblSelectedDirectory = new System.Windows.Forms.Label();
            this.btnSelectDirectory = new System.Windows.Forms.Button();
            this.grpDecodingProcess = new System.Windows.Forms.GroupBox();
            this.chkRule6StaticSeed = new System.Windows.Forms.CheckBox();
            this.chkRule5FewIterations = new System.Windows.Forms.CheckBox();
            this.chkNoStaticSalt = new System.Windows.Forms.CheckBox();
            this.chkNoStaticKeysForSymmetricCrypto = new System.Windows.Forms.CheckBox();
            this.chkCCS13Rule2NoStaticIv = new System.Windows.Forms.CheckBox();
            this.chkCCS13Rule1NoECB = new System.Windows.Forms.CheckBox();
            this.chkFindAllUseCases = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtReportPath = new System.Windows.Forms.TextBox();
            this.lblInterestingApkN = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.dlgDirectorySelectionDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.chkAnalyzeDataFlowAnalysis = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFileUploaderThreads)).BeginInit();
            this.grpDirectorySelection.SuspendLayout();
            this.grpDecodingProcess.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDecodeAPKs
            // 
            this.btnDecodeAPKs.Location = new System.Drawing.Point(6, 30);
            this.btnDecodeAPKs.Name = "btnDecodeAPKs";
            this.btnDecodeAPKs.Size = new System.Drawing.Size(75, 23);
            this.btnDecodeAPKs.TabIndex = 0;
            this.btnDecodeAPKs.Text = "Process";
            this.btnDecodeAPKs.UseVisualStyleBackColor = true;
            this.btnDecodeAPKs.Click += new System.EventHandler(this.btnDecodeAPKs_Click);
            // 
            // prgProgressBar
            // 
            this.prgProgressBar.Location = new System.Drawing.Point(115, 30);
            this.prgProgressBar.Name = "prgProgressBar";
            this.prgProgressBar.Size = new System.Drawing.Size(601, 23);
            this.prgProgressBar.TabIndex = 1;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(112, 117);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(48, 13);
            this.lblProgress.TabIndex = 3;
            this.lblProgress.Text = "Progress";
            // 
            // nudThreads
            // 
            this.nudThreads.Location = new System.Drawing.Point(205, 163);
            this.nudThreads.Maximum = new decimal(new int[] {
            64,
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
            this.label1.Location = new System.Drawing.Point(112, 147);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(195, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Threads To Use (Decoding/Uploading):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(112, 237);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Temp Dir Path:";
            // 
            // txtTempDir
            // 
            this.txtTempDir.Location = new System.Drawing.Point(205, 234);
            this.txtTempDir.Name = "txtTempDir";
            this.txtTempDir.Size = new System.Drawing.Size(511, 20);
            this.txtTempDir.TabIndex = 7;
            this.txtTempDir.Text = "T:\\";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(112, 263);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "ApkTool Path:";
            // 
            // txtApkToolPath
            // 
            this.txtApkToolPath.Location = new System.Drawing.Point(205, 260);
            this.txtApkToolPath.Name = "txtApkToolPath";
            this.txtApkToolPath.Size = new System.Drawing.Size(511, 20);
            this.txtApkToolPath.TabIndex = 9;
            this.txtApkToolPath.Text = "S:\\csnowcode\\apkinsight\\scripts\\apktool.bat";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(447, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Total Files Uploaded:";
            // 
            // lblFilesUploaded
            // 
            this.lblFilesUploaded.AutoSize = true;
            this.lblFilesUploaded.Location = new System.Drawing.Point(560, 117);
            this.lblFilesUploaded.Name = "lblFilesUploaded";
            this.lblFilesUploaded.Size = new System.Drawing.Size(13, 13);
            this.lblFilesUploaded.TabIndex = 11;
            this.lblFilesUploaded.Text = "0";
            // 
            // lblThreadsCount
            // 
            this.lblThreadsCount.AutoSize = true;
            this.lblThreadsCount.Location = new System.Drawing.Point(560, 145);
            this.lblThreadsCount.Name = "lblThreadsCount";
            this.lblThreadsCount.Size = new System.Drawing.Size(13, 13);
            this.lblThreadsCount.TabIndex = 13;
            this.lblThreadsCount.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(369, 145);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(185, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Currently Running Decoding Threads:";
            // 
            // nudFileUploaderThreads
            // 
            this.nudFileUploaderThreads.Location = new System.Drawing.Point(274, 163);
            this.nudFileUploaderThreads.Maximum = new decimal(new int[] {
            128,
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
            1,
            0,
            0,
            0});
            // 
            // lblRunningUploadingThreads
            // 
            this.lblRunningUploadingThreads.AutoSize = true;
            this.lblRunningUploadingThreads.Location = new System.Drawing.Point(560, 172);
            this.lblRunningUploadingThreads.Name = "lblRunningUploadingThreads";
            this.lblRunningUploadingThreads.Size = new System.Drawing.Size(13, 13);
            this.lblRunningUploadingThreads.TabIndex = 16;
            this.lblRunningUploadingThreads.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(367, 172);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(187, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Currently Running Uploading Threads:";
            // 
            // prgFilesUploading
            // 
            this.prgFilesUploading.Location = new System.Drawing.Point(115, 59);
            this.prgFilesUploading.Name = "prgFilesUploading";
            this.prgFilesUploading.Size = new System.Drawing.Size(601, 23);
            this.prgFilesUploading.TabIndex = 17;
            // 
            // grpDirectorySelection
            // 
            this.grpDirectorySelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDirectorySelection.Controls.Add(this.btnResetFilter);
            this.grpDirectorySelection.Controls.Add(this.btnLoadFilter);
            this.grpDirectorySelection.Controls.Add(this.lblFoundObjects);
            this.grpDirectorySelection.Controls.Add(this.lblSelectedDirectory);
            this.grpDirectorySelection.Controls.Add(this.btnSelectDirectory);
            this.grpDirectorySelection.Location = new System.Drawing.Point(12, 12);
            this.grpDirectorySelection.Name = "grpDirectorySelection";
            this.grpDirectorySelection.Size = new System.Drawing.Size(999, 71);
            this.grpDirectorySelection.TabIndex = 18;
            this.grpDirectorySelection.TabStop = false;
            this.grpDirectorySelection.Text = "STEP 1: Select Directory";
            // 
            // btnResetFilter
            // 
            this.btnResetFilter.Location = new System.Drawing.Point(452, 29);
            this.btnResetFilter.Name = "btnResetFilter";
            this.btnResetFilter.Size = new System.Drawing.Size(75, 23);
            this.btnResetFilter.TabIndex = 4;
            this.btnResetFilter.Text = "Reset Filter";
            this.btnResetFilter.UseVisualStyleBackColor = true;
            this.btnResetFilter.Click += new System.EventHandler(this.btnResetFilter_Click);
            // 
            // btnLoadFilter
            // 
            this.btnLoadFilter.Location = new System.Drawing.Point(371, 29);
            this.btnLoadFilter.Name = "btnLoadFilter";
            this.btnLoadFilter.Size = new System.Drawing.Size(75, 23);
            this.btnLoadFilter.TabIndex = 3;
            this.btnLoadFilter.Text = "Load Filter";
            this.btnLoadFilter.UseVisualStyleBackColor = true;
            this.btnLoadFilter.Click += new System.EventHandler(this.btnLoadFilter_Click);
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
            // grpDecodingProcess
            // 
            this.grpDecodingProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDecodingProcess.Controls.Add(this.chkAnalyzeDataFlowAnalysis);
            this.grpDecodingProcess.Controls.Add(this.chkRule6StaticSeed);
            this.grpDecodingProcess.Controls.Add(this.chkRule5FewIterations);
            this.grpDecodingProcess.Controls.Add(this.chkNoStaticSalt);
            this.grpDecodingProcess.Controls.Add(this.chkNoStaticKeysForSymmetricCrypto);
            this.grpDecodingProcess.Controls.Add(this.chkCCS13Rule2NoStaticIv);
            this.grpDecodingProcess.Controls.Add(this.chkCCS13Rule1NoECB);
            this.grpDecodingProcess.Controls.Add(this.chkFindAllUseCases);
            this.grpDecodingProcess.Controls.Add(this.label5);
            this.grpDecodingProcess.Controls.Add(this.txtReportPath);
            this.grpDecodingProcess.Controls.Add(this.lblInterestingApkN);
            this.grpDecodingProcess.Controls.Add(this.label8);
            this.grpDecodingProcess.Controls.Add(this.prgProgressBar);
            this.grpDecodingProcess.Controls.Add(this.btnDecodeAPKs);
            this.grpDecodingProcess.Controls.Add(this.prgFilesUploading);
            this.grpDecodingProcess.Controls.Add(this.lblProgress);
            this.grpDecodingProcess.Controls.Add(this.lblRunningUploadingThreads);
            this.grpDecodingProcess.Controls.Add(this.nudThreads);
            this.grpDecodingProcess.Controls.Add(this.label7);
            this.grpDecodingProcess.Controls.Add(this.label1);
            this.grpDecodingProcess.Controls.Add(this.nudFileUploaderThreads);
            this.grpDecodingProcess.Controls.Add(this.label2);
            this.grpDecodingProcess.Controls.Add(this.lblThreadsCount);
            this.grpDecodingProcess.Controls.Add(this.txtTempDir);
            this.grpDecodingProcess.Controls.Add(this.label6);
            this.grpDecodingProcess.Controls.Add(this.label3);
            this.grpDecodingProcess.Controls.Add(this.lblFilesUploaded);
            this.grpDecodingProcess.Controls.Add(this.txtApkToolPath);
            this.grpDecodingProcess.Controls.Add(this.label4);
            this.grpDecodingProcess.Location = new System.Drawing.Point(12, 151);
            this.grpDecodingProcess.Name = "grpDecodingProcess";
            this.grpDecodingProcess.Size = new System.Drawing.Size(999, 293);
            this.grpDecodingProcess.TabIndex = 19;
            this.grpDecodingProcess.TabStop = false;
            this.grpDecodingProcess.Text = "STEP 3: Decoding && Uploading";
            // 
            // chkRule6StaticSeed
            // 
            this.chkRule6StaticSeed.AutoSize = true;
            this.chkRule6StaticSeed.Location = new System.Drawing.Point(768, 168);
            this.chkRule6StaticSeed.Name = "chkRule6StaticSeed";
            this.chkRule6StaticSeed.Size = new System.Drawing.Size(151, 17);
            this.chkRule6StaticSeed.TabIndex = 28;
            this.chkRule6StaticSeed.Text = "Rule 6: Static Seed for SR";
            this.chkRule6StaticSeed.UseVisualStyleBackColor = true;
            // 
            // chkRule5FewIterations
            // 
            this.chkRule5FewIterations.AutoSize = true;
            this.chkRule5FewIterations.Location = new System.Drawing.Point(768, 145);
            this.chkRule5FewIterations.Name = "chkRule5FewIterations";
            this.chkRule5FewIterations.Size = new System.Drawing.Size(128, 17);
            this.chkRule5FewIterations.TabIndex = 27;
            this.chkRule5FewIterations.Text = "Rule 5: Few iterations";
            this.chkRule5FewIterations.UseVisualStyleBackColor = true;
            // 
            // chkNoStaticSalt
            // 
            this.chkNoStaticSalt.AutoSize = true;
            this.chkNoStaticSalt.Location = new System.Drawing.Point(768, 122);
            this.chkNoStaticSalt.Name = "chkNoStaticSalt";
            this.chkNoStaticSalt.Size = new System.Drawing.Size(126, 17);
            this.chkNoStaticSalt.TabIndex = 26;
            this.chkNoStaticSalt.Text = "Rule 4: No static Salt";
            this.chkNoStaticSalt.UseVisualStyleBackColor = true;
            // 
            // chkNoStaticKeysForSymmetricCrypto
            // 
            this.chkNoStaticKeysForSymmetricCrypto.AutoSize = true;
            this.chkNoStaticKeysForSymmetricCrypto.Location = new System.Drawing.Point(768, 99);
            this.chkNoStaticKeysForSymmetricCrypto.Name = "chkNoStaticKeysForSymmetricCrypto";
            this.chkNoStaticKeysForSymmetricCrypto.Size = new System.Drawing.Size(131, 17);
            this.chkNoStaticKeysForSymmetricCrypto.TabIndex = 25;
            this.chkNoStaticKeysForSymmetricCrypto.Text = "Rule 3: No static Keys";
            this.chkNoStaticKeysForSymmetricCrypto.UseVisualStyleBackColor = true;
            // 
            // chkCCS13Rule2NoStaticIv
            // 
            this.chkCCS13Rule2NoStaticIv.AutoSize = true;
            this.chkCCS13Rule2NoStaticIv.Location = new System.Drawing.Point(768, 76);
            this.chkCCS13Rule2NoStaticIv.Name = "chkCCS13Rule2NoStaticIv";
            this.chkCCS13Rule2NoStaticIv.Size = new System.Drawing.Size(157, 17);
            this.chkCCS13Rule2NoStaticIv.TabIndex = 24;
            this.chkCCS13Rule2NoStaticIv.Text = "Rule 2: No static IV for CBC";
            this.chkCCS13Rule2NoStaticIv.UseVisualStyleBackColor = true;
            // 
            // chkCCS13Rule1NoECB
            // 
            this.chkCCS13Rule1NoECB.AutoSize = true;
            this.chkCCS13Rule1NoECB.Location = new System.Drawing.Point(768, 53);
            this.chkCCS13Rule1NoECB.Name = "chkCCS13Rule1NoECB";
            this.chkCCS13Rule1NoECB.Size = new System.Drawing.Size(130, 17);
            this.chkCCS13Rule1NoECB.TabIndex = 23;
            this.chkCCS13Rule1NoECB.Text = "Rule 1: No ECB mode";
            this.chkCCS13Rule1NoECB.UseVisualStyleBackColor = true;
            // 
            // chkFindAllUseCases
            // 
            this.chkFindAllUseCases.AutoSize = true;
            this.chkFindAllUseCases.Location = new System.Drawing.Point(768, 30);
            this.chkFindAllUseCases.Name = "chkFindAllUseCases";
            this.chkFindAllUseCases.Size = new System.Drawing.Size(114, 17);
            this.chkFindAllUseCases.TabIndex = 22;
            this.chkFindAllUseCases.Text = "Find All Use-Cases";
            this.chkFindAllUseCases.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(112, 211);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Report Path:";
            // 
            // txtReportPath
            // 
            this.txtReportPath.Location = new System.Drawing.Point(205, 208);
            this.txtReportPath.Name = "txtReportPath";
            this.txtReportPath.Size = new System.Drawing.Size(511, 20);
            this.txtReportPath.TabIndex = 21;
            this.txtReportPath.Text = "d:\\";
            // 
            // lblInterestingApkN
            // 
            this.lblInterestingApkN.AutoSize = true;
            this.lblInterestingApkN.Location = new System.Drawing.Point(560, 94);
            this.lblInterestingApkN.Name = "lblInterestingApkN";
            this.lblInterestingApkN.Size = new System.Drawing.Size(24, 13);
            this.lblInterestingApkN.TabIndex = 19;
            this.lblInterestingApkN.Text = "0/0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(406, 94);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(148, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Total Interesting/Failed APKs:";
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.Filter = "Excel CSV|*.csv";
            // 
            // chkAnalyzeDataFlowAnalysis
            // 
            this.chkAnalyzeDataFlowAnalysis.AutoSize = true;
            this.chkAnalyzeDataFlowAnalysis.Location = new System.Drawing.Point(768, 191);
            this.chkAnalyzeDataFlowAnalysis.Name = "chkAnalyzeDataFlowAnalysis";
            this.chkAnalyzeDataFlowAnalysis.Size = new System.Drawing.Size(162, 17);
            this.chkAnalyzeDataFlowAnalysis.TabIndex = 29;
            this.chkAnalyzeDataFlowAnalysis.Text = "Analyze Data Flow for Cipher";
            this.chkAnalyzeDataFlowAnalysis.UseVisualStyleBackColor = true;
            // 
            // FDecodeAndUploadApks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 456);
            this.Controls.Add(this.grpDecodingProcess);
            this.Controls.Add(this.grpDirectorySelection);
            this.Name = "FDecodeAndUploadApks";
            this.Text = "Decode And Upload APK Files";
            this.Load += new System.EventHandler(this.FDecodeApk_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudThreads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFileUploaderThreads)).EndInit();
            this.grpDirectorySelection.ResumeLayout(false);
            this.grpDirectorySelection.PerformLayout();
            this.grpDecodingProcess.ResumeLayout(false);
            this.grpDecodingProcess.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.GroupBox grpDirectorySelection;
        private System.Windows.Forms.Label lblFoundObjects;
        private System.Windows.Forms.Label lblSelectedDirectory;
        private System.Windows.Forms.Button btnSelectDirectory;
        private System.Windows.Forms.GroupBox grpDecodingProcess;
        private System.Windows.Forms.FolderBrowserDialog dlgDirectorySelectionDialog;
        private System.Windows.Forms.Label lblInterestingApkN;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtReportPath;
        private System.Windows.Forms.CheckBox chkFindAllUseCases;
        private System.Windows.Forms.CheckBox chkCCS13Rule1NoECB;
        private System.Windows.Forms.Button btnLoadFilter;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.Button btnResetFilter;
        private System.Windows.Forms.CheckBox chkCCS13Rule2NoStaticIv;
        private System.Windows.Forms.CheckBox chkNoStaticKeysForSymmetricCrypto;
        private System.Windows.Forms.CheckBox chkNoStaticSalt;
        private System.Windows.Forms.CheckBox chkRule5FewIterations;
        private System.Windows.Forms.CheckBox chkRule6StaticSeed;
        private System.Windows.Forms.CheckBox chkAnalyzeDataFlowAnalysis;
    }
}