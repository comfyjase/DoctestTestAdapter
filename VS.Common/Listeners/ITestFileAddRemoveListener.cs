using System;

namespace VS.Common
{
    public interface ITestFileAddRemoveListener
    {
        event EventHandler<TestFileChangedEventArgs> TestFileChanged;

        void StartListeningForTestFileChanges();
        void StopListeningForTestFileChanges();
    }
}
