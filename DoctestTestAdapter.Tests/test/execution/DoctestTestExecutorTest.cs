using DoctestTestAdapter.Shared.Helpers;
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

namespace DoctestTestAdapter.Tests.Execution
{
	[TestClass]
	public class DoctestTestExecutorTest
	{
        [TestMethod]
		public void ExecuteExe()
		{
			List<TestCase> testCases = Utilities.GetTestCases(TestCommon.UsingDoctestMainExecutableFilePath);
            Assert.IsTrue(testCases.Count == 4);

            TestCase failedTestCase = testCases[1];
            TestCommon.AssertTestCase(failedTestCase,
                    TestCommon.UsingDoctestMainExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Fail",
                    "[UsingDoctestMain] Testing IsEven Always Fail",
                    TestCommon.UsingDoctestMainTestHeaderFile,
                    57);

            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
            Captured<TestResult> capturedTestResults = A.Captured<TestResult>();
            IRunContext runContext = A.Fake<IRunContext>();
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            A.CallTo(() => frameworkHandle.RecordStart(capturedTestCases._))
                .DoesNothing();
            A.CallTo(() => frameworkHandle.RecordResult(capturedTestResults._))
                .DoesNothing();

            ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
            doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

            Assert.IsTrue(capturedTestResults.Values.Count == 4);
            Assert.IsTrue(capturedTestResults.Values[0].Outcome == TestOutcome.Passed);
            Assert.IsTrue(capturedTestResults.Values[1].Outcome == TestOutcome.Failed);
            Assert.IsTrue(!string.IsNullOrEmpty(capturedTestResults.Values[1].ErrorMessage));
            Assert.IsTrue(capturedTestResults.Values[1].ErrorMessage.Contains("CHECK( IsEven(1) ) is NOT correct!"));
            Assert.IsTrue(capturedTestResults.Values[1].ErrorMessage.Contains("CHECK( IsEven(3) ) is NOT correct!"));
            Assert.IsTrue(capturedTestResults.Values[1].ErrorMessage.Contains("CHECK( IsEven(5) ) is NOT correct!"));
            Assert.IsTrue(capturedTestResults.Values[2].Outcome == TestOutcome.Skipped);
            Assert.IsTrue(capturedTestResults.Values[3].Outcome == TestOutcome.Passed);
        }

        [TestMethod]
        public void ExecuteExeAndDLL()
        {
            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.ExecutableUsingDLLExecutableFilePath);
            Assert.IsTrue(testCases.Count == 8);

            TestCase dllFailedTestCase = testCases[1];
            TestCommon.AssertTestCase(dllFailedTestCase,
                    TestCommon.ExecutableUsingDLLExecutableFilePath,
                    "TestDLL::Empty Class::[DLL] Testing IsEven Always Fail",
                    "[DLL] Testing IsEven Always Fail",
                    TestCommon.DLLTestHeaderFile,
                    53);

            TestCase executableUsingDLLFailedTestCase = testCases[5];
            TestCommon.AssertTestCase(executableUsingDLLFailedTestCase,
                    TestCommon.ExecutableUsingDLLExecutableFilePath,
                    "TestExecutableUsingDLL::Empty Class::[ExecutableUsingDLL] Testing IsEven Always Fail",
                    "[ExecutableUsingDLL] Testing IsEven Always Fail",
                    TestCommon.ExecutableUsingDLLTestHeaderFile,
                    53);

            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
            Captured<TestResult> capturedTestResults = A.Captured<TestResult>();
            IRunContext runContext = A.Fake<IRunContext>();
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            A.CallTo(() => frameworkHandle.RecordStart(capturedTestCases._))
                .DoesNothing();
            A.CallTo(() => frameworkHandle.RecordResult(capturedTestResults._))
                .DoesNothing();

            ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
            doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

            Assert.IsTrue(capturedTestResults.Values.Count == 8);

            // DLL Test Checks
            Assert.IsTrue(capturedTestResults.Values[0].Outcome == TestOutcome.Passed);
            Assert.IsTrue(capturedTestResults.Values[1].Outcome == TestOutcome.Failed);
            Assert.IsTrue(!string.IsNullOrEmpty(capturedTestResults.Values[1].ErrorMessage));
            Assert.IsTrue(capturedTestResults.Values[1].ErrorMessage.Contains("CHECK( IsEven(7) ) is NOT correct!"));
            Assert.IsTrue(capturedTestResults.Values[1].ErrorMessage.Contains("CHECK( IsEven(9) ) is NOT correct!"));
            Assert.IsTrue(capturedTestResults.Values[1].ErrorMessage.Contains("CHECK( IsEven(11) ) is NOT correct!"));
            Assert.IsTrue(capturedTestResults.Values[2].Outcome == TestOutcome.Skipped);
            Assert.IsTrue(capturedTestResults.Values[3].Outcome == TestOutcome.Passed);

            // Exe Test Checks
            Assert.IsTrue(capturedTestResults.Values[4].Outcome == TestOutcome.Passed);
            Assert.IsTrue(capturedTestResults.Values[5].Outcome == TestOutcome.Failed);
            Assert.IsTrue(!string.IsNullOrEmpty(capturedTestResults.Values[5].ErrorMessage));
            Assert.IsTrue(capturedTestResults.Values[5].ErrorMessage.Contains("CHECK( IsEven(1) ) is NOT correct!"));
            Assert.IsTrue(capturedTestResults.Values[5].ErrorMessage.Contains("CHECK( IsEven(3) ) is NOT correct!"));
            Assert.IsTrue(capturedTestResults.Values[5].ErrorMessage.Contains("CHECK( IsEven(5) ) is NOT correct!"));
            Assert.IsTrue(capturedTestResults.Values[6].Outcome == TestOutcome.Skipped);
            Assert.IsTrue(capturedTestResults.Values[7].Outcome == TestOutcome.Passed);
        }
	}
}
