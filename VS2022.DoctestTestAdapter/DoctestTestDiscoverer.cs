using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using VS.Common;

namespace VS2022.DoctestTestAdapter
{
    [DefaultExecutorUri(DoctestTestAdapterConstants.ExecutorUriString)]
    [FileExtension(".dll")]
    [FileExtension(".exe")]
    public class DoctestTestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            Logger.Instance.WriteLine("Begin");

            List<TestCase> tests = DoctestTestAdapterUtilities.GetTests(sources, discoveryContext, logger, discoverySink);

            foreach (TestCase test in tests)
            {
                Logger.Instance.WriteLine("discoverySink sending TestCase: " + test.DisplayName, 1);
                discoverySink.SendTestCase(test);
            }

            Logger.Instance.WriteLine("End");
        }
    }
}
