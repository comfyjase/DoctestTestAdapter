using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;

using Helpers = DoctestTestAdapter.Shared.Helpers;

namespace DoctestTestAdapter
{
    [ExtensionUri(Helpers.Constants.ExecutorUriString)]
    public sealed class DoctestTestExecutor : ITestExecutor2
    {
        private bool _cancelled = false;

        public void Cancel()
        {
            _cancelled = true;
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            foreach (TestCase test in tests)
            {
                if (_cancelled)
                {
                    return;
                }

                frameworkHandle.RecordStart(test);

                // TEMP
                // Replace with actual logic...
                TestResult testResult = new TestResult(test);
                testResult.Outcome = TestOutcome.None;

                frameworkHandle.RecordResult(testResult);
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

                tests.AddRange(Utilities.GetTestCases(source));
            }

            RunTests(tests, runContext, frameworkHandle);
        }

        public bool ShouldAttachToTestHost(IEnumerable<string> sources, IRunContext runContext) => true;

        public bool ShouldAttachToTestHost(IEnumerable<TestCase> tests, IRunContext runContext) => true;
    }
}
