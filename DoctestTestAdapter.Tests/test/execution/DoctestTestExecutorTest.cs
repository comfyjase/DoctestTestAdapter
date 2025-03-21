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
        private void UsingDoctestMainExe(string settingsAsString, string expectedExeFileName, bool assertTestResults, bool shouldExpectToPrintStandardOutput)
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

                testCases = Utilities.GetTestCases(TestCommon.UsingDoctestMainExecutableFilePath, frameworkHandle, doctestTestSettings);

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

            Assert.HasCount(25, testCases);

            Assert.IsEmpty(capturedTestMessageLevels.Values);
            Assert.IsEmpty(capturedTestMessages.Values);

            TestCommon.AssertTestCases(testCases,
                TestCommon.UsingDoctestMainExecutableFilePath,
                "UsingDoctestMain",
                TestCommon.UsingDoctestMainTestHeaderFilePath);          

            ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
            doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

            Assert.HasCount(1, capturedTestMessageLevels.Values);
            Assert.HasCount(1, capturedTestMessages.Values);
            Assert.AreEqual(TestMessageLevel.Informational, capturedTestMessageLevels.Values[0]);
            Assert.IsTrue(capturedTestMessages.Values[0].Contains(Shared.Helpers.Constants.InformationMessagePrefix + " - About to start exe " + expectedExeFileName + " with command arguments: "));

            if (assertTestResults)
            {
                Assert.HasCount(25, capturedTestResults.Values);
                TestCommon.AssertTestResults(capturedTestResults.Values.ToList());
            }
        }

        [TestMethod]
		public void ExecuteExe() => 
            UsingDoctestMainExe(string.Empty, "UsingDoctestMain.exe", true, false);

        [TestMethod]
        public void ExecuteExeAndDLL()
        {
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            Captured<TestMessageLevel> capturedTestMessageLevels = A.Captured<TestMessageLevel>();
            Captured<string> capturedTestMessages = A.Captured<string>();
            A.CallTo(() => frameworkHandle.SendMessage(capturedTestMessageLevels._, capturedTestMessages._))
               .DoesNothing();

            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.ExecutableUsingDLLExecutableFilePath, frameworkHandle);
            Assert.HasCount(50, testCases);

            Assert.IsEmpty(capturedTestMessageLevels.Values);
            Assert.IsEmpty(capturedTestMessages.Values);

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

            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
            Captured<TestResult> capturedTestResults = A.Captured<TestResult>();
            IRunContext runContext = A.Fake<IRunContext>();
            A.CallTo(() => frameworkHandle.RecordStart(capturedTestCases._))
                .DoesNothing();
            A.CallTo(() => frameworkHandle.RecordResult(capturedTestResults._))
                .DoesNothing();

            ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
            doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

            Assert.HasCount(1, capturedTestMessageLevels.Values);
            Assert.HasCount(1, capturedTestMessages.Values);
            Assert.AreEqual(TestMessageLevel.Informational, capturedTestMessageLevels.Values[0]);
            Assert.IsTrue(capturedTestMessages.Values[0].Contains(Shared.Helpers.Constants.InformationMessagePrefix + " - About to start exe ExecutableUsingDLL.exe with command arguments: "));

            Assert.HasCount(50, capturedTestResults.Values);
            List<TestResult> dllTestResults = capturedTestResults.Values
                .Where(t => t.TestCase.DisplayName.Contains("[DLL]"))
                .ToList();
            List<TestResult> executableUsingDLLTestResults = capturedTestResults.Values
                .Where(t => t.TestCase.DisplayName.Contains("[ExecutableUsingDLL]"))
                .ToList();
            TestCommon.AssertTestResults(dllTestResults);
            TestCommon.AssertTestResults(executableUsingDLLTestResults);
        }

        [TestMethod]
        public void ExecuteExeWithExeOverrideSetting() =>
            UsingDoctestMainExe(TestCommon.ExecutorRunSettingsRelativeExecutableOverrideExample, "UsingCustomMain.exe", false, false);

        [TestMethod]
        public void ExecuteExeWithPrintStandardOutputSetting() =>
            UsingDoctestMainExe(TestCommon.GeneralRunSettingsPrintStandardOutputExample, "UsingDoctestMain.exe", false, true);
    }
}
