using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using VS.Common.DoctestTestAdapter;

namespace VS2022.DoctestTestAdapter
{
    public static class DoctestTestAdapterUtilities
    {
        public static TestCase CreateTestCase()
        {
            Logger.Instance.WriteLine("Begin");

            TestCase testCase = new TestCase();

            

            Logger.Instance.WriteLine("End");

            return testCase;
        }

        public static List<TestCase> GetTests(IEnumerable<string> _sources, IDiscoveryContext _discoveryContext, IMessageLogger _logger, ITestCaseDiscoverySink _discoverySink)
        {
            Logger.Instance.WriteLine("Begin");

            List<TestCase> tests = new List<TestCase>();

            foreach (string source in _sources)
            {
                Logger.Instance.WriteLine("Searching file: " + source, 1);

                // TODO: Logic to search through file and find doctest TestCases?
                // Read file line by line like in the custom window tool setup?
                // Search for keyword macros?

            }

            Logger.Instance.WriteLine("Found " + tests.Count + " TestCases");
            Logger.Instance.WriteLine("End");

            return tests;
        }
    }
}
