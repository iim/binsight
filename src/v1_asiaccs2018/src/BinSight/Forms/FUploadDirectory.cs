using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using APKInsight.Logic;
using APKInsight.Models.DataBase;
using APKInsight.Queries;
using CsnowFramework.InputOutput;

namespace APKInsight.Forms
{
    public partial class FUploadDirectory : Form
    {
        List<string> _files;
        private int _failedUploads;

        public FUploadDirectory()
        {
            InitializeComponent();
        }

        private void FUploadDirectory_Load(object sender, EventArgs e)
        {
            LoadDataSetCombo();
            SetUploadButtonEnabledState();
        }

        private void LoadDataSetCombo()
        {
            try
            {
                var query = new QueryDataSet();
                var datasets = query.SelectDataSets();
                cmbDataSets.Items.Clear();
                foreach (var dataset in datasets)
                {
                    cmbDataSets.Items.Add(dataset);
                }
            }
            catch (Exception)
            {
                MessageBox.Show($"Failed to Load DataSets", $"DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetUploadButtonEnabledState()
        {
            btnStartUploading.Enabled = cmbDataSets.SelectedIndex >= 0;
        }

        private void btnSelectDirectory_Click(object sender, EventArgs e)
        {
            if (dlgDirectorySelectionDialog.ShowDialog() == DialogResult.OK)
            {
                btnSelectDirectory.Enabled = false;
                lblSelectedDirectory.Text = dlgDirectorySelectionDialog.SelectedPath;
                _files = Utilities.GetChildFiles(dlgDirectorySelectionDialog.SelectedPath, ".apk");
                lblFoundObjects.Text = _files.Count.ToString();
                grpUploadProgress.Enabled = _files.Count > 0;
                btnSelectDirectory.Enabled = true;
            }
        }

        private void btnStartUploading_Click(object sender, EventArgs e)
        {
            btnStartUploading.Enabled = false;
            UploadApk uploadLogic = new UploadApk((cmbDataSets.SelectedItem as DataSet).UId.Value);

            prbUploadProgress.Minimum = 0;
            prbUploadProgress.Value = 0;
            prbUploadProgress.Maximum = _files.Count;
            lblProgress.Text = $"{prbUploadProgress.Value}/{prbUploadProgress.Maximum}";
            _failedUploads = 0;
            foreach (string filename in _files)
            {
                if (uploadLogic.UploadApkFile(filename) == null)
                    _failedUploads++;
                prbUploadProgress.Value++;
                lblProgress.Text = $"{prbUploadProgress.Value}/{prbUploadProgress.Maximum}, Failed = {_failedUploads} ({_failedUploads*100.0D/prbUploadProgress.Value}%)";

                Application.DoEvents();
                //Thread.Sleep(200);
            }
            var query = new QueryDataSet();
            query.UpdateBinaryCountInDataSetCategory((cmbDataSets.SelectedItem as DataSet).UId.Value);
            query.UpdateBinaryCountInDataSet((cmbDataSets.SelectedItem as DataSet).UId.Value);
        }

        private void cmbDataSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetUploadButtonEnabledState();
        }
    }
}
