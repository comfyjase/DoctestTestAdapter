using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
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
        private bool waitingForTestResults = false;
        private int totalNumberOfExecutables = 0;
        private int currentNumberOfRunningExecutables = 0;

        List<System.Diagnostics.Process> processList = new List<System.Diagnostics.Process>();
        Dictionary<string, List<TestCase>> mappedExecutableTests = new Dictionary<string, List<TestCase>>();
        Dictionary<string, List<string>> mappedTestOutputs = new Dictionary<string, List<string>>();
        Dictionary<string, EventHandler> mappedExitHandlers = new Dictionary<string, EventHandler>();

        public void RunTests(IEnumerable<TestCase> _tests, IRunContext _runContext, IFrameworkHandle _frameworkHandle)
        {
            Logger.Instance.WriteLine("Begin");

            waitingForTestResults = true;
            //allTestsHaveStarted = true;
            cancelled = false;
            processList.Clear();
            mappedExecutableTests.Clear();
            mappedTestOutputs.Clear();
            mappedExitHandlers.Clear();

            DoctestSettingsProvider doctestSettings = _runContext.RunSettings.GetSettings(DoctestTestAdapterConstants.SettingsName) as DoctestSettingsProvider;
            foreach (TestCase test in _tests)
            {
                if (cancelled)
                {
                    return;
                }

                _frameworkHandle.RecordStart(test);

                string executableFilePath = DoctestTestAdapterUtilities.GetTestFileExecutableFilePath(doctestSettings, test.CodeFilePath);

                ////TODO_comfyjase_29/01/2025: Find a way to link dll -> related exe file, in the meantime, mark it as skipped
                //if (Path.GetExtension(executableFilePath).Equals(DoctestTestAdapterConstants.DLLFileExtension, StringComparison.OrdinalIgnoreCase))
                //{
                //    TestResult testResult = new TestResult(test);
                //    testResult.Outcome = TestOutcome.Skipped;
                //    _frameworkHandle.RecordResult(testResult);
                //    continue;
                //}    

                if (mappedExecutableTests.TryGetValue(executableFilePath, out List<TestCase> testFiles))
                {
                    testFiles.Add(test);
                    mappedExecutableTests[executableFilePath] = testFiles;
                }
                else
                {
                    List<TestCase> newTestCaseList = new List<TestCase>{ test };
                    mappedExecutableTests.Add(executableFilePath, newTestCaseList);
                }
            }

            totalNumberOfExecutables = mappedExecutableTests.Count;
            currentNumberOfRunningExecutables = totalNumberOfExecutables;

            foreach (KeyValuePair<string, List<TestCase>> testSetup in mappedExecutableTests)
            {
                if (cancelled)
                {
                    return;
                }

                List<string> testCaseNames = testSetup.Value.Select(t => t.DisplayName).ToList();

                System.Diagnostics.Process testExecutable = new System.Diagnostics.Process();
                processList.Add(testExecutable);

                testExecutable.EnableRaisingEvents = true;
                testExecutable.StartInfo.CreateNoWindow = true;
                testExecutable.StartInfo.RedirectStandardOutput = true;
                testExecutable.StartInfo.RedirectStandardError = true;
                testExecutable.StartInfo.UseShellExecute = false;

                testExecutable.StartInfo.FileName = testSetup.Key;

                // Should be something like: "[TestDecorator] Test 1, [TestDecorator] Test 2"
                string commaSeparatedListOfTestCaseNames = string.Join(",", testCaseNames);

                // Sorted into doctest specific argument formatting: "*"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"*"
                string doctestTestCaseCommandArgument = "--test-case=" + string.Join(",", commaSeparatedListOfTestCaseNames.Split(',').Select(x => string.Format("*\"{0}\"*", x)).ToList());

                // Full doctest arguments: --test-case="*"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"*"
                string doctestArguments = doctestTestCaseCommandArgument;

                testExecutable.StartInfo.Arguments = doctestArguments;

                EventHandler testExecutableExitEventHandler = (_sender, _e) => OnTestExecutableFinished(_sender, _e, _runContext, _frameworkHandle, testExecutable);
                testExecutable.Exited += testExecutableExitEventHandler;
                mappedExitHandlers.Add(testSetup.Key, testExecutableExitEventHandler);
                Logger.Instance.WriteLine("Executable " + testSetup.Key + " subscribed to Exit event.");

                testExecutable.OutputDataReceived += (object _sender, DataReceivedEventArgs _e) =>
                {
                    if (_e.Data != null && _e.Data.Count() > 0)
                    {
                        Console.WriteLine(_e.Data);

                        //Logger.Instance.WriteLine(_e.Data);
                        if (mappedTestOutputs.TryGetValue(testSetup.Key, out List<string> outputStrings))
                        {
                            outputStrings.Add(_e.Data + "\n");
                            mappedTestOutputs[testSetup.Key] = outputStrings;
                        }
                        else
                        {
                            List<string> newOutputStringsList = new List<String> { (_e.Data + "\n") };
                            mappedTestOutputs.Add(testSetup.Key, newOutputStringsList);
                        }
                    }
                };

                Logger.Instance.WriteLine("About to start executable " + testExecutable.StartInfo.FileName + " with command arguments: " + testExecutable.StartInfo.Arguments);

                // Start the executable now to run the doctests unit tests
                bool executableStartedSuccessfully = testExecutable.Start();
                Debug.Assert(executableStartedSuccessfully, "Failed to start " + testExecutable.StartInfo.FileName + " test executable");

                testExecutable.BeginOutputReadLine();
            }

            // TEMP TEST...
            while (waitingForTestResults)
            {
                System.Threading.Thread.Sleep(100);
                //Thread.Sleep(checksInterval);
            }

            Logger.Instance.WriteLine("End");
        }

        private void OnTestExecutableFinished(object _sender, EventArgs _e, IRunContext _runContext, IFrameworkHandle _frameworkHandle, System.Diagnostics.Process _testExecutable)
        {
            Logger.Instance.WriteLine("Begin");

            if (_testExecutable != null)
            {
                if (mappedExitHandlers.TryGetValue(_testExecutable.StartInfo.FileName, out EventHandler exitEventHandler))
                {
                    currentNumberOfRunningExecutables--;

                    _testExecutable.Exited -= exitEventHandler;
                    mappedExitHandlers.Remove(_testExecutable.StartInfo.FileName);

                    Logger.Instance.WriteLine("Test Executable: " + Path.GetFileName(_testExecutable.StartInfo.FileName) + " finished.");

                    _testExecutable.Close();
                    _testExecutable = null;

                    if (currentNumberOfRunningExecutables > 0)
                    {
                        Logger.Instance.WriteLine("Waiting for " + currentNumberOfRunningExecutables + " executables...");
                    }
                    else
                    {
                        Logger.Instance.WriteLine("No more test executables to run...");
                    }

                    // No more test executables to run.
                    if (currentNumberOfRunningExecutables == 0)
                    {
                        OnAllTestExecutablesFinished(_frameworkHandle);
                    }
                }
            }

            Logger.Instance.WriteLine("End");
        }

        private void OnAllTestExecutablesFinished(IFrameworkHandle _frameworkHandle)
        {
            Logger.Instance.WriteLine("All test executables have finished now.");

            foreach (KeyValuePair<string, List<TestCase>> mappedTests in mappedExecutableTests)
            {
                bool mappedOutputExists = mappedTestOutputs.TryGetValue(mappedTests.Key, out List<string> testOutput);
                Debug.Assert(mappedOutputExists, "No mapped output for " + Path.GetFileName(mappedTests.Key));

                if (mappedOutputExists)
                {
                    // This below looks a little confusing I know... just some C# linq fun
                    // failedTestFullErrorMessages will just read any test output lines that have the "ERROR: " string in it.
                    // E.g. Path\To\TestFile.h(21): ERROR: CHECK( SomethingGoesWrongHere() ) is NOT correct!
                    //
                    // failedTestErrorMessages is the sorted list of error messages, so it takes the full output line and just gets the specific error message:
                    // E.g. CHECK( SomethingGoesWrongHere() ) is NOT correct!
                    // This is done to populate the TestExplorer Error Message column with easier to read information that's more useful to explain WHY the test failed.
                    List<string> failedTestFullErrorMessages = testOutput.Where(s => s.Contains(DoctestTestAdapterConstants.TestResultErrorKeyword)).ToList();
                    List<string> failedTestErrorMessages = failedTestFullErrorMessages.Select(s =>
                        s.Substring(s.IndexOf(DoctestTestAdapterConstants.TestResultErrorKeyword) + DoctestTestAdapterConstants.TestResultErrorKeyword.Length, s.Length - (s.IndexOf(DoctestTestAdapterConstants.TestResultErrorKeyword) + DoctestTestAdapterConstants.TestResultErrorKeyword.Length)))
                        .ToList();

                    foreach (TestCase test in mappedTests.Value)
                    {
                        string testName = test.DisplayName;
                        bool testSkipped = DoctestTestAdapterUtilities.GetTestPropertyValue<bool>(test, DoctestTestAdapterConstants.ShouldBeSkippedTestProperty);
                        string testResultString = "";
                        TestResult testResult = new TestResult(test);

                        if (testSkipped)
                        {
                            testResultString = "Skipped";
                            testResult.Outcome = TestOutcome.Skipped;
                        }
                        else
                        {
                            bool testFailed = testOutput.Any(s => s.Contains(testName));
                            if (testFailed)
                            {
                                testResultString = "Failed";
                                testResult.Outcome = TestOutcome.Failed;
                                testResult.ErrorMessage = string.Join("", failedTestErrorMessages);
                            }
                            else
                            {
                                testResultString = "Passed";
                                testResult.Outcome = TestOutcome.Passed;
                            }
                        }

                        Logger.Instance.WriteLine("Recording result " + testResultString + " for test " + test.DisplayName);
                        _frameworkHandle.RecordResult(testResult);
                    }
                }
            }

            processList.Clear();
            waitingForTestResults = false;
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
            processList.Clear();

            Logger.Instance.WriteLine("End");
        }
    }
}
