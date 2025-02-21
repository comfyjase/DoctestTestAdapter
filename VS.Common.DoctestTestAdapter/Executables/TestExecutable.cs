using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using VS.Common.DoctestTestAdapter.EventArgs;

namespace VS.Common.DoctestTestAdapter.Executables
{
    public class TestBatch
    {
        public List<TestCase> Tests
        {
            get; private set;
        }

        public string CommandArguments
        {
            get; private set;
        }

        public int BatchNumber
        {
            get; private set;
        }

        public TestBatch() : this(null, null, -1)
        { }

        public TestBatch(List<TestCase> _tests, string _commandArguments, int _batchNumber)
        {
            Tests = _tests;
            CommandArguments = _commandArguments;
            BatchNumber = _batchNumber;
        }
    }

    public class TestExecutable
    {
        public string FilePath
        {
            get { return filePath; }
        }
        private string filePath = string.Empty;
        
        public string CommandArguments
        {
            get { return commandArguments; }
            set { commandArguments = value; }
        }
        private string commandArguments = string.Empty;

        private string optionsFilePath = string.Empty;
        private IFrameworkHandle frameworkHandle = null;
        private System.Diagnostics.Process process = null;

        public List<TestCase> Tests
        {
            get { return tests; }
        }
        private List<TestCase> tests = new List<TestCase>();
        
        // In case we need to queue up tests for running later on
        // Specifically solves the situations where the command arguments are too long
        // So we need to be able to split up the tests into separate lists 
        private List<TestBatch> testBatches = new List<TestBatch>();

        private List<string> output = new List<string>();
        public event EventHandler<TestExecutableFinishedEventArgs> Finished = null;

        public TestExecutable() : this(null, null, null, null)
        {}

        public TestExecutable(string _filePath, string _optionsFilePath, IFrameworkHandle _frameworkHandle) : this(_filePath, null, _optionsFilePath, _frameworkHandle)
        {}

        public TestExecutable(string _filePath, string _commandArguments, string _optionsFilePath, IFrameworkHandle _frameworkHandle)
        {
            filePath = _filePath;
            commandArguments = _commandArguments;
            optionsFilePath = _optionsFilePath;
            frameworkHandle = _frameworkHandle;
        }

        public void AddTestCase(TestCase _test)
        {
            Debug.Assert(_test != null);
            tests.Add(_test);
        }

        public void AddTestCases(List<TestCase> _tests)
        {
            Debug.Assert(_tests != null);
            Debug.Assert(_tests.Count > 0);
            tests.AddRange(_tests);
        }

        public void AddTestBatch(List<TestCase> _tests, string _commandArguments)
        {
            Debug.Assert(_tests != null);
            Debug.Assert(_tests.Count > 0);
            testBatches.Add(new TestBatch(_tests, _commandArguments, testBatches.Count + 1));
        }

        private void RecordTestStart()
        {
            foreach (TestCase testCase in tests)
            {
                frameworkHandle.RecordStart(testCase);
            }
        }

        private void RecordTestFinish()
        {
            // failedTestFullErrorMessages will just read any test output lines that have the "ERROR: " string in it.
            // Regardless of which test case caused the error.
            // E.g. Path\To\TestFile.h(21): ERROR: CHECK( SomethingGoesWrongHere() ) is NOT correct!
            List<string> failedTestFullErrorMessages = output.Where(s => s.Contains(Constants.TestAdapter.TestResultErrorKeyword)).ToList();

            foreach (TestCase test in tests)
            {
                // testCaseErrorMessages just contains error messages relevant for the current test case.
                //
                // This below looks a little confusing I know... just some C# linq fun
                // failedTestErrorMessages is the sorted list of error messages, so it takes the full output line and just gets the specific error message:
                // E.g. CHECK( SomethingGoesWrongHere() ) is NOT correct!
                // This is done to populate the TestExplorer Error Message column with easier to read information that's more useful to explain WHY the test failed.
                List<string> testCaseErrorMessages = failedTestFullErrorMessages.Where(s => s.Contains(test.CodeFilePath)).ToList();
                List<string> failedTestErrorMessages = testCaseErrorMessages.Select(s =>
                    s.Substring(s.IndexOf(Constants.TestAdapter.TestResultErrorKeyword) + Constants.TestAdapter.TestResultErrorKeyword.Length, s.Length - (s.IndexOf(Constants.TestAdapter.TestResultErrorKeyword) + Constants.TestAdapter.TestResultErrorKeyword.Length)))
                    .ToList();

                string testName = test.DisplayName;
                bool testSkipped = VSUtilities.GetTestPropertyValue<bool>(test, Constants.TestAdapter.ShouldBeSkippedTestProperty);
                string testResultString = "";
                TestResult testResult = new TestResult(test);

                if (testSkipped)
                {
                    testResultString = "Skipped";
                    testResult.Outcome = TestOutcome.Skipped;
                }
                else
                {
                    bool testFailed = output.Any(s => s.Contains(testName));
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
                frameworkHandle.RecordResult(testResult);
            }
        }

        public void Start()
        {
            List<string> testCaseNames = tests.Select(t => t.DisplayName).ToList();

            process = new System.Diagnostics.Process();
            process.EnableRaisingEvents = true;

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.CreateNoWindow = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.Arguments = "/c " + filePath + " " + commandArguments;
            process.StartInfo = processStartInfo;

            process.Exited += OnProcessExited;
            Logger.Instance.WriteLine("Executable " + Path.GetFileName(process.StartInfo.FileName) + " subscribed to Exit event.");

            process.OutputDataReceived += (object _sender, DataReceivedEventArgs _e) =>
            {
                if (_e.Data != null && _e.Data.Count() > 0)
                {
                    Console.WriteLine(_e.Data);
                    output.Add(_e.Data + "\n");
                }
            };

            process.ErrorDataReceived += (object _sender, DataReceivedEventArgs _e) =>
            {
                if (_e.Data != null && _e.Data.Count() > 0)
                {
                    Console.WriteLine(_e.Data);
                }
            };

            Logger.Instance.WriteLine("About to start executable " + process.StartInfo.FileName + " with command arguments: " + commandArguments);

            RecordTestStart();

            // Start the executable now to run the doctests unit tests
            bool executableStartedSuccessfully = process.Start();
            Debug.Assert(executableStartedSuccessfully, "Failed to start " + process.StartInfo.FileName + " test executable");

            process.BeginOutputReadLine();
        }

        private void OnProcessExited(object _sender, System.EventArgs _e)
        {
            Logger.Instance.WriteLine("Begin");

            if (process != null)
            {
                process.Exited -= OnProcessExited;

                Logger.Instance.WriteLine("Test Executable: " + Path.GetFileName(process.StartInfo.FileName) + " finished.");

                process.Close();
                process = null;

                RecordTestFinish();

                Finished?.Invoke(this, new TestExecutableFinishedEventArgs(this));

                CheckIfAnyTestsAreLeftToRun();   
            }

            Logger.Instance.WriteLine("End");
        }

        private void CheckIfAnyTestsAreLeftToRun()
        {
            // If there are other test cases still to run, setup the test executable to run them next.
            if (testBatches.Count > 0)
            {
                // The current list of tests has finished, so clear it out so we can pass in whatever the next batch of tests.
                tests.Clear();

                // Pass information from the batch to the test executable to use and reset ready for running again.
                TestBatch testBatch = testBatches.First();
                tests = testBatch.Tests;
                commandArguments = testBatch.CommandArguments;
                output.Clear();

                // Remove this from the list because the test executable has got the information it needs.
                // So if there are multiple test batches to run, it won't re-run the same tests again.
                testBatches.Remove(testBatch);

                Logger.Instance.WriteLine("Test batch " + testBatch.BatchNumber + " is about to run with: " + Path.GetFileName(filePath));

                // Start up again now we have setup the new test batch to run.
                Start();
            }
        }
    }
}
