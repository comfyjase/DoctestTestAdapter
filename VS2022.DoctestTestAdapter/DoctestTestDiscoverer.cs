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
            
            //TODO_comfyjase_27/01/2025: Setup custom run settings for doctest setup.
            IRunSettings runSettings = _discoveryContext.RunSettings;
            if (runSettings != null)
            {
                ISettingsProvider settingsProvider = runSettings.GetSettings("DoctestTestAdapterSettings");

                // ...

                // Access settings as xml doc
                // Search for executable values
                // Check if test.CodeFilePath contains the name of the executable file.
                // If so, add a test property to test to include that executable filepath?

                // ...
            }

            foreach (TestCase test in tests)
            {
                Logger.Instance.WriteLine("discoverySink sending TestCase: " + test.DisplayName, 1);
                _discoverySink.SendTestCase(test);
            }

            Logger.Instance.WriteLine("End");
        }
    }
}
