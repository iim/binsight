using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using APKInsight.Configs;
using APKInsight.Logic;
using APKInsight.Logic.Analysis;
using APKInsight.Logic.Analysis.Data;
using APKInsight.Logic.Task;
using CsnowFramework.InputOutput;

namespace APKInsight.Forms
{

    /// <summary>
    /// Dialog that analyzes APK files and uploads them if needed and requested
    /// </summary>
    public partial class FDecodeAndUploadApks : Form
    {
        //private string _useCaseAnalysisPath = @"..\..\..\..\data\stage1_filtering\";
        private TaskDecodeApkFiles _decodeTask;
        private int _totalFilesUploaded;
        private int _totalFilesToUpload = 0;
        private int _uploadingThreadRunning = 0;
        private DateTime _lastStatsUpdateDateTime;
        private DateTime _initualUploaDateTime;
        private List<TaskUploadInternalFiles> _uploaders = new List<TaskUploadInternalFiles>();

        private List<ApkInfo> _detectedUseCases;
        private List<string> _filesInDir;
        private List<string> _files;
        private int _reportFrequency = 100;
        
        
        #region Constructor

        public FDecodeAndUploadApks()
        {
            InitializeComponent();
            txtApkToolPath.Text = ApplicationConfiguration.ApkToolLocation;
            txtTempDir.Text = ApplicationConfiguration.TempDriveLocation;
        }

        #endregion


        #region Loading form

        private void FDecodeApk_Load(object sender, EventArgs e)
        {
            SetUploadButtonEnabledState();
#if DEBUG
            LoadDirectory(@"d:\_testds");
            _detectedUseCases = Reader.GetApkinfo(@"d:\_testds_found_usecases.csv");
            UpdateFilesToProcess();
            txtReportPath.Text = @"d:\";
            //chkCCS13Rule1NoECB.Checked = true;
            //chkCCS13Rule2NoStaticIv.Checked = true;
            //chkNoStaticKeysForSymmetricCrypto.Checked = true;
            //chkNoStaticSalt.Checked = true;
            //chkRule5FewIterations.Checked = true;
            //chkRule6StaticSeed.Checked = true;
            chkAnalyzeDataFlowAnalysis.Checked = true;
#endif
        }


        private void SetUploadButtonEnabledState()
        {
            btnLoadFilter.Enabled = false;
            btnResetFilter.Enabled = false;

            grpDecodingProcess.Enabled = false;
        }

        #endregion


        #region User Actions

        private void btnSelectDirectory_Click(object sender, EventArgs e)
        {
            if (dlgDirectorySelectionDialog.ShowDialog() == DialogResult.OK)
            {
                btnSelectDirectory.Enabled = false;
                LoadDirectory(dlgDirectorySelectionDialog.SelectedPath);
                btnResetFilter.Enabled = false;
            }

            btnSelectDirectory.Enabled = true;
        }

        private void LoadDirectory(string dirName)
        {
            lblSelectedDirectory.Text = dirName;
            _filesInDir = Utilities.GetChildFiles(dirName);
            _filesInDir = _filesInDir.Where(fn => Path.GetFileName(fn).Length == 40 || fn.EndsWith(".apk")).ToList();
            UpdateFilesToProcess();
            _detectedUseCases = null;
            btnLoadFilter.Enabled = _filesInDir.Count > 0;

        }

        private void btnLoadFilter_Click(object sender, EventArgs e)
        {
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                _detectedUseCases = Reader.GetApkinfo(dlgOpenFile.FileName);
            }
            UpdateFilesToProcess();
        }

        private void UpdateFilesToProcess()
        {
            if (_detectedUseCases != null && _detectedUseCases.Count > 0)
            {
                var filenames = _detectedUseCases.Select(o => Path.GetFileName(o.Filename)).ToList();
                _files = _filesInDir.Where(fn => filenames.Contains(Path.GetFileName(fn))).ToList();
                btnResetFilter.Enabled = false;
            }
            else
            {
                _files = new List<string>(_filesInDir);
            }

            lblFoundObjects.Text = _files.Count.ToString();
            grpDecodingProcess.Enabled = _files.Count > 0;

        }

        private void btnResetFilter_Click(object sender, EventArgs e)
        {
            _detectedUseCases = null;
            UpdateFilesToProcess();
        }

        #endregion


        #region Decoding processing

        private void btnDecodeAPKs_Click(object sender, EventArgs e)
        {
            SetEnableState(false);

            _decodeTask = new TaskDecodeApkFiles(txtTempDir.Text, txtApkToolPath.Text) { ParentForm = this };
            _decodeTask.LoadNotLoadedApksToProcess(_files);
            _decodeTask.OnTaskCompleted += decodeTask_OnTaskCompleted;
            _decodeTask.OnTaskThreadCompleted += decodeTask_OnTaskThreadCompleted;
            _decodeTask.OnTaskThreadStarted += decodeTask_OnTaskThreadStarted;

            _decodeTask.ThreadsToUse = Convert.ToInt32(nudThreads.Value);

            // Set flags
            DecodeApk.FindAllUseCases = chkFindAllUseCases.Checked;
            DecodeApk.EvalRule1FromCcs13 = chkCCS13Rule1NoECB.Checked;
            DecodeApk.EvalRule2FromCcs13 = chkCCS13Rule2NoStaticIv.Checked;
            DecodeApk.EvalRule3FromCcs13 = chkNoStaticKeysForSymmetricCrypto.Checked;
            DecodeApk.EvalRule4FromCcs13 = chkNoStaticSalt.Checked;
            DecodeApk.EvalRule5FromCcs13 = chkRule5FewIterations.Checked;
            DecodeApk.EvalRule6FromCcs13 = chkRule6StaticSeed.Checked;
            DecodeApk.EvalDataFlowAnalysis = chkAnalyzeDataFlowAnalysis.Checked;
            DecodeApk.UseCases = _detectedUseCases;

            // Set report filename, and read in the 
            prgProgressBar.Value = 0;
            prgProgressBar.Maximum = 1;
            prgFilesUploading.Value = 0;
            prgFilesUploading.Maximum = 0;

            _totalFilesToUpload = 0;
            _totalFilesUploaded = 0;
            _uploadingThreadRunning = 0;
            _lastStatsUpdateDateTime = DateTime.Now;
            _initualUploaDateTime = DateTime.Now;

            InitReports();

            // Kick of the decoding tasks
            _decodeTask.StartThreads();
        }

        private void SetEnableState(bool state)
        {
            btnDecodeAPKs.Enabled = state;
            txtApkToolPath.Enabled = state;
            txtTempDir.Enabled = state;
            txtReportPath.Enabled = state;
            nudThreads.Enabled = state;
            nudFileUploaderThreads.Enabled = state;
            grpDirectorySelection.Enabled = state;
        }

        #endregion


        #region Decoding task events

        private int _reportAppN = 0;
        private int _reportAppNFailed = 0;

        private void decodeTask_OnTaskThreadStarted(object sender, EventArgs args)
        {
            prgProgressBar.Maximum = _decodeTask.NumberOfApksToDecode();
            lblThreadsCount.Text = _decodeTask.CurrentThreadsCount.ToString();
            lblProgress.Text = $"Processed {prgProgressBar.Value} APK files out of {prgProgressBar.Maximum}";
        }

        private void decodeTask_OnTaskThreadCompleted(object sender, EventArgs args)
        {
            prgProgressBar.Value++;
            lblThreadsCount.Text = _decodeTask.CurrentThreadsCount.ToString();
            lblProgress.Text = $"Processed {prgProgressBar.Value} APK files out of {prgProgressBar.Maximum}";
            ApkDecodedEventArgs apkArgs = args as ApkDecodedEventArgs;
            if (apkArgs.DecodingProcessor.Failed)
            {
                _reportAppNFailed++;
            }
            else
            {
                _reportAppN++;
            }
            lblInterestingApkN.Text = $"{_reportAppN}/{_reportAppNFailed}";
            _decodeTask.ThreadsSuspended--;
            if (prgProgressBar.Value % _reportFrequency == 0)
            {
                SaveReports();
            }
        }

        private void decodeTask_OnTaskCompleted(object sender, EventArgs args)
        {
            _decodeTask.OnTaskCompleted -= decodeTask_OnTaskCompleted;
            _decodeTask.OnTaskThreadCompleted -= decodeTask_OnTaskThreadCompleted;
            _decodeTask.OnTaskThreadStarted -= decodeTask_OnTaskThreadStarted;
            _decodeTask = null;
            SaveReports();

        }

        private void InitReports()
        {
            DecodeApk.InitReport();
            AnalysisLogicFindAllUseCases.InitReport();
            AnalysisLogicRule1Ccs13.InitReport();
            AnalysisLogicRule2Ccs13.InitReport();
            AnalysisLogicRule3Ccs13.InitReport();
            AnalysisLogicRule4Ccs13.InitReport();
            AnalysisLogicRule5Ccs13.InitReport();
            AnalysisLogicRule6Ccs13.InitReport();
            AnalysisLogicDataFlowForCipher.InitReport();
        }

        private void SaveReports()
        {
            DecodeApk.SaveFailedReport(Path.Combine(txtReportPath.Text, "failed2decode.csv"));
            if (chkFindAllUseCases.Checked)
                AnalysisLogicFindAllUseCases.SaveReport(
                    Path.Combine(txtReportPath.Text, "stage1_found_usecases.csv"),
                    Path.Combine(txtReportPath.Text, "stage1_notfound_usecases.csv"));
            if (chkCCS13Rule1NoECB.Checked)
                AnalysisLogicRule1Ccs13.SaveReport(Path.Combine(txtReportPath.Text, "stage2_rule1.csv"));
            if (chkCCS13Rule2NoStaticIv.Checked)
                AnalysisLogicRule2Ccs13.SaveReport(Path.Combine(txtReportPath.Text, "stage2_rule2.csv"));
            if (chkNoStaticKeysForSymmetricCrypto.Checked)
                AnalysisLogicRule3Ccs13.SaveReport(Path.Combine(txtReportPath.Text, "stage2_rule3.csv"));
            if (chkNoStaticSalt.Checked)
                AnalysisLogicRule4Ccs13.SaveReport(Path.Combine(txtReportPath.Text, "stage2_rule4.csv"));
            if (chkRule5FewIterations.Checked)
                AnalysisLogicRule5Ccs13.SaveReport(Path.Combine(txtReportPath.Text, "stage2_rule5.csv"));
            if (chkRule6StaticSeed.Checked)
                AnalysisLogicRule6Ccs13.SaveReport(Path.Combine(txtReportPath.Text, "stage2_rule6.csv"));
            if (chkAnalyzeDataFlowAnalysis.Checked)
                AnalysisLogicDataFlowForCipher.SaveReport(Path.Combine(txtReportPath.Text, "stage2_rule7.csv"));
        }

        #endregion


        #region Internal files uploading

        private void StartInternalFilesUpload(ApkDecodedEventArgs apkArgs)
        {
            _totalFilesToUpload += apkArgs.DecodingProcessor.Files.Count;
            prgFilesUploading.Maximum = _totalFilesToUpload;

            int threadsCount = Convert.ToInt32(nudFileUploaderThreads.Value);
            int workloadSize = apkArgs.DecodingProcessor.Files.Count/threadsCount;
            workloadSize = workloadSize > 1000
                ? 1000
                : workloadSize;

            TaskUploadInternalFiles fileUploader = new TaskUploadInternalFiles
            {
                Bio = apkArgs.Bio,
                Files = apkArgs.DecodingProcessor.Files,
                ApkToolCmdLine = txtApkToolPath.Text,
                RamDrivePath = txtTempDir.Text,
                ThreadsToUse = threadsCount,
                ThreadLoadSize = workloadSize,
                ParentForm = this
            };
            fileUploader.OnTaskThreadItemCompleted += FileUploader_OnTaskThreadItemCompleted;
            fileUploader.OnTaskThreadCompleted += FileUploader_OnTaskThreadCompleted;
            fileUploader.OnTaskThreadStarted += FileUploader_OnTaskThreadStarted;
            fileUploader.OnTaskCompleted += FileUploader_OnTaskCompleted;
            _uploaders.Add(fileUploader);
            fileUploader.StartThreads();
        }

        private void UpdateRunningUploadingThreads()
        {
            _uploadingThreadRunning = 0;
            foreach (var taskUploadInternalFilese in _uploaders)
            {
                _uploadingThreadRunning += taskUploadInternalFilese.CurrentThreadsCount;
            }
            lblRunningUploadingThreads.Text = $"{_uploadingThreadRunning}";

            lblThreadsCount.Text = _decodeTask?.CurrentThreadsCount.ToString() ?? "0";
        }

        private void UpdateUploadingStats()
        {
            _lastStatsUpdateDateTime = DateTime.Now;
            prgFilesUploading.Value = _totalFilesUploaded;
            double speed = _totalFilesUploaded / (DateTime.Now - _initualUploaDateTime).TotalSeconds;
            lblFilesUploaded.Text = $"{_totalFilesUploaded}/{_totalFilesToUpload} ({speed} files/sec)";
        }

        #endregion


        #region Internal files uploading task events


        private void FileUploader_OnTaskThreadStarted(object sender, EventArgs args)
        {
            _uploadingThreadRunning++;
            UpdateRunningUploadingThreads();
        }

        private void FileUploader_OnTaskThreadItemCompleted(object sender, EventArgs args)
        {
            _totalFilesUploaded++;
            if ((DateTime.Now - _lastStatsUpdateDateTime).TotalSeconds > 5.0)
            {
                UpdateUploadingStats();
            }
        }

        private void FileUploader_OnTaskThreadCompleted(object sender, EventArgs args)
        {
            _uploadingThreadRunning--;
            UpdateRunningUploadingThreads();
            UpdateUploadingStats();
        }
 
        private void FileUploader_OnTaskCompleted(object sender, EventArgs args)
        {
            UpdateUploadingStats();
            TaskUploadInternalFiles fileUploader = sender as TaskUploadInternalFiles;
            fileUploader.OnTaskThreadItemCompleted -= FileUploader_OnTaskThreadItemCompleted;
            fileUploader.OnTaskThreadCompleted -= FileUploader_OnTaskThreadCompleted;
            fileUploader.OnTaskThreadStarted -= FileUploader_OnTaskThreadStarted;
            fileUploader.OnTaskCompleted -= FileUploader_OnTaskCompleted;
            if (_decodeTask != null)
                _decodeTask.ThreadsSuspended--;

            for (int i = 0; i < _uploaders.Count; i++)
            {
                if (_uploaders[i].Equals(fileUploader))
                {
                    _uploaders.RemoveAt(i);
                }
            }
            DecodeApk logic = new DecodeApk(txtTempDir.Text, txtApkToolPath.Text);
            logic.FinalizeDecoding(fileUploader.Bio);
            UpdateRunningUploadingThreads();

            if (_decodeTask == null && _uploadingThreadRunning == 0)
            {
                SetEnableState(true);
            }
        }


        #endregion

    }
}
