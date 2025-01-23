using System;

namespace VS.Common.DoctestTestAdapter
{
    public interface ISolutionEventsListener
    {
        event EventHandler<SolutionEventsListenerEventArgs> SolutionProjectChanged;
        event EventHandler SolutionUnloaded;

        void StartListeningForChanges();
        void StopListeningForChanges();
    }
}
