using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VS.Common.DoctestTestAdapter;
using VS.Common.DoctestTestAdapter.Executables;

namespace VS2022.DoctestTestAdapter
{
    [ExtensionUri(DoctestTestAdapterConstants.ExecutorUriString)]
    public class DoctestTestExecutor : ITestExecutor
    {
        private bool cancelled = false;
        private bool waitingForTestResults = false;
        private int totalNumberOfTestRuns = 0;
        private int currentNumberOfTestRuns = 0;

        private List<TestExecutable> testExecutables = new List<TestExecutable>();

        private string GetCommandArguments(IEnumerable<TestCase> _tests, string _optionsFilePath)
        {
            List<string> testCaseNames = _tests.Select(t => t.DisplayName).ToList();

            // Should be something like: [TestDecorator] Test 1, [TestDecorator] Test 2
            string commaSeparatedListOfTestCaseNames = string.Join(",", testCaseNames);

            // Sorted into doctest specific argument formatting: *"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"*
            string doctestTestCaseCommandArgument = "--test-case=" + string.Join(",", commaSeparatedListOfTestCaseNames.Split(',').Select(x => string.Format("*\"{0}\"*", x)).ToList());

            // Full doctest arguments: --test-case=*"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"*
            string doctestArguments = doctestTestCaseCommandArgument;

            // Whatever the user has filled in the Tools -> Options -> Test Adapter for Doctest -> General -> Command Arguments option.
            string userDefinedArguments = VSUtilities.GetOptionValue<string>(_optionsFilePath,
                                                            VS.Common.DoctestTestAdapter.Constants.XmlNodeNames.GeneralOptions,
                                                            VS.Common.DoctestTestAdapter.Constants.XmlNodeNames.CommandArguments);

            // Combined user defined arguments (if any) and doctest arguments for running the unit tests.
            string fullCommandArguments = string.IsNullOrEmpty(userDefinedArguments) ? (doctestArguments) : (userDefinedArguments + " " + doctestArguments);

            return fullCommandArguments;
        }

        public void RunTests(IEnumerable<TestCase> _tests, IRunContext _runContext, IFrameworkHandle _frameworkHandle)
        {
            Logger.Instance.WriteLine("Begin");

            waitingForTestResults = true;
            cancelled = false;
            testExecutables.Clear();

            string optionsFilePath = _runContext.SolutionDirectory + "\\DoctestTestAdapter\\Options.xml";
            string userDefinedTestExecutableFilePath = VSUtilities.GetOptionValue<string>(optionsFilePath,
                                                                VS.Common.DoctestTestAdapter.Constants.XmlNodeNames.GeneralOptions,
                                                                VS.Common.DoctestTestAdapter.Constants.XmlNodeNames.TestExecutableFilePath);
            bool hasUserDefinedTestExecutableFilePath = !string.IsNullOrEmpty(userDefinedTestExecutableFilePath);

            if (hasUserDefinedTestExecutableFilePath)
            {
                Logger.Instance.WriteLine("User has provided test executable: " + userDefinedTestExecutableFilePath + " so using that to run " + _tests.Count() + " tests.");

                TestExecutable userDefinedTestExecutable = new TestExecutable(
                    userDefinedTestExecutableFilePath,
                    optionsFilePath,
                    _frameworkHandle
                );

                userDefinedTestExecutable.AddTestCases(_tests.ToList());

                testExecutables.Add(userDefinedTestExecutable);
            }
            else
            {
                Logger.Instance.WriteLine("No user defined test executable path provided, working out what executables to use from the source files and discovered executables.");

                foreach (TestCase test in _tests)
                {
                    if (cancelled)
                    {
                        return;
                    }

                    string executableFilePath = DoctestTestAdapterUtilities.GetTestExecutableFilePath(test.CodeFilePath);

                    TestExecutable associatedTestExecutable = null;

                    foreach (TestExecutable testExecutable in testExecutables)
                    {
                        if (testExecutable.FilePath.Equals(executableFilePath))
                        {
                            associatedTestExecutable = testExecutable;
                            break;
                        }
                    }

                    if (associatedTestExecutable == null)
                    {
                        associatedTestExecutable = new TestExecutable(
                            executableFilePath,
                            optionsFilePath,
                            _frameworkHandle
                        );
                        associatedTestExecutable.AddTestCase(test);
                        testExecutables.Add(associatedTestExecutable);
                    }
                    else
                    {
                        associatedTestExecutable.AddTestCase(test);
                    }
                }
            }
            
            totalNumberOfTestRuns = testExecutables.Count;
            currentNumberOfTestRuns = totalNumberOfTestRuns;

            // Fix up command arguments that are too long
            // Can happen in projects with LOADS of unit tests in
            // This test adapter works by passing in selected test cases into the --test-case argument
            // So many test cases can mean long arguments
            foreach (TestExecutable testExecutable in testExecutables)
            {
                if (cancelled)
                {
                    return;
                }

                string commandArguments = GetCommandArguments(testExecutable.Tests, optionsFilePath);

                if (commandArguments.Length > VS.Common.DoctestTestAdapter.Constants.MaxCommandArgumentLength)
                {
                    Logger.Instance.WriteLine(Path.GetFileName(testExecutable.FilePath) + " command arguments is too long, length: " + commandArguments.Length + " is greater than the limit: " + VS.Common.DoctestTestAdapter.Constants.MaxCommandArgumentLength + " so splitting up test cases to reduce command argument length.");

                    // Split the list of tests in half.
                    List<TestCase> testCases = testExecutable.Tests;
                    int halfNumberOfTestCasesList = testCases.Count / 2;
                    List<TestCase> firstHalfOfTests = testCases.Take(halfNumberOfTestCasesList).ToList();
                    List<TestCase> secondHalfOfTests = testCases.Skip(halfNumberOfTestCasesList).ToList();

                    // Clear the current list of tests from test executable since we need to reduce the amount of arguments.
                    testExecutable.Tests.Clear();
                    testExecutable.AddTestCases(firstHalfOfTests);
                    string newCommandArguments = GetCommandArguments(testExecutable.Tests, optionsFilePath);
                    Logger.Instance.WriteLine("Executable: " + Path.GetFileName(testExecutable.FilePath) + " will run " + testExecutable.Tests.Count + " tests with a shortened command argument length of: " + newCommandArguments.Length);
                    testExecutable.CommandArguments = newCommandArguments;

                    //TODO_comfyjase_20/02/2025: Might need a way of recursively checking command arguments and adding new executables?

                    // Pass the second half of the tests as a batch to be run after the first half of the tests has run.
                    newCommandArguments = GetCommandArguments(secondHalfOfTests, optionsFilePath);
                    testExecutable.AddTestBatch(secondHalfOfTests, newCommandArguments);
                    
                    // Increment the total number of test runs to be completed since there is a new batch of tests as well.
                    totalNumberOfTestRuns++;
                    currentNumberOfTestRuns = totalNumberOfTestRuns;
                }
                else
                {
                    testExecutable.CommandArguments = commandArguments;
                }

                testExecutable.Finished += OnTestExecutableFinished;
                testExecutable.Start();
            }

            //TODO_comfyjase_03/02/2025: Check if you still need this.
            while (waitingForTestResults)
            {
                System.Threading.Thread.Sleep(100);
            }

            Logger.Instance.WriteLine("End");
        }

        private void OnTestExecutableFinished(object sender, VS.Common.DoctestTestAdapter.EventArgs.TestExecutableFinishedEventArgs e)
        {
            currentNumberOfTestRuns--;

            if (currentNumberOfTestRuns > 0)
            {
                Logger.Instance.WriteLine("Waiting for " + currentNumberOfTestRuns + " executables...");
            }
            else
            {
                Logger.Instance.WriteLine("No more test executables to run...");
            }

            // No more test executables to run.
            if (currentNumberOfTestRuns == 0)
            {
                OnAllTestExecutablesFinished();
            }
        }

        private void OnAllTestExecutablesFinished()
        {
            Logger.Instance.WriteLine("All test executables have finished now.");
            testExecutables.Clear();
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
            testExecutables.Clear();

            Logger.Instance.WriteLine("End");
        }
    }
}
