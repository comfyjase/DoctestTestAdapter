// DoctestExecutable.cs
//
// Copyright (c) 2025-present Jase Mottershead
//
// MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using DoctestTestAdapter.Settings;
using DoctestTestAdapter.Shared.EqualityComparers;
using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;

namespace DoctestTestAdapter.Shared.Executables
{
    internal class DoctestExecutable : Executable
    {
        private enum DoctestKeywordNameType
        {
            TestSuite = 0,
            TestCase
        }

        private List<TestCase> _allTestCases = new List<TestCase>();

        private List<DoctestExecutableTestBatch> _testBatches = new List<DoctestExecutableTestBatch>();
        internal int NumberOfTestBatches
        {
            get
            {
                return _testBatches.Count;
            }
        }
        private DoctestExecutableTestBatch _currentTestBatch = null;

        internal event EventHandler<EventArgs> Finished = null;

        internal DoctestExecutable(string executableFilePath, string solutionDirectory, DoctestTestSettings settings, IRunContext runContext, IMessageLogger logger, IFrameworkHandle frameworkHandle) : base(executableFilePath, solutionDirectory, settings, runContext, logger, frameworkHandle)
        {}

        private List<string> GetDoctestKeywordNames(DoctestKeywordNameType nameType)
        {
            List<string> doctestNames = null;

            string listArgument = null;
            switch (nameType)
            {
                case DoctestKeywordNameType.TestSuite:  { listArgument = "--list-test-suites";  break; }
                case DoctestKeywordNameType.TestCase:   { listArgument = "--list-test-cases";   break; }
                default:                                { throw new InvalidEnumArgumentException($"Undefined DoctestKeywordNameType {nameType}, abort!"); }
            }

            if (Settings != null && Settings.TryGetCommandArguments(out string commandArguments))
            {
                Arguments = commandArguments + " --no-intro=true --no-version=true " + listArgument;
            }
            else
            {
                Arguments = "--no-intro=true --no-version=true " + listArgument;
            }

            Start();

            if (string.IsNullOrEmpty(Output))
                throw new NullReferenceException($"{FilePath} did not provide valid 'Output' for {Arguments}, abort!");

            string startSearchString = "===============================================================================\r\n";
            string endSearchString = "\r\n===============================================================================";
            int startOfDoctestListIndex = Output.IndexOf(startSearchString) + startSearchString.Length;
            int endOfDoctestListIndex = Output.LastIndexOf(endSearchString);
            string subString = Output.Substring(startOfDoctestListIndex, endOfDoctestListIndex - startOfDoctestListIndex);

            if (!string.IsNullOrEmpty(subString))
            {
                doctestNames = subString
                    .Split('\n')
                    .Select(s => s.Trim())
                    .ToList();
            }

            return doctestNames;
        }

        internal List<string> GetTestSuiteNames() => GetDoctestKeywordNames(DoctestKeywordNameType.TestSuite);

        internal List<string> GetTestCaseNames() => GetDoctestKeywordNames(DoctestKeywordNameType.TestCase);

        internal void TrackTestCase(TestCase testCase)
        {
            Utilities.CheckNull(testCase, nameof(testCase));

            if (!_allTestCases.Contains(testCase))
                _allTestCases.Add(testCase);
        }

        internal void AddTestBatch(List<TestCase> tests, string commandArguments)
        {
            string batchTestReportFilePath = Directory.GetParent(FilePath).FullName + "\\" + Path.GetFileNameWithoutExtension(FilePath) + "_TestReport_" + (_testBatches.Count + 1).ToString() + ".xml";
            _testBatches.Add(new DoctestExecutableTestBatch(tests, commandArguments, _testBatches.Count + 1, batchTestReportFilePath));

            if (_currentTestBatch == null)
            {
                _currentTestBatch = _testBatches.First();
            }
        }

        private void RecordTestStart()
        {
            foreach (TestCase testCase in _currentTestBatch.Tests)
            {
                FrameworkHandle.RecordStart(testCase);
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
                                                errorMessage += typeAttribute.Value.Trim();
                                            }

                                            XmlNode originalNode = expressionNode.SelectSingleNode("Original");
                                            if (originalNode != null)
                                            {
                                                errorMessage += ("( " + originalNode.InnerText.Trim() + " ) is NOT correct!");
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
                        FrameworkHandle.RecordResult(testResult);
                    }
                }
            }
        }

        private string GetBatchCommandArguments(IEnumerable<TestCase> tests, int batchNumber)
        {
            List<string> testCaseNames = tests.Select(t => string.Format("*\"{0}\"*", t.DisplayName.Replace(@"\", @"\\").Replace(",", @"\,"))).ToList();

            // Sorted into doctest specific argument formatting: *"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"*
            string doctestTestCaseCommandArgument = "--test-case=" + string.Join(",", testCaseNames);

            // Report so we know what tests passed, failed, skipped.
            string testReportFilePath = Directory.GetParent(FilePath).FullName + "\\" + Path.GetFileNameWithoutExtension(FilePath) + "_TestReport_" + batchNumber.ToString() + ".xml";
            string doctestReporterCommandArgument = "--duration=true --reporters=xml --out=" + testReportFilePath;

            // Full doctest arguments: --test-case=*"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"* --duration=true --reporters=xml --out=AppTestReport.xml
            string doctestArguments = doctestTestCaseCommandArgument + " " + doctestReporterCommandArgument;

            string fullCommandArguments = string.Empty;

            // User defined command arguments: --test
            if (Settings != null && Settings.TryGetCommandArguments(out string commandArguments))
            {
                fullCommandArguments = commandArguments + " " + doctestArguments;
            }
            // Otherwise, just use regular doctest arguments
            else
            {
                fullCommandArguments = doctestArguments;
            }

            return fullCommandArguments;
        }

        internal void SetupBatches(IEnumerable<TestCase> tests, int startingBatchNumber = 1)
        {
            // Get the relevant test cases for this test executable.
            List<TestCase> selectedTestCasesForSource = tests
                .Intersect(_allTestCases, new TestCaseComparer())
                .ToList();

            string commandArguments = GetBatchCommandArguments(selectedTestCasesForSource, startingBatchNumber);
            if (commandArguments.Length >= Shared.Helpers.Constants.MaxCommandPromptArgumentLength)
            {
                int numberOfBatchesRequired = (int)Math.Ceiling((decimal)commandArguments.Length / Shared.Helpers.Constants.MaxCommandPromptArgumentLength);
                int numberOfElementsEachList = (int)Math.Ceiling((decimal)selectedTestCasesForSource.Count / numberOfBatchesRequired);

                for (int iB = 0; iB < numberOfBatchesRequired; iB++)
                {
                    // If there are 12 tests overall and we need 3 batches
                    // Create 3 lists, each with 4 elements in
                    // 12 tests -> GetRange(4 * iB[0]) = startIndex: 0 -> count: numberOfElements
                    // 12 tests -> GetRange(4 * iB[1]) = startIndex: 4 -> "
                    // 12 tests -> GetRange(4 * iB[2]) = startIndex: 8 -> "
                    //
                    // If there happens to be an odd number of cases the last batch is will check to see how many tests to use
                    // based on how many tests there are remaining to be batched.

                    int amountOfTestsBatched = (numberOfElementsEachList * iB);
                    int remainingNumberOfTestCasesToBatch = selectedTestCasesForSource.Count - amountOfTestsBatched;
                    int numberOfElements = (numberOfElementsEachList > remainingNumberOfTestCasesToBatch ? remainingNumberOfTestCasesToBatch : numberOfElementsEachList);

                    List<TestCase> batchedTests = selectedTestCasesForSource.GetRange((numberOfElementsEachList * iB), numberOfElements)
                    .ToList();

                    string argumentsForBatchedTests = GetBatchCommandArguments(batchedTests, (iB + startingBatchNumber));

                    // If the arguments are still too long, recursively shrink them until they are the right length and add batches.
                    if (argumentsForBatchedTests.Length >= Shared.Helpers.Constants.MaxCommandPromptArgumentLength)
                    {
                        int previousNumberOfTestBatches = _testBatches.Count;

                        // Recursively add new batches.
                        SetupBatches(batchedTests, (iB + startingBatchNumber));

                        // Increment the starting batch number by the number of batches added.
                        int numberOfBatchesAdded = _testBatches.Count - previousNumberOfTestBatches;
                        startingBatchNumber += (numberOfBatchesAdded - 1); // to take into account the zero based index for loop.
                    }
                    // Otherwise, they are fine, so add a new batch now.
                    else
                    {
                        // Increment the total number of test runs to be completed since there is a new batch of tests as well.
                        AddTestBatch(batchedTests, argumentsForBatchedTests);
                    }
                }
            }
            else
            {
                AddTestBatch(selectedTestCasesForSource, commandArguments);
            }
        }

        internal void RunUnitTests()
        {
            // Correct executable file path if needed.
            // Done in case a separate exe is generated but not by project output and is preferred for running tests against.
            // E.g. any .console.exe versions of a regular .exe file to run command line stuff.
            string testSource = FilePath;
            if (Settings != null && Settings.TryGetExecutableOverrides(out List<ExecutableOverride> executableOverrides))
            {
                if (Settings.ExecutorSettings.AreExecutableOverridesValid(SolutionDirectory, out string message))
                {
                    foreach (ExecutableOverride executableOverride in executableOverrides)
                    {
                        string key = executableOverride.Key;
                        string value = executableOverride.Value;
                        string keyFullPath = key;

                        // Check to see if key is an absolute filepath.
                        // If the key filepath doesn't exist that means it must be relative.
                        if (!File.Exists(key))
                        {
                            keyFullPath = Path.Combine(SolutionDirectory, key);
                        }

                        if (testSource.Equals(keyFullPath))
                        {
                            string valueFullPath = value;
                            if (!File.Exists(value))
                            {
                                valueFullPath = Path.Combine(SolutionDirectory, value);
                            }

                            testSource = Path.Combine(SolutionDirectory, valueFullPath);
                            break;
                        }
                    }
                }
                else
                {
                    Logger.SendMessage(TestMessageLevel.Warning, message);
                }
            }

            RecordTestStart();

            Arguments = _currentTestBatch.CommandArguments;

            Logger.SendMessage(TestMessageLevel.Informational, Shared.Helpers.Constants.InformationMessagePrefix + " - About to start exe " + Path.GetFileName(testSource) + " with command arguments: " + Arguments);

            Start(false, testSource);
        }

        protected override void OnProcessExited(object sender, EventArgs e)
        {
            base.OnProcessExited(sender, e);

            RecordTestFinish();

            Finished?.Invoke(this, EventArgs.Empty);

            CheckIfAnyTestsAreLeftToRun();
        }

        private void CheckIfAnyTestsAreLeftToRun()
        {
            _testBatches.Remove(_testBatches.First());

            if (_testBatches.Count > 0)
            {
                _currentTestBatch = _testBatches.First();
                RunUnitTests();
            }
        }
    }
}
