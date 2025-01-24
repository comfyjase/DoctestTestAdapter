using System;

namespace VS.Common.DoctestTestAdapter
{
    public interface ITestFilesUpdateListener
    {
        event EventHandler<TestFileChangedEventArgs> FileChangedEvent;

        void AddWatch(string _path);
        void RemoveWatch(string _path);
    }
}
