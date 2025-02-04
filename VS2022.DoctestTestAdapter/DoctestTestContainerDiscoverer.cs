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

        private System.IServiceProvider serviceProvider = null;
        private ISolutionEventsListener solutionListener = null;
        private ITestFilesUpdateListener testFilesUpdateListener = null;
        private ITestFileAddRemoveListener testFilesAddRemoveListener = null;
        private IVsSolution solution = null;

        private string solutionDirectory = "";
        private bool initialContainerSearch = true;
        private readonly List<ITestContainer> cachedContainers = new List<ITestContainer>();

        [ImportingConstructor]
        public DoctestTestContainerDiscoverer(
            [Import(typeof(SVsServiceProvider))] System.IServiceProvider _serviceProvider)
        {
            //TODO_comfyjase_02/02/2025: Remove this if you get timestamped log files working correctly...
            //Logger.Instance.Clear();

            Logger.Instance.WriteLine("Constructor called");

            if (VSUtilities.ShouldAttachDebugger())
            {
                Debugger.Launch();
            }

            serviceProvider = _serviceProvider;
            object service = serviceProvider.GetService(typeof(SVsSolution));
            if (service != null)
            {
                solution = service as IVsSolution;
                if (solution != null)
                {
                    solution.GetSolutionInfo(out solutionDirectory, out string solutionName, out string solutionDirectory2);
                }
            }
            //Debug.Assert(!String.IsNullOrEmpty(solutionDirectory), "Couldn't find solutionDirectory?");

            solutionListener = new SolutionEventsListener(serviceProvider);
            testFilesUpdateListener = new TestFilesUpdateListener();
            testFilesAddRemoveListener = new TestFileAddRemoveListener(serviceProvider);

            testFilesAddRemoveListener.TestFileChanged += OnProjectItemChanged;
            testFilesAddRemoveListener.StartListeningForTestFileChanges();

            solutionListener.SolutionOpened += SolutionListenerOnSolutionOpened;
            solutionListener.SolutionUnloaded += SolutionListenerOnSolutionUnloaded;
            solutionListener.SolutionProjectChanged += SolutionListenerOnProjectChanged;
            solutionListener.StartListeningForChanges();

            testFilesUpdateListener.FileChangedEvent += OnProjectItemChanged;
        }

        private void SolutionListenerOnSolutionOpened(object sender, EventArgs e)
        {
            Debug.Assert(serviceProvider != null);

            if (solution == null)
            {
                object service = serviceProvider.GetService(typeof(SVsSolution));
                if (service != null)
                {
                    solution = service as IVsSolution;
                    if (solution != null)
                    {
                        solution.GetSolutionInfo(out solutionDirectory, out string solutionName, out string solutionDirectory2);
                        Debug.Assert(!string.IsNullOrEmpty(solutionDirectory));

                        IEnumerable<string> files = FindTestFiles();
                        UpdateFileListener(files, true);
                    }
                }
            }
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

        private void SolutionListenerOnProjectChanged(object _sender, SolutionEventsListenerEventArgs _e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Logger.Instance.WriteLine("Begin");

            if (_e != null)
            {
                IEnumerable<string> files = FindTestFiles(_e.Project);
                if (_e.ChangedReason == SolutionChangedReason.Load)
                {
                    _e.Project.ToString();
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
            // Can immediately filter out folders.
            if (!Path.HasExtension(_path))
            {
                return false;
            }

            // Can also immediately filter out the doctest header itself.
            if (Path.GetFileName(_path).Equals("doctest.h", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            try
            {
                Debug.Assert(solution != null);
                solution.GetSolutionInfo(out solutionDirectory, out string solutionName, out string solutionDirectory2);
                Debug.Assert(!string.IsNullOrWhiteSpace(solutionDirectory)); 

                bool isProjectFile = _path.Contains(solutionDirectory);
                bool isDLLFile = (Path.GetExtension(_path).Equals(DoctestTestAdapterConstants.DLLFileExtension, StringComparison.OrdinalIgnoreCase));
                bool isExeFile = (Path.GetExtension(_path).Equals(DoctestTestAdapterConstants.ExeFileExtension, StringComparison.OrdinalIgnoreCase));
                bool isHFile = (Path.GetExtension(_path).Equals(DoctestTestAdapterConstants.HFileExtension, StringComparison.OrdinalIgnoreCase));
                bool isHPPFile = (Path.GetExtension(_path).Equals(DoctestTestAdapterConstants.HPPFileExtension, StringComparison.OrdinalIgnoreCase));
                bool containsDoctestTestCaseKeyword = File.ReadAllLines(_path).Any(s => s.Contains("TEST_CASE(\""));
                bool isCodeFile = (containsDoctestTestCaseKeyword && (isHFile || isHPPFile));

                bool isTestFile = (isProjectFile && (isDLLFile || isExeFile || isCodeFile));

                if (isTestFile)
                {
                    Logger.Instance.WriteLine("Found potential test file: " + _path);
                }

                return isTestFile;
            }
            catch (IOException e)
            {
                Logger.Instance.WriteLine("IO error when detecting a test file during Test Container Discovery " + e.Message);
            }

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
                    solutionListener.SolutionOpened -= SolutionListenerOnSolutionOpened;
                    solutionListener.SolutionProjectChanged -= SolutionListenerOnProjectChanged;
                    solutionListener.SolutionUnloaded -= SolutionListenerOnSolutionUnloaded;
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
