using DoctestTestAdapter.Shared.Helpers;
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

namespace DoctestTestAdapter.Tests.Execution
{
	[TestClass]
	public class DoctestTestExecutorTest
	{
        [TestMethod]
		public void ExecuteExe()
		{
			List<TestCase> testCases = Utilities.GetTestCases(TestCommon.ExampleExecutableFilePath);
            Assert.IsTrue(testCases.Count == 4);

            List<string> sourceFiles = Utilities.GetSourceFiles(TestCommon.ExampleExecutableFilePath);
            Assert.IsTrue(sourceFiles.Count == 1);

            string sourceFile = sourceFiles[0];
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.IsTrue(sourceFile.EndsWith("TestIsEvenUsingDoctestMain.h"));

            TestCase failedTestCase = testCases[1];
            TestCommon.AssertTestCase(failedTestCase,
                    TestCommon.ExampleExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Fail",
                    "[UsingDoctestMain] Testing IsEven Always Fail",
                    sourceFile,
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
            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.ExampleExecutableUsingDLLFilePath);
            Assert.IsTrue(testCases.Count == 8);

            List<string> sourceFiles = Utilities.GetSourceFiles(TestCommon.ExampleExecutableUsingDLLFilePath);
            Assert.IsTrue(sourceFiles.Count == 4);

            string dllTestSourceFile = sourceFiles[2];
            Assert.IsTrue(File.Exists(dllTestSourceFile));
            Assert.IsTrue(dllTestSourceFile.EndsWith("TestIsEvenDLL.h"));

            string executableUsingDLLTestSourceFile = sourceFiles[3];
            Assert.IsTrue(File.Exists(executableUsingDLLTestSourceFile));
            Assert.IsTrue(executableUsingDLLTestSourceFile.EndsWith("TestIsEvenExecutableUsingDLL.h"));

            TestCase dllFailedTestCase = testCases[1];
            TestCommon.AssertTestCase(dllFailedTestCase,
                    TestCommon.ExampleExecutableUsingDLLFilePath,
                    "TestDLL::Empty Class::[DLL] Testing IsEven Always Fail",
                    "[DLL] Testing IsEven Always Fail",
                    dllTestSourceFile,
                    53);

            TestCase executableUsingDLLFailedTestCase = testCases[5];
            TestCommon.AssertTestCase(executableUsingDLLFailedTestCase,
                    TestCommon.ExampleExecutableUsingDLLFilePath,
                    "TestExecutableUsingDLL::Empty Class::[ExecutableUsingDLL] Testing IsEven Always Fail",
                    "[ExecutableUsingDLL] Testing IsEven Always Fail",
                    executableUsingDLLTestSourceFile,
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
