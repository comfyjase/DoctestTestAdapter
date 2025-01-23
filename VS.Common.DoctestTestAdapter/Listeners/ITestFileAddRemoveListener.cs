using System;

namespace VS.Common.DoctestTestAdapter
{
    public interface ITestFileAddRemoveListener
    {
        event EventHandler<TestFileChangedEventArgs> TestFileChanged;

        void StartListeningForTestFileChanges();
        void StopListeningForTestFileChanges();
    }
}
