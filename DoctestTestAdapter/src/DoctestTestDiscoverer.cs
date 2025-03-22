// DoctestTestDiscoverer.cs
//
// Copyright (c) 2025-present Jase Mottershead
//
// MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using DoctestTestAdapter.Settings;
using DoctestTestAdapter.Shared.Factory;
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
                        new TestCaseFactory(source, settings, null, logger)
                            .CreateTestCases()
                            .ForEach(testCase => discoverySink.SendTestCase(testCase));
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
