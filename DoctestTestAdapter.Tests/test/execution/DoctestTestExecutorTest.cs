using DoctestTestAdapter.Shared.Helpers;
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

namespace DoctestTestAdapter.Tests.Execution
{
	[TestClass]
	public class DoctestTestExecutorTest
	{
        private string _exampleExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe";
        private string _exampleExecutableUsingDLLFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\ExecutableUsingDLL\\ExecutableUsingDLL.exe";

        [TestMethod]
		public void ExecuteExe()
		{
            //IEnumerable<string> sources = new List<string>() { _exampleExecutableFilePath };
			List<TestCase> testCases = Utilities.GetTestCases(_exampleExecutableFilePath);
            Assert.IsTrue(testCases.Count == 3);

            List<string> sourceFiles = Utilities.GetSourceFiles(_exampleExecutableFilePath);
            Assert.IsTrue(sourceFiles.Count == 1);

            string sourceFile = sourceFiles[0];
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.IsTrue(sourceFile.EndsWith("TestIsEvenUsingDoctestMain.h"));

            TestCase failedTestCase = testCases[1];
            TestCommon.AssertTestCase(failedTestCase,
                    _exampleExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Fail",
                    "[UsingDoctestMain] Testing IsEven Always Fail",
                    sourceFile,
                    19);

            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
            Captured<TestResult> capturedTestResults = A.Captured<TestResult>();
            IRunContext runContext = A.Fake<IRunContext>();
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            A.CallTo(() => frameworkHandle.RecordStart(capturedTestCases._))
                .DoesNothing();
            A.CallTo(() => frameworkHandle.RecordResult(capturedTestResults._))
                .DoesNothing();

            ITestExecutor doctestTestExecutor = new DoctestTestExecutor();

            // Write any console output to a string so we can verify the executor has worked correctly.
            string testExecutorOutput = string.Empty;
            using (StringWriter stringWriter = new StringWriter())
            {
                // Console.Write/WriteLine should write to stringWriter now.
                TextWriter previousWriter = Console.Out;
                Console.SetOut(stringWriter);

                doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

                testExecutorOutput = stringWriter.ToString();

                // Reset back to standard output.
                Console.SetOut(previousWriter);
            }

            Assert.IsTrue(testExecutorOutput.Contains(sourceFile));
            Assert.IsTrue(testExecutorOutput.Contains(failedTestCase.DisplayName));
            Assert.IsTrue(testExecutorOutput.Contains(@"ERROR: CHECK( IsEven(1) ) is NOT correct!"));
            Assert.IsTrue(testExecutorOutput.Contains(@"ERROR: CHECK( IsEven(3) ) is NOT correct!"));
            Assert.IsTrue(testExecutorOutput.Contains(@"ERROR: CHECK( IsEven(5) ) is NOT correct!"));

            string doctestFinalOutput = @"[doctest] test cases: 2 | 1 passed | 1 failed | 1 skipped
[doctest] assertions: 6 | 3 passed | 3 failed |
[doctest] Status: FAILURE!";

            Assert.IsTrue(testExecutorOutput.Contains(doctestFinalOutput));
        }

        [TestMethod]
        public void ExecuteExeAndDLL()
        {
            //IEnumerable<string> sources = new List<string>() { _exampleExecutableUsingDLLFilePath };
            List<TestCase> testCases = Utilities.GetTestCases(_exampleExecutableUsingDLLFilePath);
            Assert.IsTrue(testCases.Count == 6);

            List<string> sourceFiles = Utilities.GetSourceFiles(_exampleExecutableUsingDLLFilePath);
            Assert.IsTrue(sourceFiles.Count == 4);

            string dllTestSourceFile = sourceFiles[2];
            Assert.IsTrue(File.Exists(dllTestSourceFile));
            Assert.IsTrue(dllTestSourceFile.EndsWith("TestIsEvenDLL.h"));

            string executableUsingDLLTestSourceFile = sourceFiles[3];
            Assert.IsTrue(File.Exists(executableUsingDLLTestSourceFile));
            Assert.IsTrue(executableUsingDLLTestSourceFile.EndsWith("TestIsEvenExecutableUsingDLL.h"));

            TestCase dllFailedTestCase = testCases[1];
            TestCommon.AssertTestCase(dllFailedTestCase,
                    _exampleExecutableUsingDLLFilePath,
                    "TestDLL::Empty Class::[DLL] Testing IsEven Always Fail",
                    "[DLL] Testing IsEven Always Fail",
                    dllTestSourceFile,
                    15);

            TestCase executableUsingDLLFailedTestCase = testCases[4];
            TestCommon.AssertTestCase(executableUsingDLLFailedTestCase,
                    _exampleExecutableUsingDLLFilePath,
                    "TestExecutableUsingDLL::Empty Class::[ExecutableUsingDLL] Testing IsEven Always Fail",
                    "[ExecutableUsingDLL] Testing IsEven Always Fail",
                    executableUsingDLLTestSourceFile,
                    15);

            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
            Captured<TestResult> capturedTestResults = A.Captured<TestResult>();
            IRunContext runContext = A.Fake<IRunContext>();
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            A.CallTo(() => frameworkHandle.RecordStart(capturedTestCases._))
                .DoesNothing();
            A.CallTo(() => frameworkHandle.RecordResult(capturedTestResults._))
                .DoesNothing();

            ITestExecutor doctestTestExecutor = new DoctestTestExecutor();

            // Write any console output to a string so we can verify the executor has worked correctly.
            string testExecutorOutput = string.Empty;
            using (StringWriter stringWriter = new StringWriter())
            {
                // Console.Write/WriteLine should write to stringWriter now.
                TextWriter previousWriter = Console.Out;
                Console.SetOut(stringWriter);

                doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

                testExecutorOutput = stringWriter.ToString();

                // Reset back to standard output.
                Console.SetOut(previousWriter);
            }

            Assert.IsTrue(testExecutorOutput.Contains(dllTestSourceFile));
            Assert.IsTrue(testExecutorOutput.Contains(dllFailedTestCase.DisplayName));
            Assert.IsTrue(testExecutorOutput.Contains(@"ERROR: CHECK( IsEven(7) ) is NOT correct!"));
            Assert.IsTrue(testExecutorOutput.Contains(@"ERROR: CHECK( IsEven(9) ) is NOT correct!"));
            Assert.IsTrue(testExecutorOutput.Contains(@"ERROR: CHECK( IsEven(11) ) is NOT correct!"));

            Assert.IsTrue(testExecutorOutput.Contains(executableUsingDLLTestSourceFile));
            Assert.IsTrue(testExecutorOutput.Contains(executableUsingDLLFailedTestCase.DisplayName));
            Assert.IsTrue(testExecutorOutput.Contains(@"ERROR: CHECK( IsEven(1) ) is NOT correct!"));
            Assert.IsTrue(testExecutorOutput.Contains(@"ERROR: CHECK( IsEven(3) ) is NOT correct!"));
            Assert.IsTrue(testExecutorOutput.Contains(@"ERROR: CHECK( IsEven(5) ) is NOT correct!"));

            string doctestFinalOutput = @"[doctest] test cases:  4 | 2 passed | 2 failed | 2 skipped
[doctest] assertions: 12 | 6 passed | 6 failed |
[doctest] Status: FAILURE!";

            Assert.IsTrue(testExecutorOutput.Contains(doctestFinalOutput));
        }
	}
}
