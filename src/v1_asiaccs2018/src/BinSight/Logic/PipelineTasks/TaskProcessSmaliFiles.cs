using System.Windows.Forms;
using APKInsight.Enums;
using APKInsight.Models;
using CsnowFramework;
using CsnowFramework.ThreadedTask;

namespace APKInsight.Logic.PipelineTasks
{
    /// <summary>
    /// The main smali processing task
    /// </summary>
    internal partial class TaskProcessSmaliFiles : ThreadedTask
    {
        private int _dataSetId;
        private LockedValue<int> _maxId = 0;

        /// <summary>
        /// Defines a number of smali files left for this worker thread.
        /// </summary>
        public LockedValue<int> LeftToSchedule { get; set; } = 0;


        #region Constructors

        /// <summary>
        /// Basic contructor without parent control
        /// </summary>
        /// <param name="dataSetId">Dataset Id to be used for all processing</param>
        /// <param name="stage">The stage which we are processing</param>
        public TaskProcessSmaliFiles(int dataSetId, BinaryObjectSmaliProcessingStage stage)
        {
            Init(dataSetId);
        }

        /// <summary>
        /// Basic contructor with parent as a control
        /// </summary>
        /// <param name="control">User control that serves as a parent for this task</param>
        /// <param name="dataSetId">Dataset Id to be used for all processing</param>
        /// <param name="stage">The stage which we are processing</param>
        public TaskProcessSmaliFiles(Control control, int dataSetId, BinaryObjectSmaliProcessingStage stage) :
            base(control)
        {
            Init(dataSetId);
        }

        /// <summary>
        /// Basic contructor with parent as a form
        /// </summary>
        /// <param name="form">Form that serves as a parent for this task</param>
        /// <param name="dataSetId">Dataset Id to be used for all processing</param>
        /// <param name="stage">The stage which we are processing</param>
        public TaskProcessSmaliFiles(Form form, int dataSetId, BinaryObjectSmaliProcessingStage stage) :
            base(form)
        {
            Init(dataSetId);
        }

        /// <summary>
        /// Shared initialization code.
        /// </summary>
        /// <param name="dataSetId">Dataset Id to be used for all processing</param>
        private void Init(int dataSetId)
        {
            _dataSetId = dataSetId;
        }

        #endregion


        #region Protected Override Functions

        /// <summary>
        /// Starts a processing thread
        /// </summary>
        protected override void StartThread()
        {
            StartJavaTypeProcessingThread();
        }

        /// <summary>
        /// Workload function.
        /// </summary>
        /// <param name="parameters">A set of parameters that include list of BIOs</param>
        protected override void ThreadWorkload(params object[] parameters)
        {
            var objectsToProcess = parameters[0] as BinaryObject;
            if (objectsToProcess != null)
                Stage1SaveJavaTypesDefinitions(objectsToProcess);
        }

        #endregion


        #region Stage Processing

        /// <summary>
        /// Overrides the default empty funciton and loads whatever is needed for specific stage
        /// </summary>
        protected override void PrepareTask()
        {
            PrepareSmaliProcessingStage();
        }

        #endregion


    }
}
