// GodotDoctestTestDiscovererTest.cs
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
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoctestTestAdapter.Tests.Godot.Discovery
{
	[TestClass]
	public class GodotDoctestTestDiscovererTest : GodotTest
    {
        [TestMethod]
		public void DiscoverExe()
		{
            TestCommon.AssertErrorOutput(() =>
            {
                IEnumerable<string> sources = new List<string>() { TestCommon.GodotExecutableFilePath };

                // Run settings.
                DoctestTestSettingsProvider doctestTestSettingsProvider = new DoctestTestSettingsProvider();
                AssertAndLoadExampleRunSettings(doctestTestSettingsProvider);

                IRunContext runContext = A.Fake<IRunContext>();
                IMessageLogger messageLogger = A.Fake<IMessageLogger>();
                ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();
                Captured<TestCase> capturedTestCasesFromDiscovery = A.Captured<TestCase>();
                A.CallTo(() => runContext.RunSettings.GetSettings(DoctestTestSettings.RunSettingsXmlNode))
                    .Returns(doctestTestSettingsProvider);
                A.CallTo(() => runContext.IsBeingDebugged)
                    .Returns(false);
                A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCasesFromDiscovery._))
                    .DoesNothing();

                ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
                doctestTestDiscoverer.DiscoverTests(sources, runContext, messageLogger, testCaseDiscoverySink);

                Assert.IsNotEmpty(capturedTestCasesFromDiscovery.Values);
                AssertMissingTestCases(runContext, capturedTestCasesFromDiscovery.Values);
            });
        }
    }
}
