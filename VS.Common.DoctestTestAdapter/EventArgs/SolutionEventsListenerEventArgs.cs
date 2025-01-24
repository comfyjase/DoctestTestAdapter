using Microsoft.VisualStudio.Shell.Interop;

namespace VS.Common.DoctestTestAdapter
{
    public enum SolutionChangedReason
    {
        None,
        Load,
        Unload,
    }

    public class SolutionEventsListenerEventArgs : System.EventArgs
    {
        public IVsProject Project { get; private set; }
        public SolutionChangedReason ChangedReason { get; private set; }

        public SolutionEventsListenerEventArgs(IVsProject _project, SolutionChangedReason _reason)
        {
            Project = _project;
            ChangedReason = _reason;
        }
    }
}
