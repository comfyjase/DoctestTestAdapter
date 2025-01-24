using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using VS.Common.DoctestTestAdapter;

namespace VS2022.DoctestTestAdapter
{
    [DefaultExecutorUri(DoctestTestAdapterConstants.ExecutorUriString)]
    [FileExtension(DoctestTestAdapterConstants.DLLFileExtension)]
    [FileExtension(DoctestTestAdapterConstants.ExeFileExtension)]
    [FileExtension(DoctestTestAdapterConstants.HFileExtension)]
    [FileExtension(DoctestTestAdapterConstants.HPPFileExtension)]
    public class DoctestTestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> _sources, IDiscoveryContext _discoveryContext, IMessageLogger _logger, ITestCaseDiscoverySink _discoverySink)
        {
            Logger.Instance.WriteLine("Begin");

            List<TestCase> tests = DoctestTestAdapterUtilities.GetTests(_sources, _discoveryContext, _logger, _discoverySink);

            foreach (TestCase test in tests)
            {
                Logger.Instance.WriteLine("discoverySink sending TestCase: " + test.DisplayName, 1);
                _discoverySink.SendTestCase(test);
            }

            Logger.Instance.WriteLine("End");
        }
    }
}
