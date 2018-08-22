using System;
using System.Collections.Generic;
using System.Windows.Forms;
using APKInsight.Configs;
using APKInsight.Logic;
using APKInsight.Logic.Task;

namespace APKInsight.Forms
{
    public partial class FDecodeApk : Form
    {
        private TaskDecodeApkFiles _decodeTask;
        private int _totalFilesUploaded;
        private int _totalFilesToUpload = 0;
        private int _uploadingThreadRunning = 0;
        private DateTime _lastStatsUpdateDateTime;
        private DateTime _initualUploaDateTime;
        private List<TaskUploadInternalFiles> _uploaders = new List<TaskUploadInternalFiles>();

        #region Constructor

        public FDecodeApk()
        {
            InitializeComponent();
            txtApkToolPath.Text = ApplicationConfiguration.ApkToolLocation;
            txtTempDir.Text = ApplicationConfiguration.TempDriveLocation;
        }

        #endregion


        #region Loading form

        private void FDecodeApk_Load(object sender, EventArgs e)
        {
        }

        #endregion


        #region Decoding processing

        private void btnDecodeAPKs_Click(object sender, EventArgs e)
        {
            SetEnableState(false);
            _decodeTask = new TaskDecodeApkFiles(txtTempDir.Text, txtApkToolPath.Text) { ParentForm = this };
            _decodeTask.LoadAllApksToProcess();
            _decodeTask.OnTaskCompleted += decodeTask_OnTaskCompleted;
            _decodeTask.OnTaskThreadCompleted += decodeTask_OnTaskThreadCompleted;
            _decodeTask.OnTaskThreadStarted += decodeTask_OnTaskThreadStarted;

            _decodeTask.ThreadsToUse = Convert.ToInt32(nudThreads.Value);

            prgProgressBar.Value = 0;
            prgProgressBar.Maximum = 1;
            prgFilesUploading.Value = 0;
            prgFilesUploading.Maximum = 0;

            _totalFilesToUpload = 0;
            _totalFilesUploaded = 0;
            _uploadingThreadRunning = 0;
            _lastStatsUpdateDateTime = DateTime.Now;
            _initualUploaDateTime = DateTime.Now;
            // Kick of the decoding tasks
            _decodeTask.StartThreads();
        }

        private void SetEnableState(bool state)
        {
            btnDecodeAPKs.Enabled = state;
            txtApkToolPath.Enabled = state;
            txtTempDir.Enabled = state;
            nudThreads.Enabled = state;
            nudFileUploaderThreads.Enabled = state;
        }

        #endregion


        #region Decoding task events

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
            StartInternalFilesUpload(apkArgs);
        }

        private void decodeTask_OnTaskCompleted(object sender, EventArgs args)
        {
            _decodeTask.OnTaskCompleted -= decodeTask_OnTaskCompleted;
            _decodeTask.OnTaskThreadCompleted -= decodeTask_OnTaskThreadCompleted;
            _decodeTask.OnTaskThreadStarted -= decodeTask_OnTaskThreadStarted;
            _decodeTask = null;
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
