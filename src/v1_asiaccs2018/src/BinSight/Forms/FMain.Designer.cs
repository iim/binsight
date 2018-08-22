namespace APKInsight.Forms
{
    partial class FMain
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
            this.mnuMainMenu = new System.Windows.Forms.MenuStrip();
            this.binaryObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnUploadDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDecodeAPKs = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMenuProcessSmaliFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.trvAPKCategories = new System.Windows.Forms.TreeView();
            this.spcSplitContainer = new System.Windows.Forms.SplitContainer();
            this.lblApplicationCategories = new System.Windows.Forms.Label();
            this.stsStatusBar = new System.Windows.Forms.StatusStrip();
            this.lblMainStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.prbMainStatusProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblNumberOfApps = new System.Windows.Forms.ToolStripStatusLabel();
            this.tscMainTooltipContainer = new System.Windows.Forms.ToolStripContainer();
            this.tlsDataSetTools = new System.Windows.Forms.ToolStrip();
            this.cmbBarDataSet = new System.Windows.Forms.ToolStripComboBox();
            this.lblNumberOfAppsTotal = new System.Windows.Forms.ToolStripLabel();
            this.tlsApkProcessingTools = new System.Windows.Forms.ToolStrip();
            this.btnBarProcessSmaliFiles = new System.Windows.Forms.ToolStripButton();
            this.btnBarLibraries = new System.Windows.Forms.ToolStripButton();
            this.btnBarCompressDb = new System.Windows.Forms.ToolStripButton();
            this.uploadAndDecodeAPKsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcSplitContainer)).BeginInit();
            this.spcSplitContainer.Panel1.SuspendLayout();
            this.spcSplitContainer.SuspendLayout();
            this.stsStatusBar.SuspendLayout();
            this.tscMainTooltipContainer.BottomToolStripPanel.SuspendLayout();
            this.tscMainTooltipContainer.ContentPanel.SuspendLayout();
            this.tscMainTooltipContainer.TopToolStripPanel.SuspendLayout();
            this.tscMainTooltipContainer.SuspendLayout();
            this.tlsDataSetTools.SuspendLayout();
            this.tlsApkProcessingTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuMainMenu
            // 
            this.mnuMainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.binaryObjectsToolStripMenuItem});
            this.mnuMainMenu.Location = new System.Drawing.Point(0, 0);
            this.mnuMainMenu.Name = "mnuMainMenu";
            this.mnuMainMenu.Size = new System.Drawing.Size(663, 24);
            this.mnuMainMenu.TabIndex = 0;
            this.mnuMainMenu.Text = "menuStrip1";
            // 
            // binaryObjectsToolStripMenuItem
            // 
            this.binaryObjectsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnUploadDirectory,
            this.btnDecodeAPKs,
            this.btnMenuProcessSmaliFiles,
            this.btnRefresh,
            this.uploadAndDecodeAPKsToolStripMenuItem});
            this.binaryObjectsToolStripMenuItem.Name = "binaryObjectsToolStripMenuItem";
            this.binaryObjectsToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            this.binaryObjectsToolStripMenuItem.Text = "Binary Objects";
            // 
            // btnUploadDirectory
            // 
            this.btnUploadDirectory.Name = "btnUploadDirectory";
            this.btnUploadDirectory.Size = new System.Drawing.Size(226, 26);
            this.btnUploadDirectory.Text = "Upload Directory";
            this.btnUploadDirectory.Click += new System.EventHandler(this.btnUploadDirectory_Click);
            // 
            // btnDecodeAPKs
            // 
            this.btnDecodeAPKs.Name = "btnDecodeAPKs";
            this.btnDecodeAPKs.Size = new System.Drawing.Size(226, 26);
            this.btnDecodeAPKs.Text = "Decode APKs ...";
            this.btnDecodeAPKs.Click += new System.EventHandler(this.btnDecodeAPKs_Click);
            // 
            // btnMenuProcessSmaliFiles
            // 
            this.btnMenuProcessSmaliFiles.Image = global::APKInsight.Properties.Resources.gears;
            this.btnMenuProcessSmaliFiles.Name = "btnMenuProcessSmaliFiles";
            this.btnMenuProcessSmaliFiles.Size = new System.Drawing.Size(226, 26);
            this.btnMenuProcessSmaliFiles.Text = "Process Smali Files ...";
            this.btnMenuProcessSmaliFiles.Click += new System.EventHandler(this.btnMenuProcessSmaliFiles_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(226, 26);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // trvAPKCategories
            // 
            this.trvAPKCategories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trvAPKCategories.Location = new System.Drawing.Point(3, 23);
            this.trvAPKCategories.Name = "trvAPKCategories";
            this.trvAPKCategories.Size = new System.Drawing.Size(214, 400);
            this.trvAPKCategories.TabIndex = 1;
            this.trvAPKCategories.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.trvAPKCategories_NodeMouseDoubleClick);
            // 
            // spcSplitContainer
            // 
            this.spcSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.spcSplitContainer.Name = "spcSplitContainer";
            // 
            // spcSplitContainer.Panel1
            // 
            this.spcSplitContainer.Panel1.Controls.Add(this.lblApplicationCategories);
            this.spcSplitContainer.Panel1.Controls.Add(this.trvAPKCategories);
            this.spcSplitContainer.Size = new System.Drawing.Size(663, 426);
            this.spcSplitContainer.SplitterDistance = 220;
            this.spcSplitContainer.TabIndex = 2;
            // 
            // lblApplicationCategories
            // 
            this.lblApplicationCategories.AutoSize = true;
            this.lblApplicationCategories.Location = new System.Drawing.Point(4, 4);
            this.lblApplicationCategories.Name = "lblApplicationCategories";
            this.lblApplicationCategories.Size = new System.Drawing.Size(112, 13);
            this.lblApplicationCategories.TabIndex = 2;
            this.lblApplicationCategories.Text = "Application Categories";
            // 
            // stsStatusBar
            // 
            this.stsStatusBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.stsStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblMainStatusLabel,
            this.prbMainStatusProgressBar,
            this.lblNumberOfApps});
            this.stsStatusBar.Location = new System.Drawing.Point(0, 502);
            this.stsStatusBar.Name = "stsStatusBar";
            this.stsStatusBar.Size = new System.Drawing.Size(663, 22);
            this.stsStatusBar.TabIndex = 3;
            this.stsStatusBar.Text = "statusStrip1";
            // 
            // lblMainStatusLabel
            // 
            this.lblMainStatusLabel.Name = "lblMainStatusLabel";
            this.lblMainStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // prbMainStatusProgressBar
            // 
            this.prbMainStatusProgressBar.Name = "prbMainStatusProgressBar";
            this.prbMainStatusProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // lblNumberOfApps
            // 
            this.lblNumberOfApps.Name = "lblNumberOfApps";
            this.lblNumberOfApps.Size = new System.Drawing.Size(101, 17);
            this.lblNumberOfApps.Text = "Total: 0, Unique: 0";
            // 
            // tscMainTooltipContainer
            // 
            // 
            // tscMainTooltipContainer.BottomToolStripPanel
            // 
            this.tscMainTooltipContainer.BottomToolStripPanel.Controls.Add(this.tlsDataSetTools);
            // 
            // tscMainTooltipContainer.ContentPanel
            // 
            this.tscMainTooltipContainer.ContentPanel.Controls.Add(this.spcSplitContainer);
            this.tscMainTooltipContainer.ContentPanel.Size = new System.Drawing.Size(663, 426);
            this.tscMainTooltipContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tscMainTooltipContainer.Location = new System.Drawing.Point(0, 24);
            this.tscMainTooltipContainer.Name = "tscMainTooltipContainer";
            this.tscMainTooltipContainer.Size = new System.Drawing.Size(663, 478);
            this.tscMainTooltipContainer.TabIndex = 5;
            this.tscMainTooltipContainer.Text = "toolStripContainer1";
            // 
            // tscMainTooltipContainer.TopToolStripPanel
            // 
            this.tscMainTooltipContainer.TopToolStripPanel.Controls.Add(this.tlsApkProcessingTools);
            // 
            // tlsDataSetTools
            // 
            this.tlsDataSetTools.Dock = System.Windows.Forms.DockStyle.None;
            this.tlsDataSetTools.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tlsDataSetTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmbBarDataSet,
            this.lblNumberOfAppsTotal});
            this.tlsDataSetTools.Location = new System.Drawing.Point(5, 0);
            this.tlsDataSetTools.Name = "tlsDataSetTools";
            this.tlsDataSetTools.Size = new System.Drawing.Size(135, 25);
            this.tlsDataSetTools.TabIndex = 0;
            // 
            // cmbBarDataSet
            // 
            this.cmbBarDataSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBarDataSet.DropDownWidth = 350;
            this.cmbBarDataSet.Name = "cmbBarDataSet";
            this.cmbBarDataSet.Size = new System.Drawing.Size(121, 25);
            this.cmbBarDataSet.SelectedIndexChanged += new System.EventHandler(this.cmbBarDataSet_SelectedIndexChanged);
            // 
            // lblNumberOfAppsTotal
            // 
            this.lblNumberOfAppsTotal.Name = "lblNumberOfAppsTotal";
            this.lblNumberOfAppsTotal.Size = new System.Drawing.Size(0, 22);
            // 
            // tlsApkProcessingTools
            // 
            this.tlsApkProcessingTools.Dock = System.Windows.Forms.DockStyle.None;
            this.tlsApkProcessingTools.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tlsApkProcessingTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnBarProcessSmaliFiles,
            this.btnBarLibraries,
            this.btnBarCompressDb});
            this.tlsApkProcessingTools.Location = new System.Drawing.Point(3, 0);
            this.tlsApkProcessingTools.Name = "tlsApkProcessingTools";
            this.tlsApkProcessingTools.Size = new System.Drawing.Size(318, 27);
            this.tlsApkProcessingTools.TabIndex = 0;
            // 
            // btnBarProcessSmaliFiles
            // 
            this.btnBarProcessSmaliFiles.Image = global::APKInsight.Properties.Resources.gears;
            this.btnBarProcessSmaliFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBarProcessSmaliFiles.Name = "btnBarProcessSmaliFiles";
            this.btnBarProcessSmaliFiles.Size = new System.Drawing.Size(129, 24);
            this.btnBarProcessSmaliFiles.Text = "Process Smali Files";
            this.btnBarProcessSmaliFiles.Click += new System.EventHandler(this.btnBarProcessSmaliFiles_Click);
            // 
            // btnBarLibraries
            // 
            this.btnBarLibraries.Image = global::APKInsight.Properties.Resources.book;
            this.btnBarLibraries.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBarLibraries.Name = "btnBarLibraries";
            this.btnBarLibraries.Size = new System.Drawing.Size(75, 24);
            this.btnBarLibraries.Text = "Libraries";
            this.btnBarLibraries.Click += new System.EventHandler(this.btnBarLibraries_Click);
            // 
            // btnBarCompressDb
            // 
            this.btnBarCompressDb.Image = global::APKInsight.Properties.Resources.dna_1;
            this.btnBarCompressDb.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBarCompressDb.Name = "btnBarCompressDb";
            this.btnBarCompressDb.Size = new System.Drawing.Size(102, 24);
            this.btnBarCompressDb.Text = "Compress DB";
            this.btnBarCompressDb.Click += new System.EventHandler(this.btnBarCompressDb_Click);
            // 
            // uploadAndDecodeAPKsToolStripMenuItem
            // 
            this.uploadAndDecodeAPKsToolStripMenuItem.Name = "uploadAndDecodeAPKsToolStripMenuItem";
            this.uploadAndDecodeAPKsToolStripMenuItem.Size = new System.Drawing.Size(226, 26);
            this.uploadAndDecodeAPKsToolStripMenuItem.Text = "Upload And Decode APKs ...";
            this.uploadAndDecodeAPKsToolStripMenuItem.Click += new System.EventHandler(this.uploadAndDecodeAPKsToolStripMenuItem_Click);
            // 
            // FMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 524);
            this.Controls.Add(this.tscMainTooltipContainer);
            this.Controls.Add(this.stsStatusBar);
            this.Controls.Add(this.mnuMainMenu);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.mnuMainMenu;
            this.Name = "FMain";
            this.Text = "APK-Insight";
            this.Load += new System.EventHandler(this.FMain_Load);
            this.mnuMainMenu.ResumeLayout(false);
            this.mnuMainMenu.PerformLayout();
            this.spcSplitContainer.Panel1.ResumeLayout(false);
            this.spcSplitContainer.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcSplitContainer)).EndInit();
            this.spcSplitContainer.ResumeLayout(false);
            this.stsStatusBar.ResumeLayout(false);
            this.stsStatusBar.PerformLayout();
            this.tscMainTooltipContainer.BottomToolStripPanel.ResumeLayout(false);
            this.tscMainTooltipContainer.BottomToolStripPanel.PerformLayout();
            this.tscMainTooltipContainer.ContentPanel.ResumeLayout(false);
            this.tscMainTooltipContainer.TopToolStripPanel.ResumeLayout(false);
            this.tscMainTooltipContainer.TopToolStripPanel.PerformLayout();
            this.tscMainTooltipContainer.ResumeLayout(false);
            this.tscMainTooltipContainer.PerformLayout();
            this.tlsDataSetTools.ResumeLayout(false);
            this.tlsDataSetTools.PerformLayout();
            this.tlsApkProcessingTools.ResumeLayout(false);
            this.tlsApkProcessingTools.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuMainMenu;
        private System.Windows.Forms.ToolStripMenuItem binaryObjectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnUploadDirectory;
        private System.Windows.Forms.TreeView trvAPKCategories;
        private System.Windows.Forms.SplitContainer spcSplitContainer;
        private System.Windows.Forms.Label lblApplicationCategories;
        private System.Windows.Forms.ToolStripMenuItem btnRefresh;
        private System.Windows.Forms.ToolStripMenuItem btnDecodeAPKs;
        private System.Windows.Forms.ToolStripMenuItem btnMenuProcessSmaliFiles;
        private System.Windows.Forms.StatusStrip stsStatusBar;
        private System.Windows.Forms.ToolStripStatusLabel lblMainStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar prbMainStatusProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel lblNumberOfApps;
        private System.Windows.Forms.ToolStripContainer tscMainTooltipContainer;
        private System.Windows.Forms.ToolStrip tlsDataSetTools;
        private System.Windows.Forms.ToolStripComboBox cmbBarDataSet;
        private System.Windows.Forms.ToolStripLabel lblNumberOfAppsTotal;
        private System.Windows.Forms.ToolStrip tlsApkProcessingTools;
        private System.Windows.Forms.ToolStripButton btnBarProcessSmaliFiles;
        private System.Windows.Forms.ToolStripButton btnBarLibraries;
        private System.Windows.Forms.ToolStripButton btnBarCompressDb;
        private System.Windows.Forms.ToolStripMenuItem uploadAndDecodeAPKsToolStripMenuItem;
    }
}

