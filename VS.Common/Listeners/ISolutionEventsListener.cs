using System;

namespace VS.Common
{
    public interface ISolutionEventsListener
    {
        event EventHandler<SolutionEventsListenerEventArgs> SolutionProjectChanged;
        event EventHandler SolutionUnloaded;

        void StartListeningForChanges();
        void StopListeningForChanges();
    }
}
