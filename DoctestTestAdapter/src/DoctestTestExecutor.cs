using DoctestTestAdapter.Execution;
using DoctestTestAdapter.Shared.EqualityComparers;
using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

using Helpers = DoctestTestAdapter.Shared.Helpers;

namespace DoctestTestAdapter
{
    [ExtensionUri(Helpers.Constants.ExecutorUriString)]
    public sealed class DoctestTestExecutor : ITestExecutor
    {
        private bool _cancelled = false; 
        private bool _waitingForTestResults = false;
        private int _currentNumberOfTestRuns = 0;
        private List<TestExecutable> _testExecutables = new List<TestExecutable>();

        private void OnTestExecutableFinished(object sender, System.EventArgs e)
        {
            _currentNumberOfTestRuns--;

            // No more test executables to run.
            if (_currentNumberOfTestRuns == 0)
            {
                OnAllTestExecutablesFinished();
            }
        }

        private void OnAllTestExecutablesFinished()
        {
            _testExecutables.Clear();
            _waitingForTestResults = false;
        }

        public void Cancel()
        {
            _cancelled = true;
            _waitingForTestResults = false;
            _currentNumberOfTestRuns = 0;
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            IMessageLogger logger = frameworkHandle;

            _waitingForTestResults = true;
            _currentNumberOfTestRuns = 0;
            _testExecutables.Clear();

            Action<TestExecutable> setupTestExecutable = (TestExecutable testExe) =>
            {
                _currentNumberOfTestRuns++;

                List<TestCase> allTestCasesFromSource = Utilities.GetTestCases(testExe.FilePath);
                List<TestCase> selectedTestCasesForSource = tests
                    .Intersect(allTestCasesFromSource, new TestCaseComparer())
                    .ToList();

                string commandArguments = Utilities.GetCommandArguments(selectedTestCasesForSource);
                if (commandArguments.Length > Helpers.Constants.MaxCommandArgumentLength)
                {
                    //TODO_comfyjase_26/02/2025: Write a way to do this recursively.

                    // Split the list of tests in half.
                    int halfNumberOfTestCasesList = selectedTestCasesForSource.Count / 2;
                    List<TestCase> firstHalfOfTests = selectedTestCasesForSource.Take(halfNumberOfTestCasesList).ToList();
                    List<TestCase> secondHalfOfTests = selectedTestCasesForSource.Skip(halfNumberOfTestCasesList).ToList();

                    testExe.AddTestBatch(firstHalfOfTests, Utilities.GetCommandArguments(firstHalfOfTests));
                    testExe.AddTestBatch(secondHalfOfTests, Utilities.GetCommandArguments(secondHalfOfTests));

                    // Increment the total number of test runs to be completed since there is a new batch of tests as well.
                    _currentNumberOfTestRuns++;
                }
                else
                {
                    testExe.AddTestBatch(selectedTestCasesForSource, commandArguments);
                }

                _testExecutables.Add(testExe);
            };

            foreach (TestCase test in tests)
            {
                bool hasTestExeAlreadyBeenSetup = _testExecutables.Any(t => t.FilePath.Equals(test.Source));
                if (hasTestExeAlreadyBeenSetup)
                    continue;

                TestExecutable newTestExecutable = new TestExecutable(test.Source, frameworkHandle);
                setupTestExecutable(newTestExecutable);
            }

            foreach (TestExecutable testExecutable in _testExecutables)
            {
                testExecutable.Finished += OnTestExecutableFinished;
                testExecutable.Start();
            }

            //TODO_comfyjase_03/02/2025: Check if you still need this.
            while (_waitingForTestResults)
            {
                System.Threading.Thread.Sleep(100);
            }
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            IDiscoveryContext discoveryContext = runContext;
            IMessageLogger logger = frameworkHandle;

            List<TestCase> tests = new List<TestCase>();
            foreach (string source in sources)
            {
                if (_cancelled)
                {
                    return;
                }

                List<TestCase> sourceTestCases = Utilities.GetTestCases(source);
                if (sourceTestCases == null || sourceTestCases.Count == 0)
                    continue;

                tests.AddRange(sourceTestCases);
            }

            RunTests(tests, runContext, frameworkHandle);
        }
    }
}
