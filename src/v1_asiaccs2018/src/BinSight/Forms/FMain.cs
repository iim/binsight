using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using APKInsight.Globals;
using APKInsight.Models;
using APKInsight.Models.Custom;
using APKInsight.Models.DataBase;
using APKInsight.Queries;

namespace APKInsight.Forms
{
    /// <summary>
    /// The main dialog of the BINSight application
    /// </summary>
    public partial class FMain : Form
    {
        private readonly object _addApkLock = new object();
        private DataSet _currentDataSet = null;
        private Dictionary<int, FBinaryDetails> _binaryDetailsDialogs = new Dictionary<int, FBinaryDetails>();

        #region Constructor

        public FMain()
        {
            InitializeComponent();
        }

        #endregion


        #region Form loading

        private void FMain_Load(object sender, EventArgs e)
        {
            Text = @"APK Insight Application: Version 0.16.3 (01/11/2016)";
            LoadDataSetCombo();
            SetEnableControlsOnToolBars(false);
        }

        #endregion

        private void btnUploadDirectory_Click(object sender, EventArgs e)
        {
            FUploadDirectory uploadForm = new FUploadDirectory();
            uploadForm.ShowDialog();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCategoriesAndApps();  
        }

        private void btnDecodeAPKs_Click(object sender, EventArgs e)
        {
            FDecodeApk decodeDialog = new FDecodeApk();
            decodeDialog.ShowDialog();
        }

        private void btnMenuProcessSmaliFiles_Click(object sender, EventArgs e)
        {
            OpenDialogForSmaliProcessing();
        }

        private void OpenDialogForSmaliProcessing()
        {
            var processSmaliFiles = new FProcessSmaliFiles(_currentDataSet);
            processSmaliFiles.ShowDialog();
        }

        private void LoadDataSetCombo()
        {
            try
            {
                var query = new QueryDataSet();
                var datasets = query.SelectDataSets();
                cmbBarDataSet.Items.Clear();
                foreach (var dataset in datasets)
                {
                    cmbBarDataSet.Items.Add(dataset);
                }
            }
            catch (Exception)
            {
                MessageBox.Show($"Failed to Load DataSets", $"DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCategoriesAndApps()
        {
            trvAPKCategories.Nodes.Clear();
            QueryApplicationCategories apcQuery = new QueryApplicationCategories();
            var categories = apcQuery.SelectAllApplicationCategories(_currentDataSet.UId.Value);
            foreach (var cat in categories)
            {
                TreeNode node = new TreeNode($"{cat.Name} ({cat.BioCount})");
                node.Tag = cat;
                trvAPKCategories.Nodes.Add(node);
                node.NodeFont = new Font(trvAPKCategories.Font, FontStyle.Italic);
            }
        }

        private void UpdateMainStatusText(string message)
        {
            if (InvokeRequired)
            {
                Invoke( new Action(() =>
                {
                    UpdateMainStatusText(message); 
                    
                }));
            }
            else
            {
                lblMainStatusLabel.Text = message;
            }
        }

        private void HideMainStatusProgress(TreeNode node)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    HideMainStatusProgress(node);

                }));
            }
            else
            {
                lblMainStatusLabel.Visible = false;
                lblMainStatusLabel.Text = "";
                prbMainStatusProgressBar.Visible = false;
                prbMainStatusProgressBar.Value = 0;
                (node.Tag as ApplicationCategoryWithCount).IsLoaded = true;
                node.Expand();
                node.NodeFont = new Font(node.NodeFont, FontStyle.Regular);

            }
        }

        private void AddApkToCategory(TreeNode node, BinaryObject bio)
        {

            if (InvokeRequired)
            {
                lock (_addApkLock)
                {
                    Invoke(new Action(() =>
                    {
                        AddApkToCategory(node, bio);

                    }));
                }
            }
            else
            {
                trvAPKCategories.SuspendLayout();
                var apkNode = new TreeNode(bio.RankInCategory.Value.ToString("D5") + " - " + bio.FileName) {Tag = bio};
                node.Nodes.Add(apkNode);
                trvAPKCategories.ResumeLayout();
            }
        }

        private object _apksLoading = new object();
     
        private void LoadApksIntoCategoriesWorker(TreeNode node, ApplicationCategoryWithCount applicationCategoryInfo)
        {
            if (Monitor.TryEnter(_apksLoading))
            {
                UpdateMainStatusText($"The initial load, need to preload paths...Please wait...");
                PathResolver.LoadAll();

                BinaryObject bioFilter = new BinaryObject();
                List<int> uniqueApks = new List<int>();
                QueryBinaryObject bioQuery = new QueryBinaryObject();
                UpdateMainStatusText($"Loading {applicationCategoryInfo.Name} category...");
                bioFilter.DataSetApplicationCategoryId = applicationCategoryInfo.DataSetApplicationCategoryId;
                bioFilter.IsRoot = 1;
                var apps = bioQuery.SelectBinaryObject(bioFilter).OrderBy(bio => bio.RankInCategory).ToList();
                uniqueApks.AddRange(apps.Select(app => app.ContentId.Value).ToList());
                foreach (var app in apps)
                {
                    AddApkToCategory(node, app);
                }

                UpdateMainStatusText("Done.");

                UpdateMainStatusText("Finished Loading BinaryObjects.");
                HideMainStatusProgress(node);
                Monitor.Exit(_apksLoading);
            }
            else
            {
                
            }
        }

        private void cmbBarDataSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBarDataSet.SelectedIndex >= 0 && cmbBarDataSet.SelectedIndex < cmbBarDataSet.Items.Count)
            {
                _currentDataSet = cmbBarDataSet.SelectedItem as DataSet;
                lblNumberOfAppsTotal.Text = $"Contains {_currentDataSet.BioCount} Objects";
                LoadCategoriesAndApps();
                SetEnableControlsOnToolBars(true);
            }
        }

        private void trvAPKCategories_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var categoryInfo = e.Node.Tag as ApplicationCategoryWithCount;
            if (categoryInfo != null && !categoryInfo.IsLoaded)
            {
                Task task = new Task(() => { LoadApksIntoCategoriesWorker(e.Node, categoryInfo);});
                task.Start();
            }
            else
            {
                var binaryInfo = e.Node.Tag as BinaryObject;
                if (binaryInfo != null)
                {
                    if (_binaryDetailsDialogs.ContainsKey(binaryInfo.UId.Value))
                    {
                        _binaryDetailsDialogs[binaryInfo.UId.Value].Activate();
                    }
                    else
                    {

                        var detailsDialog = new FBinaryDetails(binaryInfo);
                        _binaryDetailsDialogs.Add(binaryInfo.UId.Value, detailsDialog);
                        detailsDialog.Closed += DetailsDialog_Closed;
                        detailsDialog.Tag = binaryInfo;
                        detailsDialog.Show();
                    }
                }
            }
        }

        private void DetailsDialog_Closed(object sender, EventArgs e)
        {
            var detailsDialog = sender as FBinaryDetails;
            var binaryInfo = detailsDialog.Tag as BinaryObject;
            _binaryDetailsDialogs.Remove(binaryInfo.UId.Value);
        }

        private void btnBarProcessSmaliFiles_Click(object sender, EventArgs e)
        {
            OpenDialogForSmaliProcessing();
        }

        private void btnBarLibraries_Click(object sender, EventArgs e)
        {
            OpenLibrariesDialog();

        }

        private void btnBarCompressDb_Click(object sender, EventArgs e)
        {
            var dialog = new FCompressDb();
            dialog.ShowDialog();
        }

        private void OpenLibrariesDialog()
        {
            var dialog = new FLibraries();
            dialog.ShowDialog();
            SetEnableControlsOnToolBars(true);
        }

        private void SetEnableControlsOnToolBars(bool state)
        {
            btnBarProcessSmaliFiles.Enabled = state && cmbBarDataSet.SelectedItem != null;
            btnMenuProcessSmaliFiles.Enabled = state && cmbBarDataSet.SelectedItem != null;

            var query = new QueryStringValue();
            btnBarLibraries.Enabled = !query.IsCompressionRequired();
            btnBarCompressDb.Enabled = !btnBarLibraries.Enabled;
        }

        private void uploadAndDecodeAPKsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FDecodeAndUploadApks decodeDialog = new FDecodeAndUploadApks();
            decodeDialog.ShowDialog();
        }
    }
}
