using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace VS.Common.DoctestTestAdapter
{
    [Export(typeof(ITestFilesUpdateListener))]
    public class TestFilesUpdateListener : IDisposable, ITestFilesUpdateListener
    {
        private class FileListenerInfo
        {
            public FileListenerInfo(FileSystemWatcher _watcher)
            {
                Watcher = _watcher;
                LastEventTime = DateTime.MinValue;
            }

            public FileSystemWatcher Watcher { get; set; }
            public DateTime LastEventTime { get; set; }
        }

        private IDictionary<string, FileListenerInfo> fileListeners = new Dictionary<string, FileListenerInfo>(StringComparer.OrdinalIgnoreCase);
        public event EventHandler<TestFileChangedEventArgs> FileChangedEvent;

        public TestFilesUpdateListener()
        {

        }

        public void AddFileListener(string _path)
        {
            ValidateArg.NotNullOrEmpty(_path, "path");

            if (!String.IsNullOrEmpty(_path))
            {
                string directoryName = Path.GetDirectoryName(_path);
                string fileName = Path.GetFileName(_path);

                FileListenerInfo fileListenerInfo;
                if (!fileListeners.TryGetValue(_path, out fileListenerInfo))
                {
                    fileListenerInfo = new FileListenerInfo(new FileSystemWatcher(directoryName, fileName));
                    fileListeners.Add(_path, fileListenerInfo);

                    fileListenerInfo.Watcher.Changed += OnChanged;
                    fileListenerInfo.Watcher.EnableRaisingEvents = true;
                }
            }
        }

        public void RemoveFileListener(string _path)
        {
            ValidateArg.NotNullOrEmpty(_path, "path");

            if (!String.IsNullOrEmpty(_path))
            {
                FileListenerInfo fileListenerInfo;
                if (fileListeners.TryGetValue(_path, out fileListenerInfo))
                {
                    fileListenerInfo.Watcher.EnableRaisingEvents = false;

                    fileListeners.Remove(_path);

                    fileListenerInfo.Watcher.Changed -= OnChanged;
                    fileListenerInfo.Watcher.Dispose();
                    fileListenerInfo.Watcher = null;
                }
            }
        }

        private void OnChanged(object _sender, FileSystemEventArgs _e)
        {
            FileListenerInfo fileListenerInfo;
            if (FileChangedEvent != null && fileListeners.TryGetValue(_e.FullPath, out fileListenerInfo))
            {
                DateTime writeTime = File.GetLastWriteTime(_e.FullPath);
                if (writeTime.Subtract(fileListenerInfo.LastEventTime).TotalMilliseconds > 500)
                {
                    fileListenerInfo.LastEventTime = writeTime;
                    FileChangedEvent(_sender, new TestFileChangedEventArgs(_e.FullPath, TestFileChangedReason.Changed));
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool _disposing)
        {
            if (_disposing && fileListeners != null)
            {
                foreach (FileListenerInfo fileListenerInfo in fileListeners.Values)
                {
                    if (fileListenerInfo != null && fileListenerInfo.Watcher != null)
                    {
                        fileListenerInfo.Watcher.Changed -= OnChanged;
                        fileListenerInfo.Watcher.Dispose();
                    }
                }

                fileListeners.Clear();
                fileListeners = null;
            }
        }
    }
}
