// DoctestTestExecutorTest.cs
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

        private void UsingDoctestMainExe(string settingsAsString, string expectedExeFileName, bool assertTestResults, bool shouldExpectToPrintDebugLogs)
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

                if (shouldExpectToPrintDebugLogs)
                {
                    TestCommon.AssertEnableDebugLogsSettingOutput(string.Join("\n", capturedTestMessages.Values), TestCommon.UsingDoctestMainTestHeaderFilePath);
                }
                else
                {
                    Assert.IsTrue(string.IsNullOrEmpty(output));
                }

                Assert.HasCount(50, testCases);

                TestCommon.AssertTestCases(testCases,
                    TestCommon.UsingDoctestMainExecutableFilePath,
                    "UsingDoctestMain",
                    TestCommon.UsingDoctestMainTestHeaderFilePath);

                ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
                doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

                foreach (TestMessageLevel testMessageLevel in capturedTestMessageLevels.Values)
                    Assert.AreEqual(TestMessageLevel.Informational, testMessageLevel);

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

                foreach (TestMessageLevel testMessageLevel in capturedTestMessageLevels.Values)
                    Assert.AreEqual(TestMessageLevel.Informational, testMessageLevel);

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
        public void ExecuteExeWithEnableDebugLogsSetting() =>
            UsingDoctestMainExe(TestCommon.GeneralRunSettingsEnableDebugLogsExample, "UsingDoctestMain.exe", false, true);

        [TestMethod]
        public void ExecuteExeWithInfoAndMessageOutput()
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

                List<TestCase> testCases = new TestCaseFactory(TestCommon.PrintOutputExecutableFilePath, null, runContext, frameworkHandle).CreateTestCases();
                Assert.HasCount(14, testCases);

                ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
                doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

                foreach (TestMessageLevel testMessageLevel in capturedTestMessageLevels.Values)
                    Assert.AreEqual(TestMessageLevel.Informational, testMessageLevel);

                Assert.IsTrue(string.Join("\n", capturedTestResults.Values[0].ErrorMessage).Contains("INFO called for test that will fail"));
                Assert.IsFalse(string.Join("\n", capturedTestResults.Values[1].ErrorMessage).Contains("INFO should not be called for this test that will pass"));
                Assert.IsTrue(string.Join("\n", capturedTestResults.Values[2].ErrorMessage).Contains("INFO called for test that will fail with variable: 11"));
                Assert.IsTrue(string.Join("\n", capturedTestResults.Values[3].ErrorMessage).Contains("INFO called for test that will fail with variable: 11"));
                Assert.IsTrue(string.Join("\n", capturedTestResults.Values[3].ErrorMessage).Contains("Another INFO called for test that will fail with another_variable: 15"));
                Assert.IsTrue(capturedTestResults.Values[4].Messages[0].Text.Contains("[PrintOutput] - MESSAGE"));
                Assert.IsTrue(capturedTestResults.Values[4].Messages[1].Text.Contains("MESSAGE called before check"));
                Assert.IsTrue(capturedTestResults.Values[4].Messages[2].Text.Contains("MESSAGE called after check"));
                Assert.IsTrue(capturedTestResults.Values[5].Messages[0].Text.Contains("[PrintOutput] - MESSAGE With Variable"));
                Assert.IsTrue(capturedTestResults.Values[5].Messages[1].Text.Contains("MESSAGE called before check with variable: 38"));
                Assert.IsTrue(capturedTestResults.Values[5].Messages[2].Text.Contains("MESSAGE called after check with variable: 38"));
                Assert.IsTrue(capturedTestResults.Values[6].Messages[0].Text.Contains("[PrintOutput] - Multiple MESSAGEs With Variable"));
                Assert.IsTrue(capturedTestResults.Values[6].Messages[1].Text.Contains("MESSAGE called before check with variable: 38"));
                Assert.IsTrue(capturedTestResults.Values[6].Messages[2].Text.Contains("Another MESSAGE called before check with another_variable: 5"));
                Assert.IsTrue(capturedTestResults.Values[6].Messages[3].Text.Contains("MESSAGE called after check with variable: 38"));
                Assert.IsTrue(capturedTestResults.Values[6].Messages[4].Text.Contains("Another MESSAGE called after check with another_variable: 5"));
                Assert.IsTrue(string.Join("\n", capturedTestResults.Values[7].ErrorMessage).Contains("CHECK_MESSAGE called for failing test."));
                Assert.IsTrue(string.Join("\n", capturedTestResults.Values[8].ErrorMessage).Contains("REQUIRE_MESSAGE called for failing test"));
                Assert.IsTrue(string.Join("\n", capturedTestResults.Values[9].ErrorMessage).Contains("FAIL called for failing test"));
                Assert.IsTrue(string.Join("\n", capturedTestResults.Values[10].ErrorMessage).Contains("FAIL_CHECK called for failing test"));
                Assert.IsTrue(capturedTestResults.Values[12].Messages[0].Text.Contains("[PrintOutput] - With Passing SUBCASES"));
                Assert.IsTrue(capturedTestResults.Values[12].Messages[1].Text.Contains("Message from TEST_CASE parent!"));
                Assert.IsTrue(capturedTestResults.Values[12].Messages[2].Text.Contains("[PrintOutput] - Passing SUBCASE 1"));
                Assert.IsTrue(capturedTestResults.Values[12].Messages[3].Text.Contains("Message from SUBCASE 1!"));
                Assert.IsTrue(capturedTestResults.Values[12].Messages[4].Text.Contains("[PrintOutput] - Passing SUBCASE 2"));
                Assert.IsTrue(capturedTestResults.Values[12].Messages[5].Text.Contains("Message from SUBCASE 2!"));
                Assert.IsTrue(capturedTestResults.Values[12].Messages[6].Text.Contains("[PrintOutput] - Nested SUBCASEs"));
                Assert.IsTrue(capturedTestResults.Values[12].Messages[7].Text.Contains("Message from Nested SUBCASE parent!"));
                Assert.IsTrue(capturedTestResults.Values[12].Messages[8].Text.Contains("[PrintOutput] - Nested SUBCASE 1"));
                Assert.IsTrue(capturedTestResults.Values[12].Messages[9].Text.Contains("Message from Nested SUBCASE 1!"));
                Assert.IsTrue(capturedTestResults.Values[12].Messages[10].Text.Contains("[PrintOutput] - Nested SUBCASE 2"));
                Assert.IsTrue(capturedTestResults.Values[12].Messages[11].Text.Contains("Message from Nested SUBCASE 2!"));

                string failingSubcasesErrorMessages = string.Join("\n", capturedTestResults.Values[13].ErrorMessage);
                Assert.IsTrue(failingSubcasesErrorMessages.Contains("[PrintOutput] - Failing SUBCASE 1"));
                Assert.IsTrue(failingSubcasesErrorMessages.Contains("Some info when failing from TEST_CASE parent!"));
                Assert.IsTrue(failingSubcasesErrorMessages.Contains("Info from SUBCASE 1!"));
                Assert.IsTrue(failingSubcasesErrorMessages.Contains("[PrintOutput] - Failing SUBCASE 2"));
                Assert.IsTrue(failingSubcasesErrorMessages.Contains("Info from SUBCASE 2!"));
                Assert.IsTrue(failingSubcasesErrorMessages.Contains("[PrintOutput] - Nested SUBCASE 1"));
                Assert.IsTrue(failingSubcasesErrorMessages.Contains("Info from Nested SUBCASE parent!"));
                Assert.IsTrue(failingSubcasesErrorMessages.Contains("Info from Nested SUBCASE 1!"));
                Assert.IsTrue(failingSubcasesErrorMessages.Contains("[PrintOutput] - Nested SUBCASE 2"));
                Assert.IsTrue(failingSubcasesErrorMessages.Contains("Info from Nested SUBCASE 2!"));
            });
        }

        [TestMethod]
        public void ExecuteExeWithTestsOnlyInHFiles()
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

                List<TestCase> testCases = new TestCaseFactory(TestCommon.TestsOnlyInHFilesExecutableFilePath, null, runContext, frameworkHandle).CreateTestCases();
                Assert.HasCount(1, testCases);

                TestCommon.AssertTestCase(testCases[0],
                    TestCommon.TestsOnlyInHFilesExecutableFilePath,
                    "Empty Namespace::Empty Class::[TestsOnlyInHFiles] - Is Even",
                    "[TestsOnlyInHFiles] - Is Even",
                    TestCommon.TestsOnlyInHFilesTestHeaderFilePath,
                    10);

                ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
                doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

                Assert.HasCount(1, capturedTestResults.Values);
                Assert.AreEqual(TestOutcome.Passed, capturedTestResults.Values[0].Outcome);
            });
        }

        [TestMethod]
        public void ExecuteExeWithTestsOnlyInHPPFiles()
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

                List<TestCase> testCases = new TestCaseFactory(TestCommon.TestsOnlyInHPPFilesExecutableFilePath, null, runContext, frameworkHandle).CreateTestCases();
                Assert.HasCount(1, testCases);

                TestCommon.AssertTestCase(testCases[0],
                    TestCommon.TestsOnlyInHPPFilesExecutableFilePath,
                    "Empty Namespace::Empty Class::[TestsOnlyInHPPFiles] - Is Even",
                    "[TestsOnlyInHPPFiles] - Is Even",
                    TestCommon.TestsOnlyInHPPFilesTestHeaderFilePath,
                    10);

                ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
                doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

                Assert.HasCount(1, capturedTestResults.Values);
                Assert.AreEqual(TestOutcome.Passed, capturedTestResults.Values[0].Outcome);
            });
        }

        [TestMethod]
        public void ExecuteExeWithTestsOnlyInCPPFiles()
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

                List<TestCase> testCases = new TestCaseFactory(TestCommon.TestsOnlyInCPPFilesExecutableFilePath, null, runContext, frameworkHandle).CreateTestCases();
                Assert.HasCount(1, testCases);

                TestCommon.AssertTestCase(testCases[0],
                    TestCommon.TestsOnlyInCPPFilesExecutableFilePath,
                    "Empty Namespace::Empty Class::[TestsOnlyInCPPFiles] - Is Even",
                    "[TestsOnlyInCPPFiles] - Is Even",
                    TestCommon.TestsOnlyInCPPFilesTestHeaderFilePath,
                    8);

                ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
                doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

                Assert.HasCount(1, capturedTestResults.Values);
                Assert.AreEqual(TestOutcome.Passed, capturedTestResults.Values[0].Outcome);
            });
        }

        [TestMethod]
        public void ExecuteExeWithSpecialCharactersExample()
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

                DoctestTestSettings doctestTestSettings = null;
                if (!string.IsNullOrEmpty(TestCommon.RunSettingsWithSpecialCharactersExample))
                {
                    DoctestTestSettingsProvider settingsProvider = new DoctestTestSettingsProvider();
                    doctestTestSettings = TestCommon.LoadDoctestSettings(settingsProvider, TestCommon.RunSettingsWithSpecialCharactersExample);
                    A.CallTo(() => runContext.RunSettings.GetSettings(DoctestTestSettings.RunSettingsXmlNode))
                        .Returns(settingsProvider);
                }

                List<TestCase> testCases = new TestCaseFactory(TestCommon.SpecialCharactersExecutableFilePath, doctestTestSettings, runContext, frameworkHandle).CreateTestCases();
                Assert.HasCount(5, testCases);

                TestCommon.AssertTestCase(testCases[0],
                    TestCommon.SpecialCharactersExecutableFilePath,
                    "Empty Namespace::Empty Class::[SpecialCharactersInFolderPath] - Is Even",
                    "[SpecialCharactersInFolderPath] - Is Even",
                    TestCommon.SpecialCharactersHeaderFilePath,
                    10);

                ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
                doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

                Assert.HasCount(5, capturedTestResults.Values);
                Assert.AreEqual(TestOutcome.Passed, capturedTestResults.Values[0].Outcome);
            });
        }

        [TestMethod]
        public void ExecuteExeWithRootDirectorySetting()
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

                // Using utilities root directory to make sure I don't hardcode path
                string rootDirectory = Utilities.GetRootDirectory();
                string settingsAsString = TestCommon.GeneralRunSettingsExample;
                settingsAsString = settingsAsString.Replace("C:/Just/An/Example/Path", rootDirectory);

                DoctestTestSettings doctestTestSettings = null;
                if (!string.IsNullOrEmpty(settingsAsString))
                {
                    DoctestTestSettingsProvider settingsProvider = new DoctestTestSettingsProvider();
                    doctestTestSettings = TestCommon.LoadDoctestSettings(settingsProvider, settingsAsString);
                    A.CallTo(() => runContext.RunSettings.GetSettings(DoctestTestSettings.RunSettingsXmlNode))
                        .Returns(settingsProvider);
                }

                List<TestCase> testCases = new TestCaseFactory(TestCommon.TestsOnlyInHFilesExecutableFilePath, doctestTestSettings, runContext, frameworkHandle).CreateTestCases();
                Assert.HasCount(1, testCases);

                TestCommon.AssertTestCase(testCases[0],
                    TestCommon.TestsOnlyInHFilesExecutableFilePath,
                    "Empty Namespace::Empty Class::[TestsOnlyInHFiles] - Is Even",
                    "[TestsOnlyInHFiles] - Is Even",
                    TestCommon.TestsOnlyInHFilesTestHeaderFilePath,
                    10);

                ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
                doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

                Assert.HasCount(1, capturedTestResults.Values);
                Assert.AreEqual(TestCommon.TestsOnlyInHFilesTestHeaderFilePath, capturedTestCases.Values[0].CodeFilePath);
                Assert.AreEqual(TestOutcome.Passed, capturedTestResults.Values[0].Outcome);
            });
        }
    }
}
