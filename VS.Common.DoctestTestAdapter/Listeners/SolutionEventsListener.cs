using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.ComponentModel.Composition;

namespace VS.Common.DoctestTestAdapter
{
    [Export(typeof(ISolutionEventsListener))]
    public class SolutionEventsListener : IVsSolutionEvents, ISolutionEventsListener
    {
        private readonly IVsSolution solution = null;
        private uint cookie = VSConstants.VSCOOKIE_NIL;

        public event EventHandler SolutionOpened;
        public event EventHandler<SolutionEventsListenerEventArgs> SolutionProjectChanged;
        public event EventHandler SolutionUnloaded;

        [ImportingConstructor]
        public SolutionEventsListener([Import(typeof(SVsServiceProvider))] IServiceProvider _serviceProvider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ValidateArg.NotNull(_serviceProvider, "serviceProvider");
            solution = _serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
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

        public void OnSolutionProjectUpdated(IVsProject _project, SolutionChangedReason _reason)
        {
            if (SolutionProjectChanged != null && _project != null)
            {
                SolutionProjectChanged(this, new SolutionEventsListenerEventArgs(_project, _reason));
            }
        }

        public void OnSolutionUnloaded()
        {
            if (SolutionUnloaded != null)
            {
                SolutionUnloaded(this, new System.EventArgs());
            }
        }

        public int OnAfterLoadProject(IVsHierarchy _pStubHierarchy, IVsHierarchy _pRealHierarchy)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IVsProject project = _pRealHierarchy as IVsProject;
            OnSolutionProjectUpdated(project, SolutionChangedReason.Load);
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy _pRealHierarchy, IVsHierarchy _pStubHierarchy)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IVsProject project = _pRealHierarchy as IVsProject;
            OnSolutionProjectUpdated(project, SolutionChangedReason.Unload);
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object _pUnkReserved)
        {
            OnSolutionUnloaded();
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy _pHierarchy, int _fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object _pUnkReserved, int _fNewSolution)
        {
            SolutionOpened?.Invoke(this, System.EventArgs.Empty);
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy _pHierarchy, int _fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object _pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy _pHierarchy, int _fRemoving, ref int _pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object _pUnkReserved, ref int _pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy _pRealHierarchy, ref int _pfCancel)
        {
            return VSConstants.S_OK;
        }
    }
}
