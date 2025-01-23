using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.ComponentModel.Composition;

namespace VS.Common
{
    [Export(typeof(ISolutionEventsListener))]
    public class SolutionEventsListener : IVsSolutionEvents, ISolutionEventsListener
    {
        private readonly IVsSolution solution;
        private uint cookie = VSConstants.VSCOOKIE_NIL;

        public event EventHandler<SolutionEventsListenerEventArgs> SolutionProjectChanged;
        public event EventHandler SolutionUnloaded;

        [ImportingConstructor]
        public SolutionEventsListener([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ValidateArg.NotNull(serviceProvider, "serviceProvider");
            solution = serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
        }

        public void StartListeningForChanges()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (solution != null)
            {
                int hr = solution.AdviseSolutionEvents(this, out cookie);
                ErrorHandler.ThrowOnFailure(hr);
            }
        }

        public void StopListeningForChanges()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (cookie != VSConstants.VSCOOKIE_NIL && solution != null)
            {
                int hr = solution.UnadviseSolutionEvents(cookie);
                ErrorHandler.Succeeded(hr);
                cookie = VSConstants.VSCOOKIE_NIL;
            }
        }

        public void OnSolutionProjectUpdated(IVsProject project, SolutionChangedReason reason)
        {
            if (SolutionProjectChanged != null && project != null)
            {
                SolutionProjectChanged(this, new SolutionEventsListenerEventArgs(project, reason));
            }
        }

        public void OnSolutionUnloaded()
        {
            if (SolutionUnloaded != null)
            {
                SolutionUnloaded(this, new System.EventArgs());
            }
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IVsProject project = pRealHierarchy as IVsProject;
            OnSolutionProjectUpdated(project, SolutionChangedReason.Load);
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IVsProject project = pRealHierarchy as IVsProject;
            OnSolutionProjectUpdated(project, SolutionChangedReason.Unload);
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            OnSolutionUnloaded();
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }
    }
}
