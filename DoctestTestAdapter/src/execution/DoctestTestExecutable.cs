﻿using DoctestTestAdapter.Settings;
using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Xml;

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

        public DoctestTestExecutable() : this(null, null, null)
        { }

        public DoctestTestExecutable(string filePath, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            _filePath = filePath;
            _runContext = runContext;
            _frameworkHandle = frameworkHandle;
        }

        public void TrackTestCase(TestCase testCase)
        {
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

            XmlDocument testReportDocument = new XmlDocument();
            testReportDocument.Load(_currentTestBatch.TestReportFilePath);

            XmlNodeList testCaseNodes = testReportDocument.SelectNodes("//doctest/TestSuite/TestCase");

            foreach (XmlNode testCaseNode in testCaseNodes)
            {
                XmlAttribute nameAttribute = testCaseNode.Attributes["name"];
                XmlAttribute fileNameAttribute = testCaseNode.Attributes["filename"];
                if (nameAttribute != null && !string.IsNullOrWhiteSpace(nameAttribute.Value)
                    && fileNameAttribute != null && !string.IsNullOrEmpty(fileNameAttribute.Value))
                {
                    // Might need to do this to remove escape symbols: nameAttribute.Value.Replace("\,", ",");
                    string testCaseNameFromReport = nameAttribute.Value;
                    string testCaseFileNameFromReport = fileNameAttribute.Value;

                    if (_currentTestBatch.Tests.Any(t => (t.DisplayName.Equals(testCaseNameFromReport) && t.CodeFilePath.Equals(testCaseFileNameFromReport.Replace("/", "\\")))))
                    {
                        TestCase testCaseFromReport = _currentTestBatch.Tests
                            .Single(t => (t.DisplayName.Equals(testCaseNameFromReport) && t.CodeFilePath.Equals(testCaseFileNameFromReport.Replace("/", "\\"))));

                        if (testCaseFromReport != null)
                        {
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
        }

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
            
            RecordTestStart();

            if (_runContext.IsBeingDebugged)
            {
                int processId = _frameworkHandle.LaunchProcessWithDebuggerAttached(testSource, solutionDirectory, _currentTestBatch.CommandArguments, null);
                _process = Process.GetProcessById(processId);
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

                _process.OutputDataReceived += (object _sender, DataReceivedEventArgs _e) =>
                {
                    if (_e.Data != null && _e.Data.Count() > 0)
                    {
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

                // TODO: Check if launch with debugger automatically closes/cleans up the process class...
                //if (!_runContext.IsBeingDebugged)
                //{
                    _process.Close();
                    _process = null;
                //}

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
