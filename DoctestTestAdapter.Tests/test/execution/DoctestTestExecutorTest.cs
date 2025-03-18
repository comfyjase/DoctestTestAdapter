using DoctestTestAdapter.Shared.Helpers;
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
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
            Assert.HasCount(25, testCases);

            TestCommon.AssertTestCases(testCases, 
                TestCommon.UsingDoctestMainExecutableFilePath,
                "UsingDoctestMain",
                TestCommon.UsingDoctestMainTestHeaderFilePath);

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
            
            Assert.HasCount(25, capturedTestResults.Values);
            TestCommon.AssertTestResults(capturedTestResults.Values.ToList());
        }

        [TestMethod]
        public void ExecuteExeAndDLL()
        {
            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.ExecutableUsingDLLExecutableFilePath);
            Assert.HasCount(50, testCases);

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
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            A.CallTo(() => frameworkHandle.RecordStart(capturedTestCases._))
                .DoesNothing();
            A.CallTo(() => frameworkHandle.RecordResult(capturedTestResults._))
                .DoesNothing();

            ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
            doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

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
	}
}
