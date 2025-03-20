using DoctestTestAdapter.Settings;
using DoctestTestAdapter.Shared.Helpers;
using DoctestTestAdapter.Shared.Profiling;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using Helpers = DoctestTestAdapter.Shared.Helpers;

namespace DoctestTestAdapter
{
    [DefaultExecutorUri(Helpers.Constants.ExecutorUriString)]
    [ExtensionUri(Helpers.Constants.ExecutorUriString)]
    [FileExtension(".exe")]
    public sealed class DoctestTestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            Utilities.CheckEnumerable(sources, nameof(sources));
            Utilities.CheckNull(discoveryContext, nameof(discoveryContext));
            Utilities.CheckNull(logger, nameof(logger));
            Utilities.CheckNull(discoverySink, nameof(discoverySink));

            Profiler profiler = new Profiler();
            profiler.Start();
            {
                try
                {
                    DoctestTestSettings settings = DoctestTestSettingsProvider.LoadSettings(discoveryContext);

                    foreach (string source in sources)
                    {
                        List<TestCase> discoveredTestCases = Utilities.GetTestCases(source, logger, settings);
                        discoveredTestCases.ForEach(testCase => discoverySink.SendTestCase(testCase));
                    }
                }
                catch(Exception ex)
                {
                    logger.SendMessage(TestMessageLevel.Error, Helpers.Constants.ErrorMessagePrefix + $"[Test Discovery]: {ex}");
                }
            }
            profiler.End();
        }
    }
}
