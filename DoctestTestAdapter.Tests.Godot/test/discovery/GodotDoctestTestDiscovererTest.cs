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
	public class GodotDoctestTestDiscovererTest
	{
        [TestMethod]
		public void DiscoverExe()
		{
            IEnumerable<string> sources = new List<string>() { TestCommon.GodotExecutableFilePath };

            // Run settings.
            DoctestTestSettingsProvider settingsProvider = new DoctestTestSettingsProvider();
            DoctestTestSettings settings = TestCommon.LoadDoctestSettings(settingsProvider, TestCommon.GodotRunSettingsExample);
            Assert.IsNotNull(settings);

            IRunContext runContext = A.Fake<IRunContext>();
            IMessageLogger messageLogger = A.Fake<IMessageLogger>();
            ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();
            Captured<TestCase> capturedTestCasesFromDiscovery = A.Captured<TestCase>();
            A.CallTo(() => runContext.RunSettings.GetSettings(DoctestTestSettings.RunSettingsXmlNode))
                .Returns(settingsProvider);
            A.CallTo(() => runContext.IsBeingDebugged)
                .Returns(false);
            A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCasesFromDiscovery._))
                .DoesNothing();

            Assert.IsNotNull(settings.GeneralSettings);
            Assert.IsFalse(string.IsNullOrEmpty(settings.GeneralSettings.CommandArguments));
            Assert.AreEqual("--headless --test", settings.GeneralSettings.CommandArguments);

            Assert.IsNotNull(settings.DiscoverySettings);
            Assert.HasCount(2, settings.DiscoverySettings.SearchDirectories);
            Assert.IsFalse(string.IsNullOrEmpty(settings.DiscoverySettings.SearchDirectories[0]));
            Assert.IsFalse(string.IsNullOrEmpty(settings.DiscoverySettings.SearchDirectories[1]));
            Assert.AreEqual("modules", settings.DiscoverySettings.SearchDirectories[0]);
            Assert.AreEqual("tests", settings.DiscoverySettings.SearchDirectories[1]);

            ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
            doctestTestDiscoverer.DiscoverTests(sources, runContext, messageLogger, testCaseDiscoverySink);
            //TODO_comfyjase_23/03/2025: This isn't the right amount, godot has at least 100 more test cases missing.
            // 1215 test cases the cmd prompt prints.
            // Godot uses TEST_CASE_TEMPLATE too, missing a lot of math unit tests.
            // Will work on that next...
            Assert.HasCount(1105, capturedTestCasesFromDiscovery.Values); 
        }
    }
}
