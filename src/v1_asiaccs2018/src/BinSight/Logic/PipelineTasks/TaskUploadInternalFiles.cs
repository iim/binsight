using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APKInsight.Models;
using CsnowFramework;
using CsnowFramework.ThreadedTask;

namespace APKInsight.Logic.Task
{
    class TaskUploadInternalFiles: ThreadedTask
    {
        public List<string> Files { get; set; }
        public BinaryObject Bio { get; set; }
        public string RamDrivePath { get; set; }
        public string ApkToolCmdLine { get; set; }

        private readonly LockedValue<int> _nextFileIndex = new LockedValue<int>(0);
        private readonly object _schedulingLock = new object();
        
        protected override void ThreadWorkload(params object[] parameters)
        {
            var fileNames = parameters[0] as List<string>;
            DecodeApk logic = new DecodeApk(RamDrivePath, ApkToolCmdLine);
            for (int i = 0; i < fileNames.Count; i++)
            {
                logic.UploadAnInternalFile(Bio, fileNames[i]);
                RaiseOnTaskThreadItemCompleted(EventArgs.Empty);
            }
            RaiseOnTaskThreadCompleted();
        }

        protected override void StartThread()
        {
            List<string> data = null;
            lock (_schedulingLock)
            {
                var count = ThreadLoadSize;
                if (_nextFileIndex.Value + count > Files.Count)
                    count = Files.Count - _nextFileIndex.Value;
                data = Files.GetRange(_nextFileIndex.Value, count);
                _nextFileIndex.Value += count;
            }
            if (data != null && data.Count > 0)
            {
                ForkThread(data);
                return;
            }

            HaveWork = false;
        }
    }
}
