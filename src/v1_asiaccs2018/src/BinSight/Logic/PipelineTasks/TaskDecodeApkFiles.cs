using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APKInsight.Models;
using APKInsight.Queries;
using CsnowFramework;
using CsnowFramework.ThreadedTask;

namespace APKInsight.Logic.Task
{
    class TaskDecodeApkFiles: ThreadedTask
    {
        private readonly string _ramDrivePath;
        private readonly string _apkToolCmpLine;
        private List<BinaryObject> _apksToDecode;
        private readonly LockedValue<int> _nextApkToProcess = new LockedValue<int>(0);
        private readonly object _threadStartLock = new object();
        private List<string> _files = null;

        public TaskDecodeApkFiles(string ramDrivePath, string apkToolCmpLine)
        {
            _ramDrivePath = ramDrivePath;
            _apkToolCmpLine = apkToolCmpLine;
        }

        protected override void ThreadWorkload(params object[] parameters)
        {
            var bio = parameters[0] as BinaryObject;
            if (bio != null)
            {
                DecodeApk logic = new DecodeApk(_ramDrivePath, _apkToolCmpLine);
                logic.DecodeApkFile(bio);
                ThreadsSuspended++;
                RaiseOnTaskThreadCompleted(new ApkDecodedEventArgs {Bio = bio, DecodingProcessor = logic });
            }
            var filename = parameters[0] as string;
            if (filename != null)
            {
                DecodeApk logic = new DecodeApk(_ramDrivePath, _apkToolCmpLine);
                try
                {
                    logic.Failed = !logic.DecodeApkFile(filename);
                    ThreadsSuspended++;

                }
                catch (Exception exp)
                {
                    logic.Failed = true;
                }
                RaiseOnTaskThreadCompleted(new ApkDecodedEventArgs { FileName = filename, DecodingProcessor = logic});
            }
        }

        protected override void StartThread()
        {
            if (_apksToDecode != null)
            {
                StartThreadForUploadedApk();
            }
            else if (_files != null)
            {
                StartThreadForNotUploadedApk();
            }
        }

        private void StartThreadForUploadedApk()
        {
            BinaryObject bio = null;
            lock (_threadStartLock)
            {
                if (_nextApkToProcess.Value < _apksToDecode.Count)
                {
                    bio = _apksToDecode[_nextApkToProcess.Value];
                    _nextApkToProcess.Value++;
                }
                HaveWork = _nextApkToProcess.Value < _apksToDecode.Count;
            }
            if (bio != null)
            {
                ForkThread(bio);
            }
            else
            {
                HaveWork = false;
            }
        }

        private void StartThreadForNotUploadedApk()
        {
            string filename = null;
            lock (_threadStartLock)
            {
                if (_nextApkToProcess.Value < _files.Count)
                {
                    filename = _files[_nextApkToProcess.Value];
                    _nextApkToProcess.Value++;
                }
                HaveWork = _nextApkToProcess.Value < _files.Count;
            }
            if (filename != null)
            {
                ForkThread(filename);
            }
            else
            {
                HaveWork = false;
            }
        }

        public void LoadAllApksToProcess()
        {
            QueryBinaryObject bioQuery = new QueryBinaryObject();
            lock (_threadStartLock)
            {
                _apksToDecode = bioQuery.SelectBinaryObject(new BinaryObject() { IsRoot = 1, ProcessingStage = 0 });
            }
        }

        public void LoadNotLoadedApksToProcess(List<string> files)
        {
            lock (_threadStartLock)
            {
                _files = files;
                _apksToDecode = null;
            }
        }

        public int NumberOfApksToDecode() => _apksToDecode?.Count ?? _files.Count;
    }

    class ApkDecodedEventArgs : EventArgs
    {
        public BinaryObject Bio { get; set; }
        public string FileName { get; set; }
        public DecodeApk DecodingProcessor { get; set; }
    }

}
