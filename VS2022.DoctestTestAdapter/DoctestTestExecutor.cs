using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using VS.Common;

namespace VS2022.DoctestTestAdapter
{
    [ExtensionUri(DoctestTestAdapterConstants.ExecutorUriString)]
    public class DoctestTestExecutor : ITestExecutor
    {
        private bool cancelled = false;

        private void RunTest(TestCase test, TestResult testResult)
        {
            // TODO: Logic to run doctest tests?

        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            Logger.Instance.WriteLine("Begin");

            cancelled = false;

            foreach (TestCase test in tests)
            {
                if (cancelled)
                    break;

                Logger.Instance.WriteLine("About to run test: " + test.DisplayName, 1);

                TestResult testResult = new TestResult(test);
                RunTest(test, testResult);
                frameworkHandle.RecordResult(testResult);
            }

            Logger.Instance.WriteLine("End");
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            Logger.Instance.WriteLine("Begin");

            IDiscoveryContext discoveryContext = runContext;
            IMessageLogger logger = frameworkHandle;
            ITestCaseDiscoverySink discoverySink = null;
            IEnumerable<TestCase> tests = DoctestTestAdapterUtilities.GetTests(sources, discoveryContext, logger, discoverySink);
            RunTests(tests, runContext, frameworkHandle);

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
