using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using VS.Common.DoctestTestAdapter;

namespace VS2022.DoctestTestAdapter
{
    [ExtensionUri(DoctestTestAdapterConstants.ExecutorUriString)]
    public class DoctestTestExecutor : ITestExecutor
    {
        private bool cancelled = false;

        private void RunTest(TestCase _test, TestResult _testResult)
        {
            // TODO: Logic to run doctest tests?
            _testResult.Outcome = TestOutcome.Passed;
        }

        public void RunTests(IEnumerable<TestCase> _tests, IRunContext _runContext, IFrameworkHandle _frameworkHandle)
        {
            Logger.Instance.WriteLine("Begin");

            cancelled = false;

            foreach (TestCase test in _tests)
            {
                if (cancelled)
                    break;

                Logger.Instance.WriteLine("About to run test: " + test.DisplayName, 1);

                TestResult testResult = new TestResult(test);
                RunTest(test, testResult);
                _frameworkHandle.RecordResult(testResult);
            }

            Logger.Instance.WriteLine("End");
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
            
            Logger.Instance.WriteLine("End");
        }
    }
}
