namespace APKInsight.Forms
{
    partial class FLibraries
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FLibraries));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnAddLibrary = new System.Windows.Forms.ToolStripButton();
            this.grvLibraries = new System.Windows.Forms.DataGridView();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grvLibraries)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddLibrary});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(760, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnAddLibrary
            // 
            this.btnAddLibrary.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddLibrary.Image = ((System.Drawing.Image)(resources.GetObject("btnAddLibrary.Image")));
            this.btnAddLibrary.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddLibrary.Name = "btnAddLibrary";
            this.btnAddLibrary.Size = new System.Drawing.Size(72, 22);
            this.btnAddLibrary.Text = "Add Library";
            this.btnAddLibrary.Click += new System.EventHandler(this.btnAddLibrary_Click);
            // 
            // grvLibraries
            // 
            this.grvLibraries.AllowUserToAddRows = false;
            this.grvLibraries.AllowUserToDeleteRows = false;
            this.grvLibraries.AllowUserToOrderColumns = true;
            this.grvLibraries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grvLibraries.Location = new System.Drawing.Point(12, 74);
            this.grvLibraries.MultiSelect = false;
            this.grvLibraries.Name = "grvLibraries";
            this.grvLibraries.ReadOnly = true;
            this.grvLibraries.Size = new System.Drawing.Size(504, 438);
            this.grvLibraries.TabIndex = 1;
            this.grvLibraries.DoubleClick += new System.EventHandler(this.grvLibraries_DoubleClick);
            // 
            // FLibraries
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 524);
            this.Controls.Add(this.grvLibraries);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FLibraries";
            this.Text = "BINsight Libraries";
            this.Load += new System.EventHandler(this.FLibraries_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grvLibraries)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnAddLibrary;
        private System.Windows.Forms.DataGridView grvLibraries;
    }
}