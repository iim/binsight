using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using APKInsight.Queries;
using System.Configuration;
using System.Linq;
using APKInsight.Enums;
using APKInsight.Logic.PipelineTasks;
using APKInsight.Models.DataBase;

namespace APKInsight.Forms
{
    /// <summary>
    /// The main form that digests and processes Smali files, so that they can be seen after.
    /// </summary>
    internal partial class FProcessSmaliFiles : Form
    {
        private readonly DataSet _dataSet;
        private int _processed = 0;
        private int _toProcess = 0;
        private int _processedThusFar = 0;
        private bool _closeOnceThreadStopped = false;
        private const string kThreadCountParameterName = "SmaliParsingTrheadCount";
        private const string TrheadWorkLoadSizeParameterName = "SmaliParsingTrheadWorkLoadSize";

        private TaskProcessSmaliFiles _taskProcessSmaliFiles;

        // Speed reporting variables
        private DateTime _processingBegan;
        private DateTime _previousSpeedReportAt;

        #region Constructors and Destructor

        /// <summary>
        /// Constructor that expects data set record for which the data processing is done.
        /// Note, not all stages take data set into account.
        /// </summary>
        /// <param name="dataSet">Dataset record that we do processing for</param>
        public FProcessSmaliFiles(DataSet dataSet)
        {
            _dataSet = dataSet;
            InitializeComponent();
        }

        #endregion


        #region Form Load and Open

        // Load required details on form load
        private void FProcessSmaliFiles_Load(object sender, EventArgs e)
        {
            lblSpeedReport.Text = "";
            lblTimeRemaining.Text = "";
            lblTimeElapsed.Text = "";

            EnableControls(true);
            UpdateFormTitle();
        }

        // Handle form closing
        private void FProcessSmaliFiles_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If we are in the process of data processing, cancel form closing and wait for processing to get fully cancelled.
            if (_taskProcessSmaliFiles != null && !_taskProcessSmaliFiles.Cancelled)
            {
                _taskProcessSmaliFiles.Cancelled = true;
                e.Cancel = true;
                _closeOnceThreadStopped = true;
            }
        }

        #endregion


        #region User actions

        // Begins data processing for selected stage
        private void btnProcess_Click(object sender, EventArgs e)
        {
            EnableControls(false);
            _processingBegan = DateTime.Now;
            _previousSpeedReportAt = DateTime.Now;
            StartProcessing();
        }

        // Stops current data processing
        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            _taskProcessSmaliFiles.Cancelled = true;
        }


        #endregion


        #region Stages processing

        // Starts data processing
        private void StartProcessing()
        {
            _processedThusFar = 0;
            _processed = 0;
            CreateTaskSmaliProcessing();

            // Show the processing
            prgLoadProgress.Visible = true;
            Application.DoEvents();;

            _taskProcessSmaliFiles.Prepare();
        }

        // Creates task object for smali files processing
        private void CreateTaskSmaliProcessing()
        {
            _taskProcessSmaliFiles = new TaskProcessSmaliFiles(this, _dataSet.UId.Value, BinaryObjectSmaliProcessingStage.Processed)
            {
                ThreadsToUse =
                ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(key => key == kThreadCountParameterName) == null ?
                    32 :
                    Convert.ToInt32(ConfigurationManager.AppSettings[kThreadCountParameterName]),
                ThreadLoadSize = ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(key => key == TrheadWorkLoadSizeParameterName) == null ?
                    32 :
                    Convert.ToInt32(ConfigurationManager.AppSettings[TrheadWorkLoadSizeParameterName])
            };
            _taskProcessSmaliFiles.OnTaskThreadItemCompleted += _taskProcessSmaliFiles_OnTaskThreadItemCompleted;
            _taskProcessSmaliFiles.OnTaskCompleted += _taskProcessSmaliFiles_OnTaskCompleted;
            _taskProcessSmaliFiles.OnTaskThreadStarted += _taskProcessSmaliFiles_OnTaskThreadStarted;
            _taskProcessSmaliFiles.OnTaskThreadCompleted += _taskProcessSmaliFiles_OnTaskThreadCompleted;
            _taskProcessSmaliFiles.OnTaskPrepared += _taskProcessSmaliFiles_OnTaskPrepared;
        }

        // Enables/disables controls based on processing status
        private void EnableControls(bool state, bool? stateNegative = null)
        {
            btnProcess.Enabled = state;
            if (stateNegative.HasValue)
            {
                btnStop.Enabled = stateNegative.Value;
            }
            else
            {
                btnStop.Enabled = !state;
            }
        }

        #endregion


        #region Processing Task Events

        private void _taskProcessSmaliFiles_OnTaskPrepared(object sender, EventArgs args)
        {
            _toProcess = _taskProcessSmaliFiles.LeftToSchedule.Value;
            _processed = 0;
            prgLoadProgress.Enabled = false;
            prgLoadProgress.Visible = false;
            prbProgress.Value = 0;
            prbProgress.Maximum = _toProcess;
            _taskProcessSmaliFiles.StartThreads();
        }

        private void _taskProcessSmaliFiles_OnTaskThreadCompleted(object sender, EventArgs args)
        {
            UpdateFormTitle();
        }

        private void _taskProcessSmaliFiles_OnTaskThreadStarted(object sender, EventArgs args)
        {
            UpdateFormTitle();
        }

        private void _taskProcessSmaliFiles_OnTaskCompleted(object sender, EventArgs args)
        {
            _taskProcessSmaliFiles.OnTaskThreadItemCompleted -= _taskProcessSmaliFiles_OnTaskThreadItemCompleted;
            _taskProcessSmaliFiles.OnTaskCompleted -= _taskProcessSmaliFiles_OnTaskCompleted;
            _taskProcessSmaliFiles.OnTaskThreadStarted -= _taskProcessSmaliFiles_OnTaskThreadStarted;
            _taskProcessSmaliFiles.OnTaskThreadCompleted -= _taskProcessSmaliFiles_OnTaskThreadCompleted;

            _taskProcessSmaliFiles = null;
            _toProcess = 0;
            UpdateStatLabel();
            EnableControls(true);
            if (_closeOnceThreadStopped)
                Close();
        }

        private void _taskProcessSmaliFiles_OnTaskThreadItemCompleted(object sender, EventArgs args)
        {
            _processedThusFar++;
            _processedThusFar = _processedThusFar > prbProgress.Maximum ? prbProgress.Maximum:_processedThusFar ;
            _processed++;
            _toProcess--;
            UpdateStatLabel();
        }

        #endregion


        #region Progress Reporting


        private void UpdateStatLabel()
        {
            if (_processed > 0)
            {
                lblStatValue.Text =
                    $"To Process: {_toProcess}, Processed {_processed}";
                //prbProgress.Value = _processedThusFar;
                var elapsed = (DateTime.Now - _previousSpeedReportAt).TotalSeconds;
                if (elapsed > 5)
                {
                    _previousSpeedReportAt = DateTime.Now;
                    var speed = _processed / (_previousSpeedReportAt - _processingBegan).TotalSeconds;
                    if (speed > 0.0001)
                    {
                        var secondsRemaining = Convert.ToDouble(_toProcess)/speed;
                        int h = Convert.ToInt32(secondsRemaining) / 3600;
                        int m = Convert.ToInt32(secondsRemaining) % 3600 / 60;
                        int s = Convert.ToInt32(secondsRemaining) % 3600 % 60;
                        lblSpeedReport.Text = $"Speed: {(60 * speed).ToString("F4")} APK files per minute.";
                        lblTimeRemaining.Text = $"{h}:{m}:{s} remaining to finish processing.";
                        lblTimeElapsed.Text = (_previousSpeedReportAt - _processingBegan).ToString("g");
                    }
                }
            }
        }

        private void UpdateFormTitle()
        {
            if (_taskProcessSmaliFiles != null)
            {
                Text = _taskProcessSmaliFiles.IsRunning
                    ? $"Smali Processing Dialog (Using  {_taskProcessSmaliFiles.CurrentThreadsCount} out of {_taskProcessSmaliFiles.ThreadsToUse} threads)"
                    : $"Smali Processing Dialog (Using {_taskProcessSmaliFiles.ThreadsToUse} threads)";
            }
        }

        #endregion

    }
}
