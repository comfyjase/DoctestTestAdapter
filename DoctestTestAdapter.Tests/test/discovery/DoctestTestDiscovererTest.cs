// DoctestTestDiscovererTest.cs
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

using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using DoctestTestAdapter.Settings;
using System.IO;
using System;

namespace DoctestTestAdapter.Tests.Discovery
{
    [TestClass]
    public class DoctestTestDiscovererTest
    {
        private void UsingDoctestMainExe(string settingsAsString, bool shouldExpectToPrintStandardOutput)
        {
            IEnumerable<string> sources = new List<string>() { TestCommon.UsingDoctestMainExecutableFilePath };

            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
            IRunContext discoveryContext = A.Fake<IRunContext>();
            IMessageLogger messageLogger = A.Fake<IMessageLogger>();
            ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();
            A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCases._))
                .DoesNothing();

            DoctestTestSettings doctestTestSettings = null;
            if (!string.IsNullOrEmpty(settingsAsString))
            {
                DoctestTestSettingsProvider settingsProvider = new DoctestTestSettingsProvider();
                doctestTestSettings = TestCommon.LoadDoctestSettings(settingsProvider, settingsAsString);
                A.CallTo(() => discoveryContext.RunSettings.GetSettings(DoctestTestSettings.RunSettingsXmlNode))
                    .Returns(settingsProvider);
            }

            string output = string.Empty;

            using (StringWriter stringWriter = new StringWriter())
            {
                TextWriter previousWriter = Console.Out;

                Console.SetOut(stringWriter);

                ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
                doctestTestDiscoverer.DiscoverTests(sources, discoveryContext, messageLogger, testCaseDiscoverySink);

                output = stringWriter.ToString();

                Console.SetOut(previousWriter);
            }
         
            if (shouldExpectToPrintStandardOutput)
            {
                TestCommon.AssertStandardOutputSettingOutput(output, TestCommon.UsingDoctestMainTestHeaderFilePath);
            }
            else
            {
                Assert.IsTrue(string.IsNullOrEmpty(output));
            }

            Assert.HasCount(25, capturedTestCases.Values);

            TestCommon.AssertTestCases(capturedTestCases.Values.ToList(),
                TestCommon.UsingDoctestMainExecutableFilePath,
                "UsingDoctestMain",
                TestCommon.UsingDoctestMainTestHeaderFilePath);
        }

        [TestMethod]
        public void DiscoverExe() =>
            UsingDoctestMainExe(null, false);

        [TestMethod]
        public void DiscoverExeAndDLL()
        {
            IEnumerable<string> sources = new List<string>() { TestCommon.ExecutableUsingDLLExecutableFilePath };
            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();

            IRunContext discoveryContext = A.Fake<IRunContext>();
            IMessageLogger messageLogger = A.Fake<IMessageLogger>();
            ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();

            A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCases._))
                .DoesNothing();

            ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
            doctestTestDiscoverer.DiscoverTests(sources, discoveryContext, messageLogger, testCaseDiscoverySink);
            Assert.HasCount(50, capturedTestCases.Values);

            List<TestCase> dllTestCases = capturedTestCases.Values
                .ToList()
                .Where(t => t.DisplayName.Contains("[DLL]"))
                .ToList();
            List<TestCase> executableUsingDLLTestCases = capturedTestCases.Values
                .ToList()
                .Where(t => t.DisplayName.Contains("[ExecutableUsingDLL]"))
                .ToList();

            TestCommon.AssertTestCases(dllTestCases,
                TestCommon.ExecutableUsingDLLExecutableFilePath,
                "DLL",
                TestCommon.DLLTestHeaderFilePath
            );
            TestCommon.AssertTestCases(executableUsingDLLTestCases,
                TestCommon.ExecutableUsingDLLExecutableFilePath,
                "ExecutableUsingDLL",
                TestCommon.ExecutableUsingDLLTestHeaderFilePath
            );
        }

        [TestMethod]
        public void DiscoverExeWithPrintStandardOutputSetting() =>
            UsingDoctestMainExe(TestCommon.GeneralRunSettingsPrintStandardOutputExample, true);
    }
}
