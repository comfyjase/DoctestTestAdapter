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
            public FileListenerInfo(FileSystemWatcher watcher)
            {
                Watcher = watcher;
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

        public void AddWatch(string path)
        {
            ValidateArg.NotNullOrEmpty(path, "path");

            if (!String.IsNullOrEmpty(path))
            {
                string directoryName = Path.GetDirectoryName(path);
                string fileName = Path.GetFileName(path);

                FileListenerInfo watcherInfo;
                if (!fileWatchers.TryGetValue(path, out watcherInfo))
                {
                    watcherInfo = new FileListenerInfo(new FileSystemWatcher(directoryName, fileName));
                    fileWatchers.Add(path, watcherInfo);

                    watcherInfo.Watcher.Changed += OnChanged;
                    watcherInfo.Watcher.EnableRaisingEvents = true;
                }
            }
        }

        public void RemoveWatch(string path)
        {
            ValidateArg.NotNullOrEmpty(path, "path");

            if (!String.IsNullOrEmpty(path))
            {
                FileListenerInfo watcherInfo;
                if (fileWatchers.TryGetValue(path, out watcherInfo))
                {
                    watcherInfo.Watcher.EnableRaisingEvents = false;

                    fileWatchers.Remove(path);

                    watcherInfo.Watcher.Changed -= OnChanged;
                    watcherInfo.Watcher.Dispose();
                    watcherInfo.Watcher = null;
                }
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            FileListenerInfo watcherInfo;
            if (FileChangedEvent != null && fileWatchers.TryGetValue(e.FullPath, out watcherInfo))
            {
                DateTime writeTime = File.GetLastWriteTime(e.FullPath);
                if (writeTime.Subtract(watcherInfo.LastEventTime).TotalMilliseconds > 500)
                {
                    watcherInfo.LastEventTime = writeTime;
                    FileChangedEvent(sender, new TestFileChangedEventArgs(e.FullPath, TestFileChangedReason.Changed));
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && fileWatchers != null)
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
