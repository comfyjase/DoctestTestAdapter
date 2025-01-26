using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel.Composition;
using System.Diagnostics;
using VS.Common.DoctestTestAdapter;

namespace VS2022.DoctestTestAdapter
{
    [Export(typeof(ITestContainerDiscoverer))]
    public class DoctestTestContainerDiscoverer : ITestContainerDiscoverer
    {
        public Uri ExecutorUri => DoctestTestAdapterConstants.ExecutorUri;
        public IEnumerable<ITestContainer> TestContainers
        {
            get
            {
                //TODO_comfyjase_24/01/2025: Comment back in once the thread exception has been fixed to fix compiler warning VSTHRD010.
                //ThreadHelper.ThrowIfNotOnUIThread(); 
                return GetTestContainers();
            }
        }
        public event EventHandler TestContainersUpdated;

        private IServiceProvider serviceProvider = null;
        private ISolutionEventsListener solutionListener = null;
        private ITestFilesUpdateListener testFilesUpdateListener = null;
        private ITestFileAddRemoveListener testFilesAddRemoveListener = null;
        private IVsSolution solution = null;

        private string solutionDirectory = "";
        private bool initialContainerSearch = true;
        private readonly List<ITestContainer> cachedContainers = new List<ITestContainer>();

        [ImportingConstructor]
        public DoctestTestContainerDiscoverer(
            [Import(typeof(SVsServiceProvider))] IServiceProvider _serviceProvider)
        {
            Logger.Instance.WriteLine("Constructor called");

            if (VSUtilities.ShouldAttachDebugger())
            {
                Debugger.Launch();
            }

            serviceProvider = _serviceProvider;
            solution = serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
            solution.GetSolutionInfo(out solutionDirectory, out string solutionName, out string solutionDirectory2);
            Debug.Assert(!String.IsNullOrEmpty(solutionDirectory), "Couldn't find solutionDirectory?");

            solutionListener = new SolutionEventsListener(serviceProvider);
            testFilesUpdateListener = new TestFilesUpdateListener();
            testFilesAddRemoveListener = new TestFileAddRemoveListener(serviceProvider);

            testFilesAddRemoveListener.TestFileChanged += OnProjectItemChanged;
            testFilesAddRemoveListener.StartListeningForTestFileChanges();

            solutionListener.SolutionUnloaded += SolutionListenerOnSolutionUnloaded;
            solutionListener.SolutionProjectChanged += OnSolutionProjectChanged;
            solutionListener.StartListeningForChanges();

            testFilesUpdateListener.FileChangedEvent += OnProjectItemChanged;
        }


        private void OnTestContainersChanged()
        {
            Logger.Instance.WriteLine("Begin");

            if (TestContainersUpdated != null && !initialContainerSearch)
            {
                TestContainersUpdated(this, EventArgs.Empty);
            }

            Logger.Instance.WriteLine("End");
        }

        private void SolutionListenerOnSolutionUnloaded(object _sender, EventArgs _eventArgs)
        {
            Logger.Instance.WriteLine("Begin");

            initialContainerSearch = true;

            Logger.Instance.WriteLine("End");
        }

        private void OnSolutionProjectChanged(object _sender, SolutionEventsListenerEventArgs _e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Logger.Instance.WriteLine("Begin");

            if (_e != null)
            {
                IEnumerable<string> files = FindTestFiles(_e.Project);
                if (_e.ChangedReason == SolutionChangedReason.Load)
                {
                    UpdateFileListener(files, true);
                }
                else if (_e.ChangedReason == SolutionChangedReason.Unload)
                {
                    UpdateFileListener(files, false);
                }
            }

            Logger.Instance.WriteLine("End");
        }

        private void UpdateFileListener(IEnumerable<string> _files, bool _isAdd)
        {
            Logger.Instance.WriteLine("Begin");

            foreach (string file in _files)
            {
                if (_isAdd)
                {
                    testFilesUpdateListener.AddFileListener(file);
                    AddTestContainerIfTestFile(file);
                }
                else
                {
                    testFilesUpdateListener.RemoveFileListener(file);
                    RemoveTestContainer(file);
                }
            }

            Logger.Instance.WriteLine("End");
        }


        private void OnProjectItemChanged(object _sender, TestFileChangedEventArgs _e)
        {
            Logger.Instance.WriteLine("Begin");

            if (_e != null)
            {
                // Don't do anything for files we are sure can't be test files
                if (!IsTestFile(_e.File)) return;

                switch (_e.ChangedReason)
                {
                    case TestFileChangedReason.Added:
                        testFilesUpdateListener.AddFileListener(_e.File);
                        AddTestContainerIfTestFile(_e.File);

                        break;
                    case TestFileChangedReason.Removed:
                        testFilesUpdateListener.RemoveFileListener(_e.File);
                        RemoveTestContainer(_e.File);

                        break;
                    case TestFileChangedReason.Changed:
                        AddTestContainerIfTestFile(_e.File);
                        break;
                }

                OnTestContainersChanged();
            }

            Logger.Instance.WriteLine("End");
        }

        private void AddTestContainerIfTestFile(string _file)
        {
            Logger.Instance.WriteLine("Begin");

            bool isTestFile = IsTestFile(_file);
            RemoveTestContainer(_file);

            if (isTestFile)
            {
                DoctestTestContainer container = new DoctestTestContainer(this, _file, ExecutorUri);
                cachedContainers.Add(container);
            }

            Logger.Instance.WriteLine("End");
        }

        private void RemoveTestContainer(string _file)
        {
            Logger.Instance.WriteLine("Begin");

            int index = cachedContainers.FindIndex(x => x.Source.Equals(_file, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                cachedContainers.RemoveAt(index);
            }

            Logger.Instance.WriteLine("End");
        }

        private IEnumerable<ITestContainer> GetTestContainers()
        {
            //TODO_comfyjase_24/01/2025: Comment back in once the thread exception has been fixed to fix compiler warning VSTHRD010.
            //ThreadHelper.ThrowIfNotOnUIThread();

            Logger.Instance.WriteLine("Begin");

            if (initialContainerSearch)
            {
                cachedContainers.Clear();
                IEnumerable<string> testFiles = FindTestFiles();
                UpdateFileListener(testFiles, true);
                initialContainerSearch = false;
            }

            Logger.Instance.WriteLine("End");

            return cachedContainers;
        }

        private IEnumerable<string> FindTestFiles()
        {
            //TODO_comfyjase_24/01/2025: Comment back in once the thread exception has been fixed to fix compiler warning VSTHRD010.
            //ThreadHelper.ThrowIfNotOnUIThread();

            Logger.Instance.WriteLine("Begin");

            if (solution != null)
            {
                IEnumerable<IVsProject> loadedProjects = solution.EnumerateLoadedProjects(__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION).OfType<IVsProject>();

                Logger.Instance.WriteLine("End");

                return loadedProjects.SelectMany(FindTestFiles).ToList();
            }

            Logger.Instance.WriteLine("End");

            return Enumerable.Empty<string>();
        }

        private IEnumerable<string> FindTestFiles(IVsProject _project)
        {
            //TODO_comfyjase_24/01/2025: Comment back in once the thread exception has been fixed to fix compiler warning VSTHRD010.
            //ThreadHelper.ThrowIfNotOnUIThread();

            Logger.Instance.WriteLine("Begin");

            IEnumerable<string> testFiles = (from item in VSUtilities.GetProjectItems(_project)
                    where IsTestFile(item)
                    select item); 
            
            Logger.Instance.WriteLine("End");

            return testFiles;
        }

        private bool IsTestFile(string _path)
        {
            Logger.Instance.WriteLine("Begin");

            try
            {
                bool isProjectFile = _path.Contains(solutionDirectory);
                bool isDLLFile = (Path.GetExtension(_path).Equals(DoctestTestAdapterConstants.DLLFileExtension, StringComparison.OrdinalIgnoreCase));
                bool isExeFile = (Path.GetExtension(_path).Equals(DoctestTestAdapterConstants.ExeFileExtension, StringComparison.OrdinalIgnoreCase));
                bool isHFile = (Path.GetExtension(_path).Equals(DoctestTestAdapterConstants.HFileExtension, StringComparison.OrdinalIgnoreCase));
                bool isHPPFile = (Path.GetExtension(_path).Equals(DoctestTestAdapterConstants.HPPFileExtension, StringComparison.OrdinalIgnoreCase));

                bool isTestFile = (isProjectFile && (isDLLFile || isExeFile || isHFile || isHPPFile));

                if (isTestFile)
                {
                    Logger.Instance.WriteLine("Found potential test file: " + _path, 1);
                }

                Logger.Instance.WriteLine("End");
                return isTestFile;
            }
            catch (IOException e)
            {
                Logger.Instance.WriteLine("IO error when detecting a test file during Test Container Discovery" + e.Message);
            }

            Logger.Instance.WriteLine("End");
            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool _disposing)
        {
            if (_disposing)
            {
                Logger.Instance.WriteLine("Begin");

                if (testFilesUpdateListener != null)
                {
                    testFilesUpdateListener.FileChangedEvent -= OnProjectItemChanged;
                    ((IDisposable)testFilesUpdateListener).Dispose();
                    testFilesUpdateListener = null;
                }

                if (testFilesAddRemoveListener != null)
                {
                    testFilesAddRemoveListener.TestFileChanged -= OnProjectItemChanged;
                    testFilesAddRemoveListener.StopListeningForTestFileChanges();
                    testFilesAddRemoveListener = null;
                }

                if (solutionListener != null)
                {
                    solutionListener.SolutionProjectChanged -= OnSolutionProjectChanged;
                    solutionListener.StopListeningForChanges();
                    solutionListener = null;
                }

                Logger.Instance.WriteLine("End");

                // TODO: Is this the right place?
                Logger.Instance.Dispose();
            }
        }
    }
}
