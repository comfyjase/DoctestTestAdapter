using System;

namespace VS.Common
{
    public interface ITestFilesUpdateListener
    {
        event EventHandler<TestFileChangedEventArgs> FileChangedEvent;

        void AddWatch(string path);
        void RemoveWatch(string path);
    }
}
