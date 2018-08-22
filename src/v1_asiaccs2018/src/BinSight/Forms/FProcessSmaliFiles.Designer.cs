namespace APKInsight.Forms
{
    partial class FProcessSmaliFiles
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
            this.prbProgress = new System.Windows.Forms.ProgressBar();
            this.btnProcess = new System.Windows.Forms.Button();
            this.lblStats = new System.Windows.Forms.Label();
            this.lblStatValue = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblSpeedReport = new System.Windows.Forms.Label();
            this.prgLoadProgress = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblTimeElapsed = new System.Windows.Forms.Label();
            this.lblTimeRemaining = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // prbProgress
            // 
            this.prbProgress.Location = new System.Drawing.Point(8, 132);
            this.prbProgress.Name = "prbProgress";
            this.prbProgress.Size = new System.Drawing.Size(388, 22);
            this.prbProgress.TabIndex = 0;
            // 
            // btnProcess
            // 
            this.btnProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnProcess.Location = new System.Drawing.Point(7, 185);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(75, 23);
            this.btnProcess.TabIndex = 3;
            this.btnProcess.Text = "Process";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // lblStats
            // 
            this.lblStats.AutoSize = true;
            this.lblStats.Location = new System.Drawing.Point(6, 101);
            this.lblStats.Name = "lblStats";
            this.lblStats.Size = new System.Drawing.Size(34, 13);
            this.lblStats.TabIndex = 2;
            this.lblStats.Text = "Stats:";
            // 
            // lblStatValue
            // 
            this.lblStatValue.AutoSize = true;
            this.lblStatValue.Location = new System.Drawing.Point(46, 101);
            this.lblStatValue.Name = "lblStatValue";
            this.lblStatValue.Size = new System.Drawing.Size(0, 13);
            this.lblStatValue.TabIndex = 3;
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStop.Location = new System.Drawing.Point(88, 185);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 0;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblSpeedReport
            // 
            this.lblSpeedReport.AutoSize = true;
            this.lblSpeedReport.Location = new System.Drawing.Point(14, 27);
            this.lblSpeedReport.Name = "lblSpeedReport";
            this.lblSpeedReport.Size = new System.Drawing.Size(73, 13);
            this.lblSpeedReport.TabIndex = 5;
            this.lblSpeedReport.Text = "Speed Report";
            // 
            // prgLoadProgress
            // 
            this.prgLoadProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.prgLoadProgress.Enabled = false;
            this.prgLoadProgress.Location = new System.Drawing.Point(180, 185);
            this.prgLoadProgress.Name = "prgLoadProgress";
            this.prgLoadProgress.Size = new System.Drawing.Size(394, 23);
            this.prgLoadProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.prgLoadProgress.TabIndex = 6;
            this.prgLoadProgress.Value = 50;
            this.prgLoadProgress.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblTimeElapsed);
            this.groupBox1.Controls.Add(this.lblTimeRemaining);
            this.groupBox1.Controls.Add(this.lblSpeedReport);
            this.groupBox1.Controls.Add(this.lblStats);
            this.groupBox1.Controls.Add(this.prbProgress);
            this.groupBox1.Controls.Add(this.lblStatValue);
            this.groupBox1.Location = new System.Drawing.Point(7, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(763, 167);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Progress Report";
            // 
            // lblTimeElapsed
            // 
            this.lblTimeElapsed.AutoSize = true;
            this.lblTimeElapsed.Location = new System.Drawing.Point(16, 73);
            this.lblTimeElapsed.Name = "lblTimeElapsed";
            this.lblTimeElapsed.Size = new System.Drawing.Size(71, 13);
            this.lblTimeElapsed.TabIndex = 7;
            this.lblTimeElapsed.Text = "Time Elapsed";
            // 
            // lblTimeRemaining
            // 
            this.lblTimeRemaining.AutoSize = true;
            this.lblTimeRemaining.Location = new System.Drawing.Point(14, 49);
            this.lblTimeRemaining.Name = "lblTimeRemaining";
            this.lblTimeRemaining.Size = new System.Drawing.Size(83, 13);
            this.lblTimeRemaining.TabIndex = 6;
            this.lblTimeRemaining.Text = "Time Remaining";
            // 
            // FProcessSmaliFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 219);
            this.Controls.Add(this.prgLoadProgress);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnProcess);
            this.DoubleBuffered = true;
            this.Name = "FProcessSmaliFiles";
            this.Text = "Smali processing dialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FProcessSmaliFiles_FormClosing);
            this.Load += new System.EventHandler(this.FProcessSmaliFiles_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar prbProgress;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.Label lblStats;
        private System.Windows.Forms.Label lblStatValue;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblSpeedReport;
        private System.Windows.Forms.ProgressBar prgLoadProgress;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblTimeElapsed;
        private System.Windows.Forms.Label lblTimeRemaining;
    }
}