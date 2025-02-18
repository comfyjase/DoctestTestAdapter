using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private int totalNumberOfExecutables = 0;
        private int currentNumberOfRunningExecutables = 0;

        private List<TestExecutable> testExecutables = new List<TestExecutable>();

        private string GetCommandArguments(IEnumerable<TestCase> _tests, string _optionsFilePath)
        {
            List<string> testCaseNames = _tests.Select(t => t.DisplayName).ToList();

            // Should be something like: "[TestDecorator] Test 1, [TestDecorator] Test 2"
            string commaSeparatedListOfTestCaseNames = string.Join(",", testCaseNames);

            // Sorted into doctest specific argument formatting: "*"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"*"
            string doctestTestCaseCommandArgument = "--test-case=" + string.Join(",", commaSeparatedListOfTestCaseNames.Split(',').Select(x => string.Format("*\"{0}\"*", x)).ToList());

            // Full doctest arguments: --test-case="*"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"*"
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

            totalNumberOfExecutables = testExecutables.Count;
            currentNumberOfRunningExecutables = totalNumberOfExecutables;

            foreach (TestExecutable testExecutable in testExecutables)
            {
                if (cancelled)
                {
                    return;
                }

                string commandArguments = GetCommandArguments(testExecutable.Tests, optionsFilePath);

                //TODO_comfyjase_18/02/2025: validate command arguments
                if (commandArguments.Length > VS.Common.DoctestTestAdapter.Constants.MaxCommandArgumentLength)
                {
                    // ...
                    // Remember to increment totalNumberOfExecutables and currentNumberOfRunningExecutables
                    // if you thread off multiple processes
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
            currentNumberOfRunningExecutables--;

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
