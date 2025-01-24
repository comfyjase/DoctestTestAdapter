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

        private IDictionary<string, FileListenerInfo> fileWatchers;
        public event EventHandler<TestFileChangedEventArgs> FileChangedEvent;

        public TestFilesUpdateListener()
        {
            fileWatchers = new Dictionary<string, FileListenerInfo>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddWatch(string _path)
        {
            ValidateArg.NotNullOrEmpty(_path, "path");

            if (!String.IsNullOrEmpty(_path))
            {
                string directoryName = Path.GetDirectoryName(_path);
                string fileName = Path.GetFileName(_path);

                FileListenerInfo watcherInfo;
                if (!fileWatchers.TryGetValue(_path, out watcherInfo))
                {
                    watcherInfo = new FileListenerInfo(new FileSystemWatcher(directoryName, fileName));
                    fileWatchers.Add(_path, watcherInfo);

                    watcherInfo.Watcher.Changed += OnChanged;
                    watcherInfo.Watcher.EnableRaisingEvents = true;
                }
            }
        }

        public void RemoveWatch(string _path)
        {
            ValidateArg.NotNullOrEmpty(_path, "path");

            if (!String.IsNullOrEmpty(_path))
            {
                FileListenerInfo watcherInfo;
                if (fileWatchers.TryGetValue(_path, out watcherInfo))
                {
                    watcherInfo.Watcher.EnableRaisingEvents = false;

                    fileWatchers.Remove(_path);

                    watcherInfo.Watcher.Changed -= OnChanged;
                    watcherInfo.Watcher.Dispose();
                    watcherInfo.Watcher = null;
                }
            }
        }

        private void OnChanged(object _sender, FileSystemEventArgs _e)
        {
            FileListenerInfo watcherInfo;
            if (FileChangedEvent != null && fileWatchers.TryGetValue(_e.FullPath, out watcherInfo))
            {
                DateTime writeTime = File.GetLastWriteTime(_e.FullPath);
                if (writeTime.Subtract(watcherInfo.LastEventTime).TotalMilliseconds > 500)
                {
                    watcherInfo.LastEventTime = writeTime;
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
            if (_disposing && fileWatchers != null)
            {
                foreach (FileListenerInfo fileWatcher in fileWatchers.Values)
                {
                    if (fileWatcher != null && fileWatcher.Watcher != null)
                    {
                        fileWatcher.Watcher.Changed -= OnChanged;
                        fileWatcher.Watcher.Dispose();
                    }
                }

                fileWatchers.Clear();
                fileWatchers = null;
            }
        }
    }
}
