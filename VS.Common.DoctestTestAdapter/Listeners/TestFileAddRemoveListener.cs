using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio;
using System;
using System.ComponentModel.Composition;

namespace VS.Common.DoctestTestAdapter
{
    [Export(typeof(ITestFileAddRemoveListener))]
    public sealed class TestFileAddRemoveListener : IVsTrackProjectDocumentsEvents2, IDisposable, ITestFileAddRemoveListener
    {
        private readonly IVsTrackProjectDocuments2 projectDocTracker;
        private uint cookie = VSConstants.VSCOOKIE_NIL;

        public event EventHandler<TestFileChangedEventArgs> TestFileChanged;

        [ImportingConstructor]
        public TestFileAddRemoveListener([Import(typeof(SVsServiceProvider))] IServiceProvider _serviceProvider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ValidateArg.NotNull(_serviceProvider, "serviceProvider");
            projectDocTracker = _serviceProvider.GetService(typeof(SVsTrackProjectDocuments)) as IVsTrackProjectDocuments2;
        }

        public void StartListeningForTestFileChanges()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (projectDocTracker != null)
            {
                int hr = projectDocTracker.AdviseTrackProjectDocumentsEvents(this, out cookie);
                ErrorHandler.ThrowOnFailure(hr);
            }
        }

        public void StopListeningForTestFileChanges()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (cookie != VSConstants.VSCOOKIE_NIL && projectDocTracker != null)
            {
                int hr = projectDocTracker.UnadviseTrackProjectDocumentsEvents(cookie);
                ErrorHandler.Succeeded(hr);
                cookie = VSConstants.VSCOOKIE_NIL;
            }
        }

        private int OnNotifyTestFileAddRemove(int _changedProjectCount, IVsProject[] _changedProjects, string[] _changedProjectItems, int[] _rgFirstIndices, TestFileChangedReason _reason)
        {
            int projectItemIndex = 0;
            for (int changeProjectIndex = 0; changeProjectIndex < _changedProjectCount; changeProjectIndex++)
            {
                int endProjectIndex = ((changeProjectIndex + 1) == _changedProjectCount) ? _changedProjectItems.Length : _rgFirstIndices[changeProjectIndex + 1];

                for (; projectItemIndex < endProjectIndex; projectItemIndex++)
                {
                    if (_changedProjects[changeProjectIndex] != null && TestFileChanged != null)
                    {
                        TestFileChanged(this, new TestFileChangedEventArgs(_changedProjectItems[projectItemIndex], _reason));
                    }

                }
            }
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterAddFilesEx(int _cProjects, int _cFiles, IVsProject[] _rgpProjects, int[] _rgFirstIndices, string[] _rgpszMkDocuments, VSADDFILEFLAGS[] _rgFlags)
        {
            return OnNotifyTestFileAddRemove(_cProjects, _rgpProjects, _rgpszMkDocuments, _rgFirstIndices, TestFileChangedReason.Added);
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterRemoveFiles(int _cProjects, int _cFiles, IVsProject[] _rgpProjects, int[] _rgFirstIndices, string[] _rgpszMkDocuments, VSREMOVEFILEFLAGS[] _rgFlags)
        {
            return OnNotifyTestFileAddRemove(_cProjects, _rgpProjects, _rgpszMkDocuments, _rgFirstIndices, TestFileChangedReason.Removed);
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterRenameFiles(int _cProjects, int _cFiles, IVsProject[] _rgpProjects, int[] _rgFirstIndices, string[] _rgszMkOldNames, string[] _rgszMkNewNames, VSRENAMEFILEFLAGS[] _rgFlags)
        {
            OnNotifyTestFileAddRemove(_cProjects, _rgpProjects, _rgszMkOldNames, _rgFirstIndices, TestFileChangedReason.Removed);
            return OnNotifyTestFileAddRemove(_cProjects, _rgpProjects, _rgszMkNewNames, _rgFirstIndices, TestFileChangedReason.Added);
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterAddDirectoriesEx(int _cProjects, int _cDirectories, IVsProject[] _rgpProjects, int[] _rgFirstIndices, string[] _rgpszMkDocuments, VSADDDIRECTORYFLAGS[] _rgFlags)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterRemoveDirectories(int _cProjects, int _cDirectories, IVsProject[] _rgpProjects, int[] _rgFirstIndices, string[] _rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] _rgFlags)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterRenameDirectories(int _cProjects, int _cDirs, IVsProject[] _rgpProjects, int[] _rgFirstIndices, string[] _rgszMkOldNames, string[] _rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] _rgFlags)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterSccStatusChanged(int _cProjects, int _cFiles, IVsProject[] _rgpProjects, int[] _rgFirstIndices, string[] _rgpszMkDocuments, uint[] _rgdwSccStatus)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryAddDirectories(IVsProject _pProject, int _cDirectories, string[] _rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] _rgFlags, VSQUERYADDDIRECTORYRESULTS[] _pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] _rgResults)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryAddFiles(IVsProject _pProject, int _cFiles, string[] _rgpszMkDocuments, VSQUERYADDFILEFLAGS[] _rgFlags, VSQUERYADDFILERESULTS[] _pSummaryResult, VSQUERYADDFILERESULTS[] _rgResults)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryRemoveDirectories(IVsProject _pProject, int _cDirectories, string[] _rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] _rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] _pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] _rgResults)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryRemoveFiles(IVsProject _pProject, int _cFiles, string[] _rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] _rgFlags, VSQUERYREMOVEFILERESULTS[] _pSummaryResult, VSQUERYREMOVEFILERESULTS[] _rgResults)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryRenameDirectories(IVsProject _pProject, int _cDirs, string[] _rgszMkOldNames, string[] _rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] _rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] _pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] _rgResults)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryRenameFiles(IVsProject _pProject, int _cFiles, string[] _rgszMkOldNames, string[] _rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] _rgFlags, VSQUERYRENAMEFILERESULTS[] _pSummaryResult, VSQUERYRENAMEFILERESULTS[] _rgResults)
        {
            return VSConstants.S_OK;
        }

        public void Dispose()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (disposing)
            {
                StopListeningForTestFileChanges();
            }
        }
    }
}
