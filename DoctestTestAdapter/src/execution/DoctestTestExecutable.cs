using DoctestTestAdapter.Settings;
using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

using Helpers = DoctestTestAdapter.Shared.Helpers;

namespace DoctestTestAdapter.Execution
{
    internal class DoctestTestExecutable
    {
        public string FilePath
        {
            get { return _filePath; }
        }
        private string _filePath = string.Empty;

        private IRunContext _runContext = null;
        private IFrameworkHandle _frameworkHandle = null;
        private System.Diagnostics.Process _process = null;
        private List<DoctestTestBatch> _testBatches = new List<DoctestTestBatch>();
        public int NumberOfTestBatches
        {
            get
            {
                return _testBatches.Count;
            }
        }

        private DoctestTestBatch _currentTestBatch = null;
        private List<string> _output = new List<string>();

        public event EventHandler<EventArgs> Finished = null;

        public DoctestTestExecutable() : this(null, null, null)
        { }

        public DoctestTestExecutable(string filePath, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            _filePath = filePath;
            _runContext = runContext;
            _frameworkHandle = frameworkHandle;
        }

        public void AddTestBatch(List<TestCase> tests, string commandArguments)
        {
            _testBatches.Add(new DoctestTestBatch(tests, commandArguments, _testBatches.Count + 1));

            if (_currentTestBatch == null)
            {
                _currentTestBatch = _testBatches.First();
            }
        }

        private void RecordTestStart()
        {
            foreach (TestCase testCase in _currentTestBatch.Tests)
            {
                _frameworkHandle.RecordStart(testCase);
            }
        }

        private void RecordTestFinish()
        {
            // failedTestFullErrorMessages will just read any test output lines that have the "ERROR: " string in it.
            // Regardless of which test case caused the error.
            // E.g. Path\To\TestFile.h(21): ERROR: CHECK( SomethingGoesWrongHere() ) is NOT correct!
            List<string> failedTestFullErrorMessages = _output.Where(s => s.Contains(Helpers.Constants.TestResultErrorKeyword)).ToList();

            foreach (TestCase test in _currentTestBatch.Tests)
            {
                // testCaseErrorMessages just contains error messages relevant for the current test case.
                //
                // This below looks a little confusing I know... just some C# linq fun
                // failedTestErrorMessages is the sorted list of error messages, so it takes the full output line and just gets the specific error message:
                // E.g. CHECK( SomethingGoesWrongHere() ) is NOT correct!
                // This is done to populate the TestExplorer Error Message column with easier to read information that's more useful to explain WHY the test failed.
                List<string> testCaseErrorMessages = failedTestFullErrorMessages
                    .Where(s => s.Contains(test.CodeFilePath))
                    .ToList();
                List<string> failedTestErrorMessages = testCaseErrorMessages
                    .Select(s => s.Substring(s.IndexOf(Helpers.Constants.TestResultErrorKeyword) + Helpers.Constants.TestResultErrorKeyword.Length, s.Length - (s.IndexOf(Helpers.Constants.TestResultErrorKeyword) + Helpers.Constants.TestResultErrorKeyword.Length)))
                    .ToList();

                string testName = test.DisplayName;
                bool testSkipped = Utilities.GetTestCasePropertyValue<bool>(test, Helpers.Constants.ShouldBeSkippedTestProperty);
                TestResult testResult = new TestResult(test);

                if (testSkipped)
                {
                    testResult.Outcome = TestOutcome.Skipped;
                }
                else
                {
                    bool testFailed = _output.Any(s => s.Contains(testName));
                    if (testFailed)
                    {
                        testResult.Outcome = TestOutcome.Failed;
                        testResult.ErrorMessage = string.Join("", failedTestErrorMessages);
                    }
                    else
                    {
                        testResult.Outcome = TestOutcome.Passed;
                    }
                }

                _frameworkHandle.RecordResult(testResult);
            }
        }

        public void Start()
        {
            _output.Clear();

            List<string> testCaseNames = _currentTestBatch.Tests.Select(t => t.DisplayName).ToList();

            _process = new System.Diagnostics.Process();
            _process.EnableRaisingEvents = true;

            // Correct executable file path if needed.
            // Done in case a separate exe is generated but not by project output and is preferred for running tests against.
            // E.g. any .console.exe versions of a regular .exe file to run command line stuff.
            DoctestTestSettings settings = DoctestTestSettingsProvider.LoadSettings(_runContext);
            string testSource = FilePath;
            string solutionDirectory = Utilities.GetSolutionDirectory(Directory.GetParent(testSource).FullName);
            if (settings != null && settings.ExecutorSettings != null && settings.ExecutorSettings.ExecutableOverrides.Count > 0)
            {
                foreach (ExecutableOverride executableOverride in settings.ExecutorSettings.ExecutableOverrides)
                {
                    string key = executableOverride.Key;
                    string value = executableOverride.Value;
                    string keyFullPath = Path.Combine(solutionDirectory, key);

                    if (Path.GetFileName(testSource).Equals(Path.GetFileName(keyFullPath)))
                    {
                        testSource = Path.Combine(solutionDirectory, value);
                        break;
                    }
                }
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.CreateNoWindow = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.WorkingDirectory = solutionDirectory;
            processStartInfo.FileName = string.Format("\"{0}\"", testSource);
            processStartInfo.Arguments = _currentTestBatch.CommandArguments;
            _process.StartInfo = processStartInfo;

            _process.Exited += OnProcessExited;

            _process.OutputDataReceived += (object _sender, DataReceivedEventArgs _e) =>
            {
                if (_e.Data != null && _e.Data.Count() > 0)
                {
                    _output.Add(_e.Data + "\n");
                    Console.WriteLine(_e.Data);
                }
            };

            _process.ErrorDataReceived += (object _sender, DataReceivedEventArgs _e) =>
            {
                if (_e.Data != null && _e.Data.Count() > 0)
                {
                    Console.WriteLine(_e.Data);
                }
            };

            RecordTestStart();

            // Start the executable now to run the doctests unit tests
            bool executableStartedSuccessfully = _process.Start();

            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        private void OnProcessExited(object sender, System.EventArgs e)
        {
            if (_process != null)
            {
                _process.Exited -= OnProcessExited;
                _process.Close();
                _process = null;

                RecordTestFinish();

                Finished?.Invoke(this, EventArgs.Empty);

                CheckIfAnyTestsAreLeftToRun();
            }
        }

        private void CheckIfAnyTestsAreLeftToRun()
        {
            _testBatches.Remove(_testBatches.First());

            if (_testBatches.Count > 0)
            {
                _currentTestBatch = _testBatches.First();
                Start();
            }
        }

        public override string ToString()
        {
            string testBatchesString = string.Empty;
            _testBatches.ForEach(t => testBatchesString += ("\t" + t.ToString() + "\n"));
            return 
            (
                "Test Executable: " + FilePath + "\n"
                + testBatchesString
            );
        }
    }
}
