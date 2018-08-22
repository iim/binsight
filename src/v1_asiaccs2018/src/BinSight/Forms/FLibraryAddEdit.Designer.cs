namespace APKInsight.Forms
{
    partial class FLibraryAddEdit
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FLibraryAddEdit));
            this.chkNotInLibraryList = new System.Windows.Forms.CheckedListBox();
            this.chkInLibraryList = new System.Windows.Forms.CheckedListBox();
            this.btnAddToLibrary = new System.Windows.Forms.Button();
            this.spcMainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.btnRemoveFromLibrary = new System.Windows.Forms.Button();
            this.btnSelectAllNotInLibrary = new System.Windows.Forms.Button();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.grpLibraryDetails = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPackageName = new System.Windows.Forms.TextBox();
            this.chkLibraryProperties = new System.Windows.Forms.CheckedListBox();
            this.grpLibraryProperties = new System.Windows.Forms.GroupBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.spcMainSplitContainer)).BeginInit();
            this.spcMainSplitContainer.Panel1.SuspendLayout();
            this.spcMainSplitContainer.Panel2.SuspendLayout();
            this.spcMainSplitContainer.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.grpLibraryDetails.SuspendLayout();
            this.grpLibraryProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkNotInLibraryList
            // 
            this.chkNotInLibraryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkNotInLibraryList.FormattingEnabled = true;
            this.chkNotInLibraryList.Location = new System.Drawing.Point(0, 0);
            this.chkNotInLibraryList.Name = "chkNotInLibraryList";
            this.chkNotInLibraryList.Size = new System.Drawing.Size(324, 414);
            this.chkNotInLibraryList.TabIndex = 0;
            // 
            // chkInLibraryList
            // 
            this.chkInLibraryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkInLibraryList.FormattingEnabled = true;
            this.chkInLibraryList.Location = new System.Drawing.Point(0, 0);
            this.chkInLibraryList.Name = "chkInLibraryList";
            this.chkInLibraryList.Size = new System.Drawing.Size(330, 414);
            this.chkInLibraryList.TabIndex = 1;
            // 
            // btnAddToLibrary
            // 
            this.btnAddToLibrary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddToLibrary.Location = new System.Drawing.Point(93, 696);
            this.btnAddToLibrary.Name = "btnAddToLibrary";
            this.btnAddToLibrary.Size = new System.Drawing.Size(75, 23);
            this.btnAddToLibrary.TabIndex = 2;
            this.btnAddToLibrary.Text = "Add";
            this.btnAddToLibrary.UseVisualStyleBackColor = true;
            this.btnAddToLibrary.Click += new System.EventHandler(this.btnAddToLibrary_Click);
            // 
            // spcMainSplitContainer
            // 
            this.spcMainSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spcMainSplitContainer.Location = new System.Drawing.Point(12, 276);
            this.spcMainSplitContainer.Name = "spcMainSplitContainer";
            // 
            // spcMainSplitContainer.Panel1
            // 
            this.spcMainSplitContainer.Panel1.Controls.Add(this.chkNotInLibraryList);
            // 
            // spcMainSplitContainer.Panel2
            // 
            this.spcMainSplitContainer.Panel2.Controls.Add(this.chkInLibraryList);
            this.spcMainSplitContainer.Size = new System.Drawing.Size(658, 414);
            this.spcMainSplitContainer.SplitterDistance = 324;
            this.spcMainSplitContainer.TabIndex = 3;
            // 
            // btnRemoveFromLibrary
            // 
            this.btnRemoveFromLibrary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveFromLibrary.Location = new System.Drawing.Point(340, 696);
            this.btnRemoveFromLibrary.Name = "btnRemoveFromLibrary";
            this.btnRemoveFromLibrary.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveFromLibrary.TabIndex = 4;
            this.btnRemoveFromLibrary.Text = "Remove";
            this.btnRemoveFromLibrary.UseVisualStyleBackColor = true;
            this.btnRemoveFromLibrary.Click += new System.EventHandler(this.btnRemoveFromLibrary_Click);
            // 
            // btnSelectAllNotInLibrary
            // 
            this.btnSelectAllNotInLibrary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectAllNotInLibrary.Location = new System.Drawing.Point(12, 696);
            this.btnSelectAllNotInLibrary.Name = "btnSelectAllNotInLibrary";
            this.btnSelectAllNotInLibrary.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAllNotInLibrary.TabIndex = 5;
            this.btnSelectAllNotInLibrary.Text = "Select All";
            this.btnSelectAllNotInLibrary.UseVisualStyleBackColor = true;
            this.btnSelectAllNotInLibrary.Click += new System.EventHandler(this.btnSelectAllNotInLibrary_Click);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(6, 26);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(63, 13);
            this.lblName.TabIndex = 6;
            this.lblName.Text = "SmaliName:";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(137, 23);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(515, 20);
            this.txtName.TabIndex = 7;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1064, 25);
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(35, 22);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtUrl
            // 
            this.txtUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUrl.Location = new System.Drawing.Point(137, 49);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(515, 20);
            this.txtUrl.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "URL:";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(137, 78);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(515, 71);
            this.txtDescription.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Description:";
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.Location = new System.Drawing.Point(12, 247);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(658, 20);
            this.txtFilter.TabIndex = 14;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 231);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Filter:";
            // 
            // grpLibraryDetails
            // 
            this.grpLibraryDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLibraryDetails.Controls.Add(this.label4);
            this.grpLibraryDetails.Controls.Add(this.txtPackageName);
            this.grpLibraryDetails.Controls.Add(this.lblName);
            this.grpLibraryDetails.Controls.Add(this.txtName);
            this.grpLibraryDetails.Controls.Add(this.label1);
            this.grpLibraryDetails.Controls.Add(this.label2);
            this.grpLibraryDetails.Controls.Add(this.txtUrl);
            this.grpLibraryDetails.Controls.Add(this.txtDescription);
            this.grpLibraryDetails.Location = new System.Drawing.Point(12, 30);
            this.grpLibraryDetails.Name = "grpLibraryDetails";
            this.grpLibraryDetails.Size = new System.Drawing.Size(658, 195);
            this.grpLibraryDetails.TabIndex = 15;
            this.grpLibraryDetails.TabStop = false;
            this.grpLibraryDetails.Text = "Library Details";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 158);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Base Package SmaliName:";
            // 
            // txtPackageName
            // 
            this.txtPackageName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPackageName.Location = new System.Drawing.Point(137, 155);
            this.txtPackageName.Name = "txtPackageName";
            this.txtPackageName.Size = new System.Drawing.Size(515, 20);
            this.txtPackageName.TabIndex = 14;
            // 
            // chkLibraryProperties
            // 
            this.chkLibraryProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkLibraryProperties.FormattingEnabled = true;
            this.chkLibraryProperties.Location = new System.Drawing.Point(3, 16);
            this.chkLibraryProperties.Name = "chkLibraryProperties";
            this.chkLibraryProperties.Size = new System.Drawing.Size(370, 670);
            this.chkLibraryProperties.TabIndex = 16;
            // 
            // grpLibraryProperties
            // 
            this.grpLibraryProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLibraryProperties.Controls.Add(this.chkLibraryProperties);
            this.grpLibraryProperties.Location = new System.Drawing.Point(676, 30);
            this.grpLibraryProperties.Name = "grpLibraryProperties";
            this.grpLibraryProperties.Size = new System.Drawing.Size(376, 689);
            this.grpLibraryProperties.TabIndex = 17;
            this.grpLibraryProperties.TabStop = false;
            this.grpLibraryProperties.Text = "Library Properties";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 26);
            // 
            // FLibraryAddEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 731);
            this.Controls.Add(this.grpLibraryProperties);
            this.Controls.Add(this.grpLibraryDetails);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btnSelectAllNotInLibrary);
            this.Controls.Add(this.btnRemoveFromLibrary);
            this.Controls.Add(this.spcMainSplitContainer);
            this.Controls.Add(this.btnAddToLibrary);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FLibraryAddEdit";
            this.Text = "New Library Definition";
            this.Load += new System.EventHandler(this.FLibraryAdd_Load);
            this.spcMainSplitContainer.Panel1.ResumeLayout(false);
            this.spcMainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcMainSplitContainer)).EndInit();
            this.spcMainSplitContainer.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.grpLibraryDetails.ResumeLayout(false);
            this.grpLibraryDetails.PerformLayout();
            this.grpLibraryProperties.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox chkNotInLibraryList;
        private System.Windows.Forms.CheckedListBox chkInLibraryList;
        private System.Windows.Forms.Button btnAddToLibrary;
        private System.Windows.Forms.SplitContainer spcMainSplitContainer;
        private System.Windows.Forms.Button btnRemoveFromLibrary;
        private System.Windows.Forms.Button btnSelectAllNotInLibrary;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox grpLibraryDetails;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPackageName;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.CheckedListBox chkLibraryProperties;
        private System.Windows.Forms.GroupBox grpLibraryProperties;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}