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
        [TestMethod]
        public void DiscoverExeWithNoDoctestUnitTests()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                IEnumerable<string> sources = new List<string>() { TestCommon.NoDoctestUnitTestsExecutableFilePath };
                IRunContext discoveryContext = A.Fake<IRunContext>();
                IMessageLogger messageLogger = A.Fake<IMessageLogger>();
                ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();
                Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
                A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCases._))
                    .DoesNothing();

                ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
                doctestTestDiscoverer.DiscoverTests(sources, discoveryContext, messageLogger, testCaseDiscoverySink);

                Assert.IsEmpty(capturedTestCases.Values);
            });
        }

        [TestMethod]
        public void DiscoverExeWithOnlyTestCases()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                IEnumerable<string> sources = new List<string>() { TestCommon.OnlyTestCasesExecutableFilePath };
                IRunContext discoveryContext = A.Fake<IRunContext>();
                IMessageLogger messageLogger = A.Fake<IMessageLogger>();
                ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();
                Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
                A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCases._))
                    .DoesNothing();

                ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
                doctestTestDiscoverer.DiscoverTests(sources, discoveryContext, messageLogger, testCaseDiscoverySink);

                Assert.HasCount(3, capturedTestCases.Values);

                TestCommon.AssertTestCase(capturedTestCases.Values[0],
                    TestCommon.OnlyTestCasesExecutableFilePath,
                    "Empty Namespace::Empty Class::[TestCasesOnly] - Test One",
                    "[TestCasesOnly] - Test One",
                    TestCommon.OnlyTestCasesTestHeaderFilePath,
                    5);

                TestCommon.AssertTestCase(capturedTestCases.Values[1],
                    TestCommon.OnlyTestCasesExecutableFilePath,
                    "Empty Namespace::Empty Class::[TestCasesOnly] - Test Two",
                    "[TestCasesOnly] - Test Two",
                    TestCommon.OnlyTestCasesTestHeaderFilePath,
                    10);

                TestCommon.AssertTestCase(capturedTestCases.Values[2],
                    TestCommon.OnlyTestCasesExecutableFilePath,
                    "Empty Namespace::Empty Class::[TestCasesOnly] - Test Three",
                    "[TestCasesOnly] - Test Three",
                    TestCommon.OnlyTestCasesTestHeaderFilePath,
                    15);
            });
        }

        [TestMethod]
        public void DiscoverExeWithOnlyTestSuites()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                IEnumerable<string> sources = new List<string>() { TestCommon.OnlyTestSuitesExecutableFilePath };
                IRunContext discoveryContext = A.Fake<IRunContext>();
                IMessageLogger messageLogger = A.Fake<IMessageLogger>();
                ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();
                Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
                A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCases._))
                    .DoesNothing();

                ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
                doctestTestDiscoverer.DiscoverTests(sources, discoveryContext, messageLogger, testCaseDiscoverySink);

                Assert.IsEmpty(capturedTestCases.Values);
            });
        }

        [TestMethod]
        public void DiscoverExeWithEmptyTestSuites()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                IEnumerable<string> sources = new List<string>() { TestCommon.EmptySuitesExecutableFilePath };
                IRunContext discoveryContext = A.Fake<IRunContext>();
                IMessageLogger messageLogger = A.Fake<IMessageLogger>();
                ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();
                Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
                A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCases._))
                    .DoesNothing();

                ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
                doctestTestDiscoverer.DiscoverTests(sources, discoveryContext, messageLogger, testCaseDiscoverySink);

                Assert.HasCount(1, capturedTestCases.Values);

                TestCommon.AssertTestCase(capturedTestCases.Values[0],
                    TestCommon.EmptySuitesExecutableFilePath,
                    "TestNamespace::Empty Class::[TestCase] - Valid",
                    "[TestCase] - Valid",
                    TestCommon.EmptyTestSuitesTestHeaderFilePath,
                    7);
            });
        }

        private void UsingDoctestMainExe(string settingsAsString, bool shouldExpectToPrintStandardOutput)
        {
            TestCommon.AssertErrorOutput(() =>
            {
                IEnumerable<string> sources = new List<string>() { TestCommon.UsingDoctestMainExecutableFilePath };
                IRunContext discoveryContext = A.Fake<IRunContext>();
                IMessageLogger messageLogger = A.Fake<IMessageLogger>();
                ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();
                Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
                A.CallTo(() => discoveryContext.IsBeingDebugged)
                    .Returns(false);
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

                using (StringWriter stringWriterOut = new StringWriter())
                {
                    TextWriter previousWriterOut = Console.Out;
                    Console.SetOut(stringWriterOut);

                    ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
                    doctestTestDiscoverer.DiscoverTests(sources, discoveryContext, messageLogger, testCaseDiscoverySink);

                    output = stringWriterOut.ToString();
                    Console.SetOut(previousWriterOut);
                }

                if (shouldExpectToPrintStandardOutput)
                {
                    TestCommon.AssertStandardOutputSettingOutput(output, TestCommon.UsingDoctestMainTestHeaderFilePath);
                }
                else
                {
                    Assert.IsTrue(string.IsNullOrEmpty(output));
                }

                Assert.HasCount(50, capturedTestCases.Values);

                TestCommon.AssertTestCases(capturedTestCases.Values.ToList(),
                    TestCommon.UsingDoctestMainExecutableFilePath,
                    "UsingDoctestMain",
                    TestCommon.UsingDoctestMainTestHeaderFilePath);
            });
        }

        [TestMethod]
        public void DiscoverExe() =>
            UsingDoctestMainExe(null, false);

        [TestMethod]
        public void DiscoverExeAndDLL()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                IEnumerable<string> sources = new List<string>() { TestCommon.ExecutableUsingDLLExecutableFilePath };
                IRunContext discoveryContext = A.Fake<IRunContext>();
                IMessageLogger messageLogger = A.Fake<IMessageLogger>();
                ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();
                Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
                A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCases._))
                    .DoesNothing();

                ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
                doctestTestDiscoverer.DiscoverTests(sources, discoveryContext, messageLogger, testCaseDiscoverySink);

                Assert.HasCount(100, capturedTestCases.Values);

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
            });
        }

        [TestMethod]
        public void DiscoverExeWithPrintStandardOutputSetting() =>
            UsingDoctestMainExe(TestCommon.GeneralRunSettingsPrintStandardOutputExample, true);
    }
}
