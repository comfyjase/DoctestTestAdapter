using DoctestTestAdapter.Execution;
using DoctestTestAdapter.Settings;
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
        private int _currentNumberOfTestBatches = 0;
        private List<DoctestTestExecutable> _testExecutables = new List<DoctestTestExecutable>();

        private void SetupTestExecutableWithTestBatches(DoctestTestExecutable newTestExecutable, IEnumerable<TestCase> tests, DoctestTestSettings settings, int startingBatchNumber = 1)
        {
            // Get the relevant test cases for this test executable.
            List<TestCase> allTestCasesFromSource = newTestExecutable.AllTestCases;
            List<TestCase> selectedTestCasesForSource = tests
                .Intersect(allTestCasesFromSource, new TestCaseComparer())
                .ToList();

            string commandArguments = Utilities.GetCommandArguments(newTestExecutable.FilePath, startingBatchNumber, settings, selectedTestCasesForSource);
            if (commandArguments.Length >= Helpers.Constants.MaxCommandPromptArgumentLength)
            {
                int numberOfBatchesRequired = (int)Math.Ceiling((decimal)commandArguments.Length / Helpers.Constants.MaxCommandPromptArgumentLength);
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

                    string argumentsForBatchedTests = Utilities.GetCommandArguments(newTestExecutable.FilePath, (iB + startingBatchNumber), settings, batchedTests);
                    
                    // If the arguments are still too long, recursively shrink them until they are the right length and add batches.
                    if (argumentsForBatchedTests.Length >= Helpers.Constants.MaxCommandPromptArgumentLength)
                    {
                        int previousNumberOfTestBatches = _currentNumberOfTestBatches;
                        
                        // Recursively add new batches.
                        SetupTestExecutableWithTestBatches(newTestExecutable, batchedTests, settings, (iB + startingBatchNumber));
                        
                        // Increment the starting batch number by the number of batches added.
                        int numberOfBatchesAdded = _currentNumberOfTestBatches - previousNumberOfTestBatches;
                        startingBatchNumber += (numberOfBatchesAdded - 1); // to take into account the zero based index for loop.
                    }
                    // Otherwise, they are fine, so add a new batch now.
                    else
                    {
                        // Increment the total number of test runs to be completed since there is a new batch of tests as well.
                        newTestExecutable.AddTestBatch(batchedTests, argumentsForBatchedTests);
                        _currentNumberOfTestBatches++;
                    }
                }
            }
            else
            {
                newTestExecutable.AddTestBatch(selectedTestCasesForSource, commandArguments);
                _currentNumberOfTestBatches++;
            }
        }

        private void OnTestExecutableFinished(object sender, System.EventArgs e)
        {
            _currentNumberOfTestBatches--;

            // No more test executables to run.
            if (_currentNumberOfTestBatches == 0)
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
            _currentNumberOfTestBatches = 0;
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            _waitingForTestResults = true;
            _currentNumberOfTestBatches = 0;
            _testExecutables.Clear();
            
            DoctestTestSettings settings = DoctestTestSettingsProvider.LoadSettings(runContext);

            foreach (TestCase test in tests)
            {
                bool hasTestExeAlreadyBeenSetup = _testExecutables.Any(t => t.FilePath.Equals(test.Source));
                if (hasTestExeAlreadyBeenSetup)
                    continue;

                DoctestTestExecutable newTestExecutable = new DoctestTestExecutable(test.Source, runContext, frameworkHandle);
                SetupTestExecutableWithTestBatches(newTestExecutable, tests, settings);
                _testExecutables.Add(newTestExecutable);
            }

            foreach (DoctestTestExecutable testExecutable in _testExecutables)
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
