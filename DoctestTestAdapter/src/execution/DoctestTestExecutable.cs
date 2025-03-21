using DoctestTestAdapter.Settings;
using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

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
        private List<TestCase> _allTestCases = new List<TestCase>();
        public List<TestCase> AllTestCases
        {
            get
            {
                return _allTestCases;
            }
        }

        private List<DoctestTestBatch> _testBatches = new List<DoctestTestBatch>();
        public int NumberOfTestBatches
        {
            get
            {
                return _testBatches.Count;
            }
        }

        private DoctestTestBatch _currentTestBatch = null;

        public event EventHandler<EventArgs> Finished = null;

        public DoctestTestExecutable(string filePath, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            Utilities.CheckString(filePath, nameof(filePath));
            Utilities.CheckNull(runContext, nameof(runContext));
            Utilities.CheckNull(frameworkHandle, nameof(frameworkHandle));

            _filePath = filePath;
            _runContext = runContext;
            _frameworkHandle = frameworkHandle;
        }

        public void TrackTestCase(TestCase testCase)
        {
            Utilities.CheckNull(testCase, nameof(testCase));

            if (!_allTestCases.Contains(testCase))
                _allTestCases.Add(testCase);
        }

        public void AddTestBatch(List<TestCase> tests, string commandArguments)
        {
            string batchTestReportFilePath = Directory.GetParent(_filePath).FullName + "\\" + Path.GetFileNameWithoutExtension(_filePath) + "_TestReport_" + (_testBatches.Count + 1).ToString() + ".xml";
            _testBatches.Add(new DoctestTestBatch(tests, commandArguments, _testBatches.Count + 1, batchTestReportFilePath));

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
            Dictionary<TestCase, bool> reportedTestResults = new Dictionary<TestCase, bool>();

            if (!File.Exists(_currentTestBatch.TestReportFilePath))
                throw new FileNotFoundException($"Could not find file {_currentTestBatch.TestReportFilePath}, abort!");

            XmlDocument testReportDocument = new XmlDocument();
            testReportDocument.Load(_currentTestBatch.TestReportFilePath);

            XmlNodeList testCaseNodes = testReportDocument.SelectNodes("//doctest/TestSuite/TestCase");

            foreach (XmlNode testCaseNode in testCaseNodes)
            {
                XmlAttribute nameAttribute = testCaseNode.Attributes["name"];
                XmlAttribute fileNameAttribute = testCaseNode.Attributes["filename"];
                XmlAttribute lineNumberAttribute = testCaseNode.Attributes["line"];
                if (nameAttribute != null && !string.IsNullOrWhiteSpace(nameAttribute.Value)
                    && fileNameAttribute != null && !string.IsNullOrEmpty(fileNameAttribute.Value)
                    && lineNumberAttribute != null && !string.IsNullOrEmpty(lineNumberAttribute.Value) && int.TryParse(lineNumberAttribute.Value, out int lineNumber))
                {
                    string testCaseNameFromReport = nameAttribute.Value;
                    string testCaseFileNameFromReport = fileNameAttribute.Value;

                    TestCase testCaseFromReport = _currentTestBatch.Tests.Find(t => 
                        (
                            t.DisplayName.Equals(testCaseNameFromReport)
                            && t.CodeFilePath.Equals(testCaseFileNameFromReport.Replace("/", "\\"))
                            && t.LineNumber == lineNumber
                        ));

                    if (testCaseFromReport != null)
                    {
                        // TODO: Test if you still need this...
                        if (reportedTestResults.TryGetValue(testCaseFromReport, out bool alreadyReported))
                        {
                            if (alreadyReported)
                            {
                                continue;
                            }
                        }

                        TestResult testResult = new TestResult(testCaseFromReport);
                        
                        // Skipped.
                        XmlAttribute skippedAttribute = testCaseNode.Attributes["skipped"];
                        if (skippedAttribute != null && !string.IsNullOrEmpty(skippedAttribute.Value))
                        {
                            testResult.Outcome = TestOutcome.Skipped;
                        }
                        else
                        {
                            XmlNode resultsNode = testCaseNode.SelectSingleNode("OverallResultsAsserts");
                            if (resultsNode != null)
                            {
                                XmlAttribute durationAttribute = resultsNode.Attributes["duration"];
                                if (durationAttribute != null && !string.IsNullOrEmpty(durationAttribute.Value))
                                {
                                    if (float.TryParse(durationAttribute.Value, out float testDurationInSeconds))
                                    {
                                        testResult.Duration = TimeSpan.FromSeconds(testDurationInSeconds);
                                    }
                                }

                                XmlAttribute testCaseSuccessAttribute = resultsNode.Attributes["test_case_success"];
                                if (testCaseSuccessAttribute != null && !string.IsNullOrEmpty(testCaseSuccessAttribute.Value))
                                {
                                    // Passed.
                                    if (testCaseSuccessAttribute.Value.Equals("true"))
                                    {
                                        testResult.Outcome = TestOutcome.Passed;
                                    }
                                    // Failed.
                                    else
                                    {
                                        testResult.Outcome = TestOutcome.Failed;

                                        string errorMessage = string.Empty;

                                        XmlNodeList expressionNodes = testCaseNode.SelectNodes("Expression");
                                        foreach (XmlNode expressionNode in expressionNodes)
                                        {
                                            XmlAttribute typeAttribute = expressionNode.Attributes["type"];
                                            if (typeAttribute != null && !string.IsNullOrEmpty(typeAttribute.Value))
                                            {
                                                errorMessage += typeAttribute.Value
                                                    .Replace("\r", string.Empty)
                                                    .Replace("\n", string.Empty);
                                            }

                                            XmlNode originalNode = expressionNode.SelectSingleNode("Original");
                                            if (originalNode != null)
                                            {
                                                errorMessage += ("( " + originalNode.InnerText
                                                    .Replace("\r", string.Empty)
                                                    .Replace("\n", string.Empty)
                                                    .Replace(" ", string.Empty)
                                                    + " ) is NOT correct!");
                                            }

                                            errorMessage += "\n";
                                        }

                                        testResult.ErrorMessage = errorMessage;
                                    }
                                }
                            }
                            // Not run.
                            else
                            {
                                testResult.Outcome = TestOutcome.None;
                            }
                        }

                        reportedTestResults.Add(testCaseFromReport, true);
                        _frameworkHandle.RecordResult(testResult);
                    }
                }
            }
        }

        /// <summary>
        /// Start - Sets up the test executable process.
        /// </summary>
        /// <exception cref="NullReferenceException">Thrown if debugging unit tests and the process fails to start with the debugger attached.</exception>
        public void Start()
        {
            // Correct executable file path if needed.
            // Done in case a separate exe is generated but not by project output and is preferred for running tests against.
            // E.g. any .console.exe versions of a regular .exe file to run command line stuff.
            DoctestTestSettings settings = DoctestTestSettingsProvider.LoadSettings(_runContext);
            string testSource = FilePath;
            string solutionDirectory = Utilities.GetSolutionDirectory(Directory.GetParent(testSource).FullName);
            if (settings != null && settings.ExecutorSettings != null && settings.ExecutorSettings.ExecutableOverrides.Count > 0)
            {
                if (settings.ExecutorSettings.AreExecutableOverridesValid(solutionDirectory, out string message))
                {
                    foreach (ExecutableOverride executableOverride in settings.ExecutorSettings.ExecutableOverrides)
                    {
                        string key = executableOverride.Key;
                        string value = executableOverride.Value;
                        string keyFullPath = key;

                        // Check to see if key is an absolute filepath.
                        // If the key filepath doesn't exist that means it must be relative.
                        if (!File.Exists(key))
                        {
                            keyFullPath = Path.Combine(solutionDirectory, key);
                        }                          

                        if (testSource.Equals(keyFullPath))
                        {
                            string valueFullPath = value;
                            if (!File.Exists(value))
                            {
                                valueFullPath = Path.Combine(solutionDirectory, value);
                            }

                            testSource = Path.Combine(solutionDirectory, valueFullPath);
                            break;
                        }
                    }
                }
                else
                {
                    _frameworkHandle.SendMessage(TestMessageLevel.Warning, message);
                }
            }
            
            RecordTestStart();

            if (_runContext.IsBeingDebugged)
            {
                int processId = _frameworkHandle.LaunchProcessWithDebuggerAttached(testSource, solutionDirectory, _currentTestBatch.CommandArguments, null);
                _process = Process.GetProcessById(processId) ?? throw new NullReferenceException($"Failed to start process {testSource} with debugger attached - _process is null, abort!");
                _process.EnableRaisingEvents = true;
                _process.Exited += OnProcessExited;
            }
            else
            {
                _process = new System.Diagnostics.Process();
                _process.EnableRaisingEvents = true;

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

                // This is a bit misleading at the moment since I'm dumping any output into an xml file using the --reporter=xml argument.
                // So this won't actually print any doctest output at all.
                // See DoctestGeneralSettings.cs for more info.
                _process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (settings != null && settings.GeneralSettings != null && settings.GeneralSettings.PrintStandardOutput)
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine(e.Data);
                        }
                    }
                });

                _process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Console.WriteLine(e.Data);
                    }
                });

                _frameworkHandle.SendMessage(TestMessageLevel.Informational, Shared.Helpers.Constants.InformationMessagePrefix + " - About to start exe " + Path.GetFileName(testSource) + " with command arguments: " + processStartInfo.Arguments);

                // Start the executable now to run the doctests unit tests
                bool executableStartedSuccessfully = _process.Start();

                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();
            }
        }

        private void OnProcessExited(object sender, System.EventArgs e)
        {
            if (_process != null)
            {
                _process.Exited -= OnProcessExited;
                _process.Close();
                _process.Dispose();
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
