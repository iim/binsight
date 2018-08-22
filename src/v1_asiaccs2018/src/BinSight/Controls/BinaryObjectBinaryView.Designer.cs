namespace APKInsight.Controls
{
    partial class BinaryObjectBinaryView
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
            this.spcSplitContainer = new System.Windows.Forms.SplitContainer();
            this.trvTreeView = new System.Windows.Forms.TreeView();
            this.lstDirectoryInternals = new System.Windows.Forms.ListView();
            ((System.ComponentModel.ISupportInitialize)(this.spcSplitContainer)).BeginInit();
            this.spcSplitContainer.Panel1.SuspendLayout();
            this.spcSplitContainer.Panel2.SuspendLayout();
            this.spcSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // spcSplitContainer
            // 
            this.spcSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.spcSplitContainer.Name = "spcSplitContainer";
            // 
            // spcSplitContainer.Panel1
            // 
            this.spcSplitContainer.Panel1.Controls.Add(this.trvTreeView);
            // 
            // spcSplitContainer.Panel2
            // 
            this.spcSplitContainer.Panel2.Controls.Add(this.lstDirectoryInternals);
            this.spcSplitContainer.Size = new System.Drawing.Size(831, 669);
            this.spcSplitContainer.SplitterDistance = 329;
            this.spcSplitContainer.TabIndex = 0;
            // 
            // trvTreeView
            // 
            this.trvTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvTreeView.Location = new System.Drawing.Point(0, 0);
            this.trvTreeView.Name = "trvTreeView";
            this.trvTreeView.Size = new System.Drawing.Size(329, 669);
            this.trvTreeView.TabIndex = 0;
            this.trvTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.trvTreeView_NodeMouseClick);
            // 
            // lstDirectoryInternals
            // 
            this.lstDirectoryInternals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstDirectoryInternals.Location = new System.Drawing.Point(0, 0);
            this.lstDirectoryInternals.Name = "lstDirectoryInternals";
            this.lstDirectoryInternals.Size = new System.Drawing.Size(498, 669);
            this.lstDirectoryInternals.TabIndex = 0;
            this.lstDirectoryInternals.UseCompatibleStateImageBehavior = false;
            this.lstDirectoryInternals.View = System.Windows.Forms.View.List;
            // 
            // BinaryObjectBinaryView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.spcSplitContainer);
            this.Name = "BinaryObjectBinaryView";
            this.Size = new System.Drawing.Size(831, 669);
            this.spcSplitContainer.Panel1.ResumeLayout(false);
            this.spcSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcSplitContainer)).EndInit();
            this.spcSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer spcSplitContainer;
        private System.Windows.Forms.TreeView trvTreeView;
        private System.Windows.Forms.ListView lstDirectoryInternals;
    }
}
