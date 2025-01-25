using System;

namespace VS.Common.DoctestTestAdapter
{
    public interface ITestFilesUpdateListener
    {
        event EventHandler<TestFileChangedEventArgs> FileChangedEvent;

        void AddFileListener(string _path);
        void RemoveFileListener(string _path);
    }
}
