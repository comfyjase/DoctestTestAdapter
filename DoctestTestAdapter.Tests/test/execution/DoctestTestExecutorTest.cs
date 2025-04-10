﻿// DoctestTestExecutorTest.cs
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
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

namespace DoctestTestAdapter.Tests.Execution
{
	[TestClass]
	public class DoctestTestExecutorTest
	{
        [TestMethod]
        public void ExecuteExeWithEmptyTestSuites()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
                IRunContext runContext = A.Fake<IRunContext>();
                Captured<TestMessageLevel> capturedTestMessageLevels = A.Captured<TestMessageLevel>();
                Captured<string> capturedTestMessages = A.Captured<string>();
                Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
                Captured<TestResult> capturedTestResults = A.Captured<TestResult>();
                A.CallTo(() => frameworkHandle.SendMessage(capturedTestMessageLevels._, capturedTestMessages._))
                   .DoesNothing();
                A.CallTo(() => frameworkHandle.RecordStart(capturedTestCases._))
                    .DoesNothing();
                A.CallTo(() => frameworkHandle.RecordResult(capturedTestResults._))
                    .DoesNothing();

                List<TestCase> testCases = new TestCaseFactory(TestCommon.EmptySuitesExecutableFilePath, null, runContext, frameworkHandle).CreateTestCases();
                Assert.HasCount(1, testCases);

                TestCommon.AssertTestCase(testCases[0],
                    TestCommon.EmptySuitesExecutableFilePath,
                    "TestNamespace::Empty Class::[TestCase] - Valid",
                    "[TestCase] - Valid",
                    TestCommon.EmptyTestSuitesTestHeaderFilePath,
                    7);

                ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
                doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

                Assert.HasCount(1, capturedTestResults.Values);
                Assert.AreEqual(TestOutcome.Passed, capturedTestResults.Values[0].Outcome);
            });
        }

        [TestMethod]
        public void ExecuteExeWithOnlyTestCases()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
                IRunContext runContext = A.Fake<IRunContext>();
                Captured<TestMessageLevel> capturedTestMessageLevels = A.Captured<TestMessageLevel>();
                Captured<string> capturedTestMessages = A.Captured<string>();
                Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
                Captured<TestResult> capturedTestResults = A.Captured<TestResult>();
                A.CallTo(() => frameworkHandle.SendMessage(capturedTestMessageLevels._, capturedTestMessages._))
                   .DoesNothing();
                A.CallTo(() => frameworkHandle.RecordStart(capturedTestCases._))
                    .DoesNothing();
                A.CallTo(() => frameworkHandle.RecordResult(capturedTestResults._))
                    .DoesNothing();

                List<TestCase> testCases = new TestCaseFactory(TestCommon.OnlyTestCasesExecutableFilePath, null, runContext, frameworkHandle).CreateTestCases();
                Assert.HasCount(3, testCases);

                TestCommon.AssertTestCase(testCases[0],
                    TestCommon.OnlyTestCasesExecutableFilePath,
                    "Empty Namespace::Empty Class::[TestCasesOnly] - Test One",
                    "[TestCasesOnly] - Test One",
                    TestCommon.OnlyTestCasesTestHeaderFilePath,
                    5);

                TestCommon.AssertTestCase(testCases[1],
                    TestCommon.OnlyTestCasesExecutableFilePath,
                    "Empty Namespace::Empty Class::[TestCasesOnly] - Test Two",
                    "[TestCasesOnly] - Test Two",
                    TestCommon.OnlyTestCasesTestHeaderFilePath,
                    10);

                TestCommon.AssertTestCase(testCases[2],
                    TestCommon.OnlyTestCasesExecutableFilePath,
                    "Empty Namespace::Empty Class::[TestCasesOnly] - Test Three",
                    "[TestCasesOnly] - Test Three",
                    TestCommon.OnlyTestCasesTestHeaderFilePath,
                    15);

                ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
                doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

                Assert.HasCount(3, capturedTestResults.Values);
                Assert.AreEqual(TestOutcome.Passed, capturedTestResults.Values[0].Outcome);
            });
        }

        private void UsingDoctestMainExe(string settingsAsString, string expectedExeFileName, bool assertTestResults, bool shouldExpectToPrintStandardOutput)
        {
            TestCommon.AssertErrorOutput(() =>
            {
                IRunContext runContext = A.Fake<IRunContext>();
                IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
                Captured<TestMessageLevel> capturedTestMessageLevels = A.Captured<TestMessageLevel>();
                Captured<string> capturedTestMessages = A.Captured<string>();
                Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
                Captured<TestResult> capturedTestResults = A.Captured<TestResult>();
                A.CallTo(() => frameworkHandle.SendMessage(capturedTestMessageLevels._, capturedTestMessages._))
                   .DoesNothing();
                A.CallTo(() => frameworkHandle.RecordStart(capturedTestCases._))
                    .DoesNothing();
                A.CallTo(() => frameworkHandle.RecordResult(capturedTestResults._))
                    .DoesNothing();
                A.CallTo(() => runContext.IsBeingDebugged)
                    .Returns(false);

                DoctestTestSettings doctestTestSettings = null;
                if (!string.IsNullOrEmpty(settingsAsString))
                {
                    DoctestTestSettingsProvider settingsProvider = new DoctestTestSettingsProvider();
                    doctestTestSettings = TestCommon.LoadDoctestSettings(settingsProvider, settingsAsString);
                    A.CallTo(() => runContext.RunSettings.GetSettings(DoctestTestSettings.RunSettingsXmlNode))
                        .Returns(settingsProvider);
                }

                List<TestCase> testCases = null;

                string output = string.Empty;

                using (StringWriter stringWriter = new StringWriter())
                {
                    TextWriter previousWriter = Console.Out;

                    Console.SetOut(stringWriter);

                    testCases = new TestCaseFactory(TestCommon.UsingDoctestMainExecutableFilePath, doctestTestSettings, runContext, frameworkHandle).CreateTestCases();

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

                Assert.HasCount(50, testCases);

                Assert.HasCount(3, capturedTestMessageLevels.Values);
                Assert.HasCount(3, capturedTestMessages.Values);

                TestCommon.AssertTestCases(testCases,
                    TestCommon.UsingDoctestMainExecutableFilePath,
                    "UsingDoctestMain",
                    TestCommon.UsingDoctestMainTestHeaderFilePath);

                ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
                doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

                Assert.HasCount(4, capturedTestMessageLevels.Values);
                Assert.HasCount(4, capturedTestMessages.Values);
                foreach (TestMessageLevel testMessageLevel in capturedTestMessageLevels.Values)
                    Assert.AreEqual(TestMessageLevel.Informational, testMessageLevel);
                Assert.IsTrue(capturedTestMessages.Values[0].Contains(Shared.Helpers.Constants.InformationMessagePrefix + " - Found macro "));
                Assert.IsTrue(capturedTestMessages.Values[3].Contains(Shared.Helpers.Constants.InformationMessagePrefix + " - About to start exe " + expectedExeFileName + " with command arguments: "));

                if (assertTestResults)
                {
                    Assert.HasCount(50, capturedTestResults.Values);
                    TestCommon.AssertTestResults(capturedTestResults.Values.ToList());
                }
            });
        }

        [TestMethod]
		public void ExecuteExe() => 
            UsingDoctestMainExe(string.Empty, "UsingDoctestMain.exe", true, false);

        [TestMethod]
        public void ExecuteExeAndDLL()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
                IRunContext runContext = A.Fake<IRunContext>();
                Captured<TestMessageLevel> capturedTestMessageLevels = A.Captured<TestMessageLevel>();
                Captured<string> capturedTestMessages = A.Captured<string>();
                Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
                Captured<TestResult> capturedTestResults = A.Captured<TestResult>();
                A.CallTo(() => frameworkHandle.SendMessage(capturedTestMessageLevels._, capturedTestMessages._))
                   .DoesNothing();
                A.CallTo(() => frameworkHandle.RecordStart(capturedTestCases._))
                    .DoesNothing();
                A.CallTo(() => frameworkHandle.RecordResult(capturedTestResults._))
                    .DoesNothing();

                List<TestCase> testCases = new TestCaseFactory(TestCommon.ExecutableUsingDLLExecutableFilePath, null, runContext, frameworkHandle).CreateTestCases();
                Assert.HasCount(100, testCases);

                Assert.HasCount(6, capturedTestMessageLevels.Values);
                Assert.HasCount(6, capturedTestMessages.Values);

                List<TestCase> dllTestCases = testCases
                    .ToList()
                    .Where(t => t.DisplayName.Contains("[DLL]"))
                    .ToList();
                List<TestCase> executableUsingDLLTestCases = testCases
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

                ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
                doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

                Assert.HasCount(8, capturedTestMessageLevels.Values);
                Assert.HasCount(8, capturedTestMessages.Values);
                foreach (TestMessageLevel testMessageLevel in capturedTestMessageLevels.Values)
                    Assert.AreEqual(TestMessageLevel.Informational, testMessageLevel);
                Assert.IsTrue(capturedTestMessages.Values[0].Contains(Shared.Helpers.Constants.InformationMessagePrefix + " - Found macro "));
                Assert.IsTrue(capturedTestMessages.Values[7].Contains(Shared.Helpers.Constants.InformationMessagePrefix + " - About to start exe ExecutableUsingDLL.exe with command arguments: "));

                Assert.HasCount(100, capturedTestResults.Values);
                List<TestResult> dllTestResults = capturedTestResults.Values
                    .Where(t => t.TestCase.DisplayName.Contains("[DLL]"))
                    .ToList();
                List<TestResult> executableUsingDLLTestResults = capturedTestResults.Values
                    .Where(t => t.TestCase.DisplayName.Contains("[ExecutableUsingDLL]"))
                    .ToList();
                TestCommon.AssertTestResults(dllTestResults);
                TestCommon.AssertTestResults(executableUsingDLLTestResults);
            });
        }

        [TestMethod]
        public void ExecuteExeWithExeOverrideSetting() =>
            UsingDoctestMainExe(TestCommon.ExecutorRunSettingsRelativeExecutableOverrideExample, "UsingCustomMain.exe", false, false);

        [TestMethod]
        public void ExecuteExeWithPrintStandardOutputSetting() =>
            UsingDoctestMainExe(TestCommon.GeneralRunSettingsPrintStandardOutputExample, "UsingDoctestMain.exe", false, true);
    }
}
