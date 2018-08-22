using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CsnowFramework.ThreadedTask
{
    /// <summary>
    /// Task prepared delegate.
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="args">Event arguments</param>
    public delegate void TaskPrepared(object sender, EventArgs args);

    /// <summary>
    /// Task completed delegate.
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="args">Event arguments</param>
    public delegate void TaskCompleted(object sender, EventArgs args);

    /// <summary>
    /// Task thread item completed.
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="args">args</param>
    public delegate void TaskThreadItemCompleted(object sender, EventArgs args);

    /// <summary>
    /// Task thread completed
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="args">args</param>
    public delegate void TaskThreadCompleted(object sender, EventArgs args);

    /// <summary>
    /// Task thread started
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="args">args</param>
    public delegate void TaskThreadStarted(object sender, EventArgs args);


    /// <summary>
    /// Threaded task allows to schedule multiple threads in a simple manner.
    /// </summary>
    public abstract class ThreadedTask
    {

        #region Public properties

        /// <summary>
        /// Parent form that uses this threaded task.
        /// </summary>
        public Form ParentForm { get; set; }

        /// <summary>
        /// Parent control that uses this threaded task.
        /// </summary>
        public Control ParentControl { get; set; }

        /// <summary>
        /// Fires when a task is prepared.
        /// </summary>
        public event TaskPrepared OnTaskPrepared;

        /// <summary>
        /// Fires when all task threads complete their assign task and there is no more work to schedule.
        /// </summary>
        public event TaskCompleted OnTaskCompleted;

        /// <summary>
        /// Fires when a single task item has been completed.
        /// </summary>
        public event TaskThreadItemCompleted OnTaskThreadItemCompleted;

        /// <summary>
        /// Fires when a thread has completed.
        /// </summary>
        public event TaskThreadCompleted OnTaskThreadCompleted;

        /// <summary>
        /// Fires when a thread has been started.
        /// </summary>
        public event TaskThreadStarted OnTaskThreadStarted;

        private readonly object _waitLock = new object();

        /// <summary>
        /// Defines how many worker threads a task should use.
        /// </summary>
        public int ThreadsToUse
        {
            get { return _threadsToUse.Value; }
            set
            {
                if (value < 0)
                    throw  new Exception("Number of worker threads should be more than 0.");
                _threadsToUse.Value = value;
            }
        }

        /// <summary>
        /// Defines how many worker threads are suspended
        /// </summary>
        public int ThreadsSuspended
        {
            get { return _threadsToSuspended.Value; }
            set
            {
                if (value < 0)
                    throw new Exception("Number of worker threads should be more than 0.");
                _threadsToSuspended.Value = value;
            }
        }

        /// <summary>
        /// A generic number to define each worker thread workload size.
        /// This number does not have any enforced meaning, rather than can be used to specify
        /// work size for each thread.
        /// </summary>
        public int ThreadLoadSize
        {
            get { return _threadLoadSize.Value; }
            set { _threadLoadSize.Value = value; }
        }

        /// <summary>
        /// Threadsafe property to get current number of threads in use.
        /// </summary>
        public int CurrentThreadsCount
        {
            get { return _currentlyRunningThreadsCount.Value; }
            private set { _currentlyRunningThreadsCount.Value = value; }
        }

        /// <summary>
        /// Threadsafe property that shows whenever or not the task was cancelled.
        /// All classes that inherit from this class must respect this flag and check for it.
        /// </summary>
        public bool Cancelled
        {
            get { return _cancelled.Value; }
            set { _cancelled.Value = value; }
        }

        /// <summary>
        /// Threadsafe property that shows whenever or not the task is currently running.
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning.Value; }
            private set { _isRunning.Value = value; }
        }

        /// <summary>
        /// Threadsafe property that shows whenever or not there is still work remaining and
        /// new worker threads should be schedulled upon another worker thread completion.
        /// </summary>
        protected bool HaveWork
        {
            get { return _haveWork.Value; }
            set { _haveWork.Value = value; }
        }



        #endregion


        #region Class internal members

        // State flags
        private readonly LockedValue<bool> _cancelled = false;
        private readonly LockedValue<bool> _haveWork = false;
        private readonly LockedValue<bool> _isRunning = false;

        // Internal scheduler objects
        private readonly LockedValue<int> _threadsToUse = 1;
        private readonly LockedValue<int> _threadsToSuspended = 0;
        private readonly LockedValue<int> _threadLoadSize = 0;
        private readonly LockedValue<int> _currentlyRunningThreadsCount = 0;
        private Thread _scheduler;
        private readonly List<Thread> _workerThreads = new List<Thread>();

        // Locks
        private readonly object _workerThreadsLock = new object();
        private readonly object _forkingLock = new object();

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor is allowed, but should be used with care due to multithreading.
        /// </summary>
        protected ThreadedTask()
        { }

        /// <summary>
        /// Constructor for the case when GUI is a control.
        /// </summary>
        /// <param name="parentControl">The parent control that needs reporting.</param>
        protected ThreadedTask(Control parentControl)
        {
            ParentControl = parentControl;
        }

        /// <summary>
        /// Constructor for the case when GUI is a form.
        /// </summary>
        /// <param name="parentForm">The parent form that needs reporting.</param>
        protected ThreadedTask(Form parentForm)
        {
            ParentForm = parentForm;
        }

        #endregion


        #region Public function

        /// <summary>
        /// Starts the scheduler in a separate thread, which kick starts all required threads.
        /// </summary>
        public void StartThreads()
        {
            if (_scheduler == null)
            {
                Cancelled = false;
                HaveWork = true;
                IsRunning = true;
                _scheduler = new Thread(SchedulerInfiniteLoop);
                _scheduler.Start();
            }
        }

        /// <summary>
        /// Prepares the task.
        /// </summary>
        public void Prepare()
        {
            new Task(() => 
            {
                PrepareTask();
                RaiseOnTaskPrepared();
            }).Start();
        }

        #endregion


        #region Protected functions

        /// <summary>
        /// Works like a usual linux fork() function, forks a new thread and passes it a set of parameters.
        /// </summary>
        /// <param name="parameters">Parameters to pass to the ThreadWorkLoad</param>
        protected void ForkThread(params object[] parameters)
        {
            lock (_forkingLock)
            {
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        ThreadWorkload(parameters);
                    }
                    catch (Exception exp)
                    {
                    }
                });
                CurrentThreadsCount++;
                lock (_workerThreadsLock)
                {
                    _workerThreads.Add(thread);
                    thread.Start();
                }
            }
        }

        /// <summary>
        /// Waits for all threads to complete.
        /// </summary>
        protected void WaitForAllThreadsToComplete()
        {
            lock (_waitLock)
            {

            }
        }

        #endregion


        #region Private functions

        /// <summary>
        /// The main "infinite" loop of the scheduler.
        /// </summary>
        private void SchedulerInfiniteLoop()
        {
            lock (_waitLock)
            {

                while (!Cancelled && HaveWork)
                {
                    // Delete old workers
                    CleanupThreads();
                    // Start new workers
                    var threadThatShouldBeUsed = ThreadsToUse - ThreadsSuspended;
                    while (CurrentThreadsCount < threadThatShouldBeUsed && threadThatShouldBeUsed > 0 && HaveWork)
                    {
                        if (Cancelled)
                            break;
                        StartThread();
                        RaiseOnTaskThreadStarted();
                    }
                    if (!Cancelled)
                        Thread.Sleep(1000);
                }

                while (CurrentThreadsCount > 0)
                {
                    CleanupThreads();
                    Thread.Sleep(250);
                }

                IsRunning = false;
            }
            RaiseOnTaskCompleted(EventArgs.Empty);
            _scheduler = null;
        }

        /// <summary>
        /// Cleans up all terminated threads.
        /// </summary>
        private void CleanupThreads()
        {
            lock (_workerThreadsLock)
            {
                for (int i = _workerThreads.Count - 1; i >= 0; i--)
                {
                    if (_workerThreads[i].ThreadState == ThreadState.Stopped)
                    {
                        _workerThreads.RemoveAt(i);
                        CurrentThreadsCount--;
                    }
                }
            }
        }

        /// <summary>
        /// Execute an task on the GUI's thread depending on what kind of control is parenting
        /// the task.
        /// </summary>
        /// <param name="action">An action to be executed</param>
        private void ExecuteAction(Action action)
        {
            if (ParentForm != null && ParentForm.InvokeRequired)
            {
                ParentForm.Invoke(action);
            }
            else if (ParentControl != null && ParentControl.InvokeRequired)
            {
                ParentControl.Invoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        #endregion


        #region Abstract or Empty virtual functions

        /// <summary>
        /// An implementation of thread's workload function with a set of parameters.
        /// </summary>
        /// <param name="parameters"></param>
        protected abstract void ThreadWorkload(params object[] parameters);

        /// <summary>
        /// Fetch data for a worker thread and fork it.
        /// </summary>
        protected abstract void StartThread();

        /// <summary>
        /// Prepares the task for execution.
        /// This part is executed before any worker thread is schedulled.
        /// </summary>
        protected virtual void PrepareTask()
        {
        }

        #endregion


        #region Event raisers.

        /// <summary>
        /// Raise an event when the task has been prepared.
        /// </summary>
        private void RaiseOnTaskPrepared()
        {
            ExecuteAction(() => { OnTaskPrepared?.Invoke(this, EventArgs.Empty); });
        }

        /// <summary>
        /// Raise an event when a thread item is completed.
        /// </summary>
        protected virtual void RaiseOnTaskThreadItemCompleted(EventArgs e = null)
        {
            ExecuteAction(() => { OnTaskThreadItemCompleted?.Invoke(this, e ?? EventArgs.Empty); });
        }

        /// <summary>
        /// Raise an event when all task's threads are completed.
        /// </summary>
        protected virtual void RaiseOnTaskCompleted(EventArgs e)
        {
            ExecuteAction(() => { OnTaskCompleted?.Invoke(this, e); });
        }

        /// <summary>
        /// Raise an event when a task thread is completed.
        /// </summary>
        protected virtual void RaiseOnTaskThreadCompleted(EventArgs args = null)
        {
            ExecuteAction(() => { OnTaskThreadCompleted?.Invoke(this, args ?? EventArgs.Empty); });
        }

        /// <summary>
        /// Raise an event when a task thread is started.
        /// </summary>
        protected virtual void RaiseOnTaskThreadStarted()
        {
            ExecuteAction(() => { OnTaskThreadStarted?.Invoke(this, EventArgs.Empty); });
        }

        #endregion

    }
}
