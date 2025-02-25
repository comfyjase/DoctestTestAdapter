﻿using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;

using Helpers = DoctestTestAdapter.Shared.Helpers;

namespace DoctestTestAdapter
{
    [DefaultExecutorUri(Helpers.Constants.ExecutorUriString)]
    [ExtensionUri(Helpers.Constants.ExecutorUriString)]
    [FileExtension(".dll")]
    [FileExtension(".exe")]
    public sealed class DoctestTestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            // Loop over sources
            // Find any exe/dll dependencies
            // Find the relevant pdb file
            // Read the pdb file
            // Get a list of strings for test files to use
            // Loop over all of the files
            // Read the contents into a list of strings
            // Loop over the string contents and search for keywords

            //string solutionDirectory = Utilities.GetSolutionDirectory();
            List<TestCase> tests = new List<TestCase>();

            foreach (string source in sources)
            {
                // Skip for now?
                if (source.EndsWith(".dll"))
                {
                    continue;
                }

                tests = Utilities.GetTestCases(source);
            }

            foreach (TestCase test in tests)
            {
                discoverySink.SendTestCase(test);
            }
        }
    }
}
