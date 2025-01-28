using EnvDTE;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using VS.Common.DoctestTestAdapter;
using VS2022.DoctestTestAdapter.Settings;

namespace VS2022.DoctestTestAdapter
{
    [ExtensionUri(DoctestTestAdapterConstants.ExecutorUriString)]
    public class DoctestTestExecutor : ITestExecutor
    {
        private bool cancelled = false;

        private System.Diagnostics.Process testExecutable = null;
        private EventHandler testExecutableExitEventHandler = null;
        private List<String> testExecutableFullOutput = new List<string>();
        private List<String> testCodeFileNames = new List<String>();

        private void CacheTestFileNames(IEnumerable<TestCase> _tests)
        {
            // If this list is still full from the last run, make sure to clear it first before populating again.
            if (testCodeFileNames.Count > 0)
            {
                testCodeFileNames.Clear();
            }

            foreach (TestCase test in _tests)
            {
                if (cancelled)
                    break;

                testCodeFileNames.Add(Path.GetFileNameWithoutExtension(test.CodeFilePath));
            }
        }

        private void RunDoctestTests(IEnumerable<TestCase> _tests, IRunContext _runContext, IFrameworkHandle _frameworkHandle)
        {
            Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter.IRunSettings runSettings = _runContext.RunSettings;
            if (runSettings != null)
            {
                DoctestSettingsProvider doctestSettingsProvider = runSettings.GetSettings(DoctestTestAdapterConstants.SettingsName) as DoctestSettingsProvider;
                Debug.Assert(doctestSettingsProvider != null);

                Debug.Assert(_tests.ToArray().Length > 0, "Should have at least one test selected...");
                string executableFilePath = DoctestTestAdapterUtilities.GetTestFileExecutableFilePath(doctestSettingsProvider, _tests.First().CodeFilePath, out string commandArguments);

                //TODO_comfyjase_28/01/2025: Work out how to make connection between dll and exe files... just skip for now.
                if (Path.GetExtension(executableFilePath).Equals(DoctestTestAdapterConstants.DLLFileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (TestCase test in _tests)
                    {
                        TestResult testResult = new TestResult(test);
                        testResult.Outcome = TestOutcome.Skipped;
                        _frameworkHandle.RecordResult(testResult);
                    }
                    return;
                }

                testExecutable = new System.Diagnostics.Process();

                testExecutable.EnableRaisingEvents = true;
                testExecutable.StartInfo.CreateNoWindow = true;
                testExecutable.StartInfo.RedirectStandardOutput = true;
                testExecutable.StartInfo.RedirectStandardError = true;
                testExecutable.StartInfo.UseShellExecute = false;

                testExecutable.StartInfo.FileName = executableFilePath;

                // Should be something like: "TestFile, TestFile2"
                string commaSeparatedListOfTestFiles = string.Join(",", testCodeFileNames);

                // Sorted into doctest specific argument formatting: "*TestFile1*,*TestFile2*"
                string doctestSourceFileArgument = string.Join(",", commaSeparatedListOfTestFiles.Split(',').Select(x => string.Format("*{0}*", x)).ToList());

                // Full doctest arguments: --source-file="*TestFile1*,*TestFile2*" --success=true
                string doctestArguments = "--source-file=" + doctestSourceFileArgument + " --success=true";

                testExecutable.StartInfo.Arguments = (commandArguments.Length > 0 ? (commandArguments + " " + doctestArguments) : doctestArguments);

                // Start the executable now to run the doctests unit tests
                bool executableStartedSuccessfully = testExecutable.Start();
                Debug.Assert(executableStartedSuccessfully, "Failed to start " + executableFilePath + " test executable");

                // Reset output strings for this test run
                testExecutableFullOutput.Clear();

                testExecutable.OutputDataReceived += (object _sender, DataReceivedEventArgs _e) =>
                {
                    if (_e.Data != null && _e.Data.Count() > 0)
                    {
                        Logger.Instance.WriteLine(_e.Data);
                        testExecutableFullOutput.Add(_e.Data + "\n");
                    }
                };
                testExecutable.BeginOutputReadLine();

                testExecutableExitEventHandler = (_sender, _e) => OnTestExecutableFinished(_sender, _e, _tests, _runContext, _frameworkHandle);
                testExecutable.Exited += testExecutableExitEventHandler;
            }
        }

        private void OnTestExecutableFinished(object _sender, EventArgs _e, IEnumerable<TestCase> _tests, IRunContext _runContext, IFrameworkHandle _frameworkHandle)
        {
            if (testExecutable != null)
            {
                foreach (string outputString in testExecutableFullOutput)
                {
                    //TODO_comfyjase_28/01/2025: read output and find any output relevant to the _tests?
                    // Create new test result and record it into the _frameworkHandle

                }

                testExecutable.Exited -= testExecutableExitEventHandler;
                testExecutable.Close();
                testExecutable = null;
            }
        }

        public void RunTests(IEnumerable<TestCase> _tests, IRunContext _runContext, IFrameworkHandle _frameworkHandle)
        {
            Logger.Instance.WriteLine("Begin");

            cancelled = false;

            CacheTestFileNames(_tests);
            RunDoctestTests(_tests, _runContext, _frameworkHandle);
            
            Logger.Instance.WriteLine("End");
        }

        public void RunTests(IEnumerable<string> _sources, IRunContext _runContext, IFrameworkHandle _frameworkHandle)
        {
            Logger.Instance.WriteLine("Begin");

            IDiscoveryContext discoveryContext = _runContext;
            IMessageLogger logger = _frameworkHandle;
            ITestCaseDiscoverySink discoverySink = null;
            IEnumerable<TestCase> tests = DoctestTestAdapterUtilities.GetTests(_sources, discoveryContext, logger, discoverySink);
            RunTests(tests, _runContext, _frameworkHandle);

            Logger.Instance.WriteLine("End");
        }

        public void Cancel()
        {
            Logger.Instance.WriteLine("Begin");

            cancelled = true;
            
            Logger.Instance.WriteLine("End");
        }
    }
}
