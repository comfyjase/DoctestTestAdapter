﻿using DoctestTestAdapter.Settings;
using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
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
            DoctestTestSettings settings = DoctestTestSettingsProvider.LoadSettings(discoveryContext);

            foreach (string source in sources)
            {
                Utilities.GetTestCases(source, settings)
                    .ForEach(testCase => discoverySink.SendTestCase(testCase));
            }
        }
    }
}
