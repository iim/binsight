namespace APKInsight.Controls
{
    partial class BinaryObjectSmaliView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BinaryObjectSmaliView));
            this.spcSplitContainer = new System.Windows.Forms.SplitContainer();
            this.trvTreeView = new System.Windows.Forms.TreeView();
            this._treeViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.btnOpenInForm = new System.Windows.Forms.Button();
            this.tbcSmaliViews = new System.Windows.Forms.TabControl();
            this.tbpMainPage = new System.Windows.Forms.TabPage();
            this.smlvMainView = new APKInsight.Controls.SmaliView();
            this.btnShowGraph = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.spcSplitContainer)).BeginInit();
            this.spcSplitContainer.Panel1.SuspendLayout();
            this.spcSplitContainer.Panel2.SuspendLayout();
            this.spcSplitContainer.SuspendLayout();
            this.tbcSmaliViews.SuspendLayout();
            this.tbpMainPage.SuspendLayout();
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
            this.spcSplitContainer.Panel2.Controls.Add(this.btnShowGraph);
            this.spcSplitContainer.Panel2.Controls.Add(this.btnOpenInForm);
            this.spcSplitContainer.Panel2.Controls.Add(this.tbcSmaliViews);
            this.spcSplitContainer.Size = new System.Drawing.Size(694, 651);
            this.spcSplitContainer.SplitterDistance = 228;
            this.spcSplitContainer.TabIndex = 0;
            // 
            // trvTreeView
            // 
            this.trvTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvTreeView.ImageIndex = 0;
            this.trvTreeView.ImageList = this._treeViewImageList;
            this.trvTreeView.Location = new System.Drawing.Point(0, 0);
            this.trvTreeView.Name = "trvTreeView";
            this.trvTreeView.SelectedImageIndex = 0;
            this.trvTreeView.Size = new System.Drawing.Size(228, 651);
            this.trvTreeView.TabIndex = 0;
            this.trvTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvTreeView_AfterSelect);
            // 
            // _treeViewImageList
            // 
            this._treeViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_treeViewImageList.ImageStream")));
            this._treeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            this._treeViewImageList.Images.SetKeyName(0, "lg.ico");
            this._treeViewImageList.Images.SetKeyName(1, "pink.ico");
            this._treeViewImageList.Images.SetKeyName(2, "gold.ico");
            this._treeViewImageList.Images.SetKeyName(3, "red.ico");
            this._treeViewImageList.Images.SetKeyName(4, "java.png");
            this._treeViewImageList.Images.SetKeyName(5, "box.png");
            this._treeViewImageList.Images.SetKeyName(6, "empty.png");
            // 
            // btnOpenInForm
            // 
            this.btnOpenInForm.Location = new System.Drawing.Point(7, 3);
            this.btnOpenInForm.Name = "btnOpenInForm";
            this.btnOpenInForm.Size = new System.Drawing.Size(75, 23);
            this.btnOpenInForm.TabIndex = 2;
            this.btnOpenInForm.Text = "In Form";
            this.btnOpenInForm.UseVisualStyleBackColor = true;
            this.btnOpenInForm.Click += new System.EventHandler(this.btnOpenInForm_Click);
            // 
            // tbcSmaliViews
            // 
            this.tbcSmaliViews.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbcSmaliViews.Controls.Add(this.tbpMainPage);
            this.tbcSmaliViews.HotTrack = true;
            this.tbcSmaliViews.Location = new System.Drawing.Point(0, 36);
            this.tbcSmaliViews.Multiline = true;
            this.tbcSmaliViews.Name = "tbcSmaliViews";
            this.tbcSmaliViews.SelectedIndex = 0;
            this.tbcSmaliViews.Size = new System.Drawing.Size(462, 615);
            this.tbcSmaliViews.TabIndex = 1;
            this.tbcSmaliViews.SelectedIndexChanged += new System.EventHandler(this.tbcSmaliViews_SelectedIndexChanged);
            // 
            // tbpMainPage
            // 
            this.tbpMainPage.Controls.Add(this.smlvMainView);
            this.tbpMainPage.Location = new System.Drawing.Point(4, 22);
            this.tbpMainPage.Name = "tbpMainPage";
            this.tbpMainPage.Padding = new System.Windows.Forms.Padding(3);
            this.tbpMainPage.Size = new System.Drawing.Size(454, 589);
            this.tbpMainPage.TabIndex = 0;
            this.tbpMainPage.Text = "Main Tab";
            this.tbpMainPage.UseVisualStyleBackColor = true;
            // 
            // smlvMainView
            // 
            this.smlvMainView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.smlvMainView.Location = new System.Drawing.Point(3, 3);
            this.smlvMainView.Name = "smlvMainView";
            this.smlvMainView.Size = new System.Drawing.Size(448, 583);
            this.smlvMainView.TabIndex = 0;
            this.smlvMainView.OnInvokeLineClicked += new APKInsight.Controls.SmaliViewEventHandler(this.SmaliView_OnInvokeLineClicked);
            this.smlvMainView.OnMethodDefinitionLineClicked += new APKInsight.Controls.SmaliViewEventHandler(this.SmaliView_OnMethodDefinitionLineClicked);
            // 
            // btnShowGraph
            // 
            this.btnShowGraph.Location = new System.Drawing.Point(89, 4);
            this.btnShowGraph.Name = "btnShowGraph";
            this.btnShowGraph.Size = new System.Drawing.Size(75, 23);
            this.btnShowGraph.TabIndex = 3;
            this.btnShowGraph.Text = "View CFG";
            this.btnShowGraph.UseVisualStyleBackColor = true;
            this.btnShowGraph.Click += new System.EventHandler(this.btnShowGraph_Click);
            // 
            // BinaryObjectSmaliView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.spcSplitContainer);
            this.Name = "BinaryObjectSmaliView";
            this.Size = new System.Drawing.Size(694, 651);
            this.Load += new System.EventHandler(this.BinaryObjectSmaliView_Load);
            this.spcSplitContainer.Panel1.ResumeLayout(false);
            this.spcSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcSplitContainer)).EndInit();
            this.spcSplitContainer.ResumeLayout(false);
            this.tbcSmaliViews.ResumeLayout(false);
            this.tbpMainPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer spcSplitContainer;
        private System.Windows.Forms.TreeView trvTreeView;
        private System.Windows.Forms.ImageList _treeViewImageList;
        private System.Windows.Forms.TabControl tbcSmaliViews;
        private System.Windows.Forms.TabPage tbpMainPage;
        private SmaliView smlvMainView;
        private System.Windows.Forms.Button btnOpenInForm;
        private System.Windows.Forms.Button btnShowGraph;
    }
}
