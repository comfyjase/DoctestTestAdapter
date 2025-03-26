// GodotDoctestTestExecutorTest.cs
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
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using DoctestTestAdapter.Shared.Factory;

namespace DoctestTestAdapter.Tests.Godot.Execution
{
    [TestClass]
    public class GodotDoctestTestExecutorTest : GodotTest
    {
        [TestMethod]
        public void ExecuteExe()
        {
            IEnumerable<string> sources = new List<string>() { TestCommon.GodotExecutableFilePath };

            DoctestTestSettingsProvider doctestTestSettingsProvider = new DoctestTestSettingsProvider();
            AssertAndLoadExampleRunSettings(doctestTestSettingsProvider);

            IRunContext runContext = A.Fake<IRunContext>();
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            Captured<TestMessageLevel> capturedTestMessageLevels = A.Captured<TestMessageLevel>();
            Captured<string> capturedTestMessages = A.Captured<string>();
            Captured<TestCase> capturedTestCasesFromExecutor = A.Captured<TestCase>();
            Captured<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult> capturedTestResults = A.Captured<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult>();
            A.CallTo(() => runContext.RunSettings.GetSettings(DoctestTestSettings.RunSettingsXmlNode))
                .Returns(doctestTestSettingsProvider);
            A.CallTo(() => runContext.IsBeingDebugged)
                .Returns(false);
            A.CallTo(() => frameworkHandle.SendMessage(capturedTestMessageLevels._, capturedTestMessages._))
               .DoesNothing();
            A.CallTo(() => frameworkHandle.RecordStart(capturedTestCasesFromExecutor._))
                .DoesNothing();
            A.CallTo(() => frameworkHandle.RecordResult(capturedTestResults._))
                .DoesNothing();

            List<TestCase> testCases = new TestCaseFactory(TestCommon.GodotExecutableFilePath, settings, runContext, frameworkHandle).CreateTestCases();
            Assert.IsNotEmpty(testCases);

            AssertMissingTestCases(runContext, testCases);

            ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
            doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);
            Assert.AreEqual(testCases.Count, capturedTestCasesFromExecutor.Values.Count);
            Assert.AreEqual(testCases.Count, capturedTestResults.Values.Count);
        }
    }
}
