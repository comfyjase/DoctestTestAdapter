// StackTraceTest.cs
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

using DoctestTestAdapter.Shared.Factory;
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

namespace DoctestTestAdapter.Tests.Helpers
{
    [TestClass]
    public class StackTraceTest
    {
        [TestMethod]
        public void PassingTest()
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
                Assert.IsTrue(string.IsNullOrEmpty(capturedTestResults.Values[0].ErrorStackTrace));
            });
        }

        [TestMethod]
        public void FailingTest()
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

                string expectedStackTrace = $"at TEST_CASE(\"[PrintOutput] - INFO\") in {TestCommon.PrintOutputHeaderFilePath}:line 11\n";
                Assert.AreEqual(expectedStackTrace, capturedTestResults.Values[0].ErrorStackTrace);

                expectedStackTrace = $"at TEST_CASE(\"[PrintOutput] - INFO With Variable\") in {TestCommon.PrintOutputHeaderFilePath}:line 24\n";
                Assert.AreEqual(expectedStackTrace, capturedTestResults.Values[2].ErrorStackTrace);

                expectedStackTrace = $"at TEST_CASE(\"[PrintOutput] - CHECK_MESSAGE\") in {TestCommon.PrintOutputHeaderFilePath}:line 64\n";
                Assert.AreEqual(expectedStackTrace, capturedTestResults.Values[7].ErrorStackTrace);

                expectedStackTrace = $"at TEST_CASE(\"[PrintOutput] - REQUIRE_MESSAGE\") in {TestCommon.PrintOutputHeaderFilePath}:line 69\n";
                Assert.AreEqual(expectedStackTrace, capturedTestResults.Values[8].ErrorStackTrace);
                
                expectedStackTrace = $"at TEST_CASE(\"[PrintOutput] - FAIL\") in {TestCommon.PrintOutputHeaderFilePath}:line 74\n";
                Assert.AreEqual(expectedStackTrace, capturedTestResults.Values[9].ErrorStackTrace);

                expectedStackTrace = $"at TEST_CASE(\"[PrintOutput] - FAIL_CHECK\") in {TestCommon.PrintOutputHeaderFilePath}:line 79\n";
                Assert.AreEqual(expectedStackTrace, capturedTestResults.Values[10].ErrorStackTrace);

                expectedStackTrace = $"at SUBCASE(\"[PrintOutput] - Failing SUBCASE 1\") in {TestCommon.PrintOutputHeaderFilePath}:line 132\n"
                + $"at SUBCASE(\"[PrintOutput] - Failing SUBCASE 2\") in {TestCommon.PrintOutputHeaderFilePath}:line 138\n"
                + $"at SUBCASE(\"[PrintOutput] - Nested SUBCASE 1\") in {TestCommon.PrintOutputHeaderFilePath}:line 148\n"
                + $"at SUBCASE(\"[PrintOutput] - Nested SUBCASE 2\") in {TestCommon.PrintOutputHeaderFilePath}:line 154\n";
                Assert.AreEqual(expectedStackTrace, capturedTestResults.Values[13].ErrorStackTrace);
            });
        }
    }
}
